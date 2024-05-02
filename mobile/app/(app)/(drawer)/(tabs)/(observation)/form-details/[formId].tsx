import { router, useLocalSearchParams } from "expo-router";
import { Screen } from "../../../../../../components/Screen";
import Header from "../../../../../../components/Header";
import { Icon } from "../../../../../../components/Icon";
import { useUserData } from "../../../../../../contexts/user/UserContext.provider";
import { Typography } from "../../../../../../components/Typography";
import { YStack } from "tamagui";
import { useMemo, useState } from "react";
import { ListView } from "../../../../../../components/ListView";
import { mapAPIAnswersToFormAnswers } from "../../../../../../services/form.parser";
import { ApiFormAnswer } from "../../../../../../services/interfaces/answer.type";
import { Dimensions, Platform } from "react-native";
import OptionsSheet from "../../../../../../components/OptionsSheet";
import ChangeLanguageDialog from "../../../../../../components/ChangeLanguageDialog";
import { setFormLanguagePreference } from "../../../../../../common/language.preferences";
import { useElectionRoundAllForms } from "../../../../../../services/queries/forms.query";
import { useFormSubmissions } from "../../../../../../services/queries/form-submissions.query";
import FormQuestionListItem, {
  FormQuestionListItemProps,
  QuestionStatus,
} from "../../../../../../components/FormQuestionListItem";
import FormOverview from "../../../../../../components/FormOverview";

const FormDetails = () => {
  const { formId, language } = useLocalSearchParams();
  const { activeElectionRound, selectedPollingStation } = useUserData();
  const [isChangeLanguageModalOpen, setIsChangeLanguageModalOpen] = useState<boolean>(false);
  const [optionSheetOpen, setOptionSheetOpen] = useState(false);

  const {
    data: allForms,
    isLoading: isLoadingForms,
    error: formsError,
  } = useElectionRoundAllForms(activeElectionRound?.id);

  const {
    data: formSubmissions,
    isLoading: isLoadingAnswers,
    error: answersError,
  } = useFormSubmissions(activeElectionRound?.id, selectedPollingStation?.pollingStationId);

  const answers: Record<string, ApiFormAnswer> = useMemo(() => {
    const formSubmission = formSubmissions?.submissions.find(
      (sub) => sub.formId === (formId as string),
    );
    return mapAPIAnswersToFormAnswers(formSubmission?.answers);
  }, [formSubmissions]);

  const { questions, numberOfAnswers } = useMemo(() => {
    const form = allForms?.forms.find((form) => form.id === formId);
    const submission = formSubmissions?.submissions.find(
      (submission) => submission.formId === formId,
    );
    return {
      questions: form?.questions.map((q) => ({
        status: answers[q.id] ? QuestionStatus.ANSWERED : QuestionStatus.NOT_ANSWERED,
        question: q.text[language as string],
        id: q.id,
      })),
      numberOfAnswers: submission ? submission.answers.length : 0,
    };
  }, [allForms, formSubmissions]);

  const { numberOfQuestions, formTitle, languages } = useMemo(() => {
    const form = allForms?.forms.find((form) => form.id === formId);
    return {
      numberOfQuestions: form ? form.questions.length : 0,
      formTitle: `${form?.code} - ${form?.name[language as string]} (${language as string})`,
      languages: form?.languages,
    };
  }, [allForms]);

  const onQuestionItemClick = (questionId: string) => {
    router.push(`/form-questionnaire/${questionId}?formId=${formId}&language=${language}`);
  };

  const onFormOverviewActionClick = () => {
    // find first unanswered question
    const form = allForms?.forms.find((form) => form.id === formId);
    // do not navigate if the form has no questions or not found
    if (!form || form.questions.length === 0) return;
    // get the first unanswered question
    const lastQ = questions?.find((q) => !answers[q.id]);
    // if all questions are answered get the last question
    const lastQId = lastQ?.id || form?.questions[form.questions.length - 1].id;
    return router.push(`/form-questionnaire/${lastQId}?formId=${formId}&language=${language}`);
  };

  const onChangeLanguagePress = () => {
    setOptionSheetOpen(false);
    setIsChangeLanguageModalOpen(true);
  };

  const onConfirmFormLanguage = (formId: string, language: string) => {
    setFormLanguagePreference({ formId, language });

    router.replace(`/form-details/${formId}?language=${language}`);
    setIsChangeLanguageModalOpen(false);
  };

  const onClearAnswersPress = () => {
    console.log("clear data");
  };

  if (isLoadingForms || isLoadingAnswers) {
    return <Typography>Loading</Typography>;
  }

  if (formsError || answersError) {
    return <Typography>Form Error</Typography>;
  }

  return (
    <Screen
      preset="scroll"
      ScrollViewProps={{
        stickyHeaderIndices: [0],
        bounces: false,
        showsVerticalScrollIndicator: false,
      }}
    >
      <Header
        title={`${formTitle}`}
        titleColor="white"
        barStyle="light-content"
        leftIcon={<Icon icon="chevronLeft" color="white" />}
        onLeftPress={() => router.back()}
        rightIcon={<Icon icon="dotsVertical" color="white" />}
        onRightPress={() => {
          setOptionSheetOpen(true);
        }}
      />
      <YStack
        paddingTop={28}
        gap="$xl"
        paddingHorizontal="$md"
        height={
          Platform.OS === "ios"
            ? Dimensions.get("screen").height - 120
            : Dimensions.get("screen").height * 1.4 // TODO: need to do something about this
        }
      >
        <ListView<Pick<FormQuestionListItemProps, "question" | "status"> & { id: string }>
          data={questions}
          ListHeaderComponent={
            <YStack gap="$xl" paddingBottom="$xxs">
              <FormOverview
                completedAnswers={numberOfAnswers}
                numberOfQuestions={numberOfQuestions}
                onFormActionClick={onFormOverviewActionClick}
              />
              <Typography preset="body1" fontWeight="700" gap="$xxs">
                Questions
              </Typography>
            </YStack>
          }
          showsVerticalScrollIndicator={false}
          bounces={false}
          renderItem={({ item, index }) => {
            return (
              <FormQuestionListItem
                key={index}
                {...item}
                index={index + 1}
                numberOfQuestions={numberOfQuestions}
                onClick={onQuestionItemClick.bind(null, item.id)}
              />
            );
          }}
          estimatedItemSize={100}
        />
      </YStack>
      {isChangeLanguageModalOpen && languages && (
        <ChangeLanguageDialog
          formId={formId as string}
          languages={languages}
          onCancel={setIsChangeLanguageModalOpen.bind(null, false)}
          onSelectLanguage={onConfirmFormLanguage}
        />
      )}
      <OptionsSheet open={optionSheetOpen} setOpen={setOptionSheetOpen}>
        <YStack paddingHorizontal="$sm" gap="$xxs">
          <Typography preset="body1" paddingVertical="$md" onPress={onChangeLanguagePress}>
            Change language
          </Typography>
          <Typography preset="body1" paddingVertical="$md" onPress={onClearAnswersPress}>
            Clear form (delete all answers)
          </Typography>
        </YStack>
      </OptionsSheet>
    </Screen>
  );
};

export default FormDetails;
