import { useMemo, useRef, useState } from "react";
import { ScrollView, XStack, YStack } from "tamagui";
import { Screen } from "../../../../components/Screen";
import Header from "../../../../components/Header";
import { useLocalSearchParams, useRouter } from "expo-router";
import { Icon } from "../../../../components/Icon";
import { useGetCitizenReportingFormById } from "../../../../services/queries/citizen.query";
import { useCitizenUserData } from "../../../../contexts/citizen-user/CitizenUserContext.provider";
import { Typography } from "../../../../components/Typography";
import { ViewStyle } from "react-native";
import LinearProgress from "../../../../components/LinearProgress";
import { useTranslation } from "react-i18next";
import { ApiFormQuestion } from "../../../../services/interfaces/question.type";
import { ApiFormAnswer } from "../../../../services/interfaces/answer.type";
import {
  mapFormSubmissionDataToAPIFormSubmissionAnswer,
  setFormDefaultValues,
  shouldDisplayQuestion,
} from "../../../../services/form.parser";
import QuestionForm from "../../../../components/QuestionForm";
import { useForm } from "react-hook-form";
import { scrollToTextarea } from "../../../../helpers/scrollToTextarea";
import WizzardControls from "../../../../components/WizzardControls";
import { usePostCitizenFormMutation } from "../../../../services/mutations/citizen/post-citizen-form.mutation";
import * as Crypto from "expo-crypto";
import { useNetInfoContext } from "../../../../contexts/net-info-banner/NetInfoContext";
import Toast from "react-native-toast-message";

const CitizenForm = () => {
  const { t } = useTranslation(["polling_station_form_wizard", "common", "network_banner"]); // TODO: change with citizen

  const language = "EN"; // TODO: remove this later
  const router = useRouter();
  const scrollViewRef = useRef(null);
  const textareaRef = useRef(null);

  const { selectedElectionRound } = useCitizenUserData();
  const { isOnline } = useNetInfoContext();

  if (!selectedElectionRound) {
    return (
      <Typography>
        [CitizenForm] There is no selected election round - Incorrect page params
      </Typography>
    );
  }

  const {
    formId,
    selectedLocationId,
    questionId: initialQuestionId,
  } = useLocalSearchParams<{
    formId: string;
    selectedLocationId: string;
    questionId: string;
  }>();

  if (!formId || !selectedLocationId || !initialQuestionId) {
    return <Typography>Incorrect page params</Typography>;
  }

  const handleFocus = () => {
    scrollToTextarea(scrollViewRef, textareaRef);
  };

  const [answers, setAnswers] = useState<Record<string, ApiFormAnswer | undefined> | undefined>();
  const [questionId, setQuestionId] = useState<string>(initialQuestionId);

  const {
    data: currentForm,
    isLoading: isLoadingCurrentForm,
    error: currentFormError,
  } = useGetCitizenReportingFormById(selectedElectionRound, formId);

  const { mutate: postCitizenForm } = usePostCitizenFormMutation();

  const displayedQuestions: ApiFormQuestion[] = useMemo(
    () => currentForm?.questions?.filter((q) => shouldDisplayQuestion(q, answers)) || [],
    [currentForm, answers],
  );

  const activeQuestion: {
    indexInAllQuestions: number;
    indexInDisplayedQuestions: number;
    question: ApiFormQuestion;
  } = useMemo(() => {
    return {
      indexInAllQuestions: currentForm?.questions.findIndex((q) => q.id === questionId) || 0,
      indexInDisplayedQuestions: displayedQuestions.findIndex((q) => q.id === questionId) || 0,
      question: currentForm?.questions.find((q) => q.id === questionId) as ApiFormQuestion,
    };
  }, [displayedQuestions, questionId]);

  const formTitle = useMemo(() => {
    return `${currentForm?.code} - ${currentForm?.name[language || currentForm.defaultLanguage]} (${language || currentForm?.defaultLanguage})`;
  }, [currentForm]);

  const { control, handleSubmit } = useForm({
    defaultValues: setFormDefaultValues(questionId, answers?.[questionId]) as any,
  });

  const findNextQuestion = (
    index: number,
    updatedAnswers: Record<string, ApiFormAnswer | undefined> | undefined,
  ): ApiFormQuestion | null => {
    if (index + 1 === currentForm?.questions.length) {
      // No more questions
      return null;
    }

    const nextQuestion = currentForm?.questions[index + 1];

    if (nextQuestion && shouldDisplayQuestion(nextQuestion, updatedAnswers)) {
      return nextQuestion;
    }

    return findNextQuestion(index + 1, updatedAnswers);
  };

  const findPreviousQuestion = (
    index: number,
    answers: Record<string, ApiFormAnswer | undefined> | undefined,
  ): ApiFormQuestion | null => {
    if (index - 1 < 0) {
      // No more questions
      return null;
    }

    const prevQ = currentForm?.questions[index - 1];

    if (prevQ && shouldDisplayQuestion(prevQ, answers)) {
      return prevQ;
    }

    return findPreviousQuestion(index - 1, answers);
  };

  const goToNextQuestion = (nextQuestion: ApiFormQuestion | null) => {
    if (nextQuestion) {
      setQuestionId(nextQuestion.id);
    }
  };

  const goToPrevQuestion = () => {
    const prevQ = findPreviousQuestion(activeQuestion.indexInAllQuestions, answers);
    if (prevQ) {
      setQuestionId(prevQ.id);
    }
  };

  const onSubmitAnswer = (formValues: any) => {
    const questionId = activeQuestion?.question.id as string;

    if (activeQuestion) {
      // map the answer values
      const updatedAnswer = mapFormSubmissionDataToAPIFormSubmissionAnswer(
        questionId,
        activeQuestion?.question.$questionType,
        formValues[questionId],
      );

      const updatedAnswers = {
        ...answers,
        [activeQuestion.question.id]: updatedAnswer,
      };

      // Find dependent questions for the current one and remove their answers
      currentForm?.questions
        ?.filter((q) => q.displayLogic?.parentQuestionId === questionId)
        .map((q) => q.id)
        .forEach((qId) => {
          if (updatedAnswers) delete updatedAnswers[qId];
        });

      const nextQuestion = findNextQuestion(activeQuestion.indexInAllQuestions, updatedAnswers);
      if (nextQuestion) {
        setAnswers(updatedAnswers);
        goToNextQuestion(nextQuestion);
      } else {
        if (!isOnline) {
          return Toast.show({
            type: "error",
            text2: t("offline_citizen", { ns: "network_banner" }),
            visibilityTime: 5000,
            text2Style: { textAlign: "center" },
          });
        }
        if (currentForm) {
          postCitizenForm(
            {
              electionRoundId: selectedElectionRound,
              citizenReportId: Crypto.randomUUID(),
              formId: currentForm.id,
              locationId: selectedLocationId,
              answers: Object.values(updatedAnswers).filter(Boolean) as ApiFormAnswer[],
            },
            {
              onSuccess: () => {
                console.log(
                  "🔵 [CitizenForm] form submitted successfully, redirect to success page",
                );
                router.replace("/citizen/main/form/success");
              },
              onError: (error) => {
                console.log("🔴 [CitizenForm] error submitting form", error);
              },
            },
          );
        }
      }
    }
  };

  if (isLoadingCurrentForm || !activeQuestion) {
    return <Typography>Loading...</Typography>;
  }

  if (currentFormError) {
    return <Typography>Error loading form {JSON.stringify(currentFormError)}</Typography>;
  }

  return (
    <Screen
      preset="fixed"
      backgroundColor="white"
      style={$screenStyle}
      contentContainerStyle={$containerStyle}
    >
      <Header
        title={formTitle}
        leftIcon={<Icon icon="chevronLeft" color="white" />}
        onLeftPress={() => {
          router.back();
        }}
      />
      <ScrollView
        ref={scrollViewRef}
        contentContainerStyle={{ flexGrow: 1 }}
        keyboardShouldPersistTaps="handled"
      >
        <YStack gap="$xxs" padding="$md">
          <XStack justifyContent="space-between">
            <Typography>{t("progress_bar.label")}</Typography>
            <Typography justifyContent="space-between">{`${activeQuestion?.indexInDisplayedQuestions + 1}/${displayedQuestions.length}`}</Typography>
          </XStack>
          <LinearProgress
            current={activeQuestion?.indexInDisplayedQuestions + 1}
            total={displayedQuestions?.length || 0}
          />
        </YStack>

        <YStack paddingHorizontal="$md" paddingBottom="$md" justifyContent="center" flex={1}>
          <QuestionForm
            control={control}
            activeQuestion={activeQuestion}
            handleFocus={handleFocus}
            ref={textareaRef}
            language={language}
            required={true}
          />

          {/* attachments */}
          {/* {activeElectionRound?.id && selectedPollingStation?.pollingStationId && formId && (
            <QuestionAttachments
              electionRoundId={activeElectionRound.id}
              pollingStationId={selectedPollingStation.pollingStationId}
              formId={formId}
              questionId={questionId}
            />
          )} */}

          {/* <AddAttachment
            label={t("attachments.add")}
            marginTop="$sm"
            onPress={() => {
              Keyboard.dismiss();
              return setIsOptionsSheetOpen(true);
            }}
          /> */}
        </YStack>
      </ScrollView>
      <WizzardControls
        isFirstElement={activeQuestion?.indexInAllQuestions === 0}
        isLastElement={
          currentForm?.questions &&
          activeQuestion?.indexInAllQuestions === currentForm?.questions?.length - 1
        }
        isNextDisabled={false}
        onActionButtonPress={handleSubmit(async (formValues) => {
          onSubmitAnswer(formValues);
        })}
        onPreviousButtonPress={() => {
          goToPrevQuestion();
        }}
      />
    </Screen>
  );
};

const $screenStyle: ViewStyle = {
  backgroundColor: "white",
  justifyContent: "space-between",
};

const $containerStyle: ViewStyle = {
  flex: 1,
};

export default CitizenForm;
