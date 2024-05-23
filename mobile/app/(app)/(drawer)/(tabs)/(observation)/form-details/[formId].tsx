import { router, useLocalSearchParams } from "expo-router";
import { Screen } from "../../../../../../components/Screen";
import Header from "../../../../../../components/Header";
import { Icon } from "../../../../../../components/Icon";
import { useUserData } from "../../../../../../contexts/user/UserContext.provider";
import { Typography } from "../../../../../../components/Typography";
import { YStack } from "tamagui";
import { useMemo, useState } from "react";
import { ListView } from "../../../../../../components/ListView";
import { Platform } from "react-native";
import OptionsSheet from "../../../../../../components/OptionsSheet";
import ChangeLanguageDialog from "../../../../../../components/ChangeLanguageDialog";
import { setFormLanguagePreference } from "../../../../../../common/language.preferences";
import { useFormById } from "../../../../../../services/queries/forms.query";
import { useFormAnswers } from "../../../../../../services/queries/form-submissions.query";
import FormQuestionListItem, {
  FormQuestionListItemProps,
  QuestionStatus,
} from "../../../../../../components/FormQuestionListItem";
import FormOverview from "../../../../../../components/FormOverview";
import { useTranslation } from "react-i18next";
import { useFormSubmissionMutation } from "../../../../../../services/mutations/form-submission.mutation";
import { shouldDisplayQuestion } from "../../../../../../services/form.parser";
import { useAttachments } from "../../../../../../services/queries/attachments.query";
import { useNotesForFormId } from "../../../../../../services/queries/notes.query";

type SearchParamsType = {
  formId: string;
  language: string;
};

const FormDetails = () => {
  const { t } = useTranslation(["form_overview", "common"]);
  const { formId, language } = useLocalSearchParams<SearchParamsType>();

  if (!formId || !language) {
    return <Typography>Incorrect page params</Typography>;
  }

  const { activeElectionRound, selectedPollingStation } = useUserData();
  const [isChangeLanguageModalOpen, setIsChangeLanguageModalOpen] = useState<boolean>(false);
  const [optionSheetOpen, setOptionSheetOpen] = useState(false);

  const { mutate: updateSubmission } = useFormSubmissionMutation({
    electionRoundId: activeElectionRound?.id,
    pollingStationId: selectedPollingStation?.pollingStationId,
    scopeId: `Submit_Answers_${activeElectionRound?.id}_${selectedPollingStation?.pollingStationId}_${formId}`,
  });

  const {
    data: currentForm,
    isLoading: isLoadingCurrentForm,
    error: currentFormError,
  } = useFormById(activeElectionRound?.id, formId);

  const {
    data: answers,
    isLoading: isLoadingAnswers,
    error: answersError,
  } = useFormAnswers(activeElectionRound?.id, selectedPollingStation?.pollingStationId, formId);

  const { data: attachments } = useAttachments(
    activeElectionRound?.id,
    selectedPollingStation?.pollingStationId,
    formId,
  );

  const { data: notes } = useNotesForFormId(
    activeElectionRound?.id,
    selectedPollingStation?.pollingStationId,
    formId,
  );

  const questions = useMemo(() => {
    return currentForm?.questions
      .filter((q) => shouldDisplayQuestion(q, answers))
      .map((q) => ({
        status: answers?.[q.id] ? QuestionStatus.ANSWERED : QuestionStatus.NOT_ANSWERED,
        question: q.text[language],
        numberOfNotes: notes?.[q.id]?.length || 0,
        numberOfAttachments: attachments?.[q.id]?.length || 0,
        id: q.id,
      }));
  }, [currentForm, answers, attachments, notes]);

  const { numberOfQuestions, formTitle, languages } = useMemo(() => {
    return {
      numberOfQuestions: questions?.length || 0,
      formTitle: `${currentForm?.code} - ${currentForm?.name[language]} (${language})`,
      languages: currentForm?.languages,
    };
  }, [currentForm, questions]);

  const onQuestionItemClick = (questionId: string) => {
    router.push(`/form-questionnaire/${questionId}?formId=${formId}&language=${language}`);
  };

  const onFormOverviewActionClick = () => {
    // find first unanswered question
    // do not navigate if the form has no questions or not found
    if (!currentForm || currentForm.questions.length === 0) return;
    // get the first unanswered question
    const lastQ = questions?.find((q) => !answers?.[q.id]);
    // if all questions are answered get the last question
    const lastQId = lastQ?.id || currentForm?.questions[currentForm.questions.length - 1].id;
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
    if (selectedPollingStation?.pollingStationId && activeElectionRound) {
      updateSubmission({
        pollingStationId: selectedPollingStation?.pollingStationId,
        electionRoundId: activeElectionRound?.id,
        formId: currentForm?.id as string,
        answers: [],
      });
      setOptionSheetOpen(false);
    }
  };

  if (isLoadingCurrentForm || isLoadingAnswers) {
    return <Typography>{t("loading", { ns: "common" })}</Typography>;
  }

  if (currentFormError || answersError) {
    return (
      <Screen preset="fixed">
        <Header
          title={`${formId}`}
          titleColor="white"
          barStyle="light-content"
          leftIcon={<Icon icon="chevronLeft" color="white" />}
          onLeftPress={() => router.back()}
        />
        <YStack paddingVertical="$xxl" alignItems="center">
          <Typography>{t("error")}</Typography>
        </YStack>
      </Screen>
    );
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
      <YStack paddingTop={28} gap="$xl" paddingHorizontal="$md" style={{ flex: 1 }}>
        <ListView<
          Pick<FormQuestionListItemProps, "question" | "status"> & {
            id: string;
            numberOfAttachments: number;
            numberOfNotes: number;
          }
        >
          data={questions}
          ListHeaderComponent={
            <YStack gap="$xl" paddingBottom="$xxs">
              <FormOverview
                completedAnswers={Object.keys(answers || {}).length}
                numberOfQuestions={numberOfQuestions}
                onFormActionClick={onFormOverviewActionClick}
              />
              <Typography preset="body1" fontWeight="700" gap="$xxs">
                {t("questions.title")}
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
      {/* //todo: change this once tamagui fixes sheet issue #2585 */}
      {(optionSheetOpen || Platform.OS === "ios") && (
        <OptionsSheet open={optionSheetOpen} setOpen={setOptionSheetOpen}>
          <YStack paddingHorizontal="$sm" gap="$xxs">
            <Typography preset="body1" paddingVertical="$md" onPress={onChangeLanguagePress}>
              {t("menu.change_language")}
            </Typography>
            <Typography preset="body1" paddingVertical="$md" onPress={onClearAnswersPress}>
              {t("menu.clear")}
            </Typography>
          </YStack>
        </OptionsSheet>
      )}
    </Screen>
  );
};

export default FormDetails;
