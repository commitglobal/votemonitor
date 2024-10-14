import { router, useLocalSearchParams } from "expo-router";
import { Screen } from "../../../../../../../components/Screen";
import Header from "../../../../../../../components/Header";
import { Icon } from "../../../../../../../components/Icon";
import { useUserData } from "../../../../../../../contexts/user/UserContext.provider";
import { Typography } from "../../../../../../../components/Typography";
import { ScrollView, Spinner, useWindowDimensions, YStack } from "tamagui";
import { useMemo, useState } from "react";
import { ListView } from "../../../../../../../components/ListView";
import OptionsSheet from "../../../../../../../components/OptionsSheet";
import ChangeLanguageDialog from "../../../../../../../components/ChangeLanguageDialog";
import { setFormLanguagePreference } from "../../../../../../../common/language.preferences";
import { useFormById } from "../../../../../../../services/queries/forms.query";
import { useFormSubmissionByFormId } from "../../../../../../../services/queries/form-submissions.query";
import FormQuestionListItem, {
  FormQuestionListItemProps,
  QuestionStatus,
} from "../../../../../../../components/FormQuestionListItem";
import FormOverview from "../../../../../../../components/FormOverview";
import { useTranslation } from "react-i18next";
import {
  useFormSubmissionMutation,
  useMarkFormSubmissionCompletionStatusMutation,
} from "../../../../../../../services/mutations/form-submission.mutation";
import { shouldDisplayQuestion } from "../../../../../../../services/form.parser";
import WarningDialog from "../../../../../../../components/WarningDialog";
import { useAttachments } from "../../../../../../../services/queries/attachments.query";
import { useNotesForFormId } from "../../../../../../../services/queries/notes.query";
import { RefreshControl } from "react-native";

const ESTIMATED_ITEM_SIZE = 100;

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
  const [clearingForm, setClearingForm] = useState(false);

  const { width } = useWindowDimensions();

  const { mutate: updateSubmission } = useFormSubmissionMutation({
    electionRoundId: activeElectionRound?.id,
    pollingStationId: selectedPollingStation?.pollingStationId,
    scopeId: `Submit_Answers_${activeElectionRound?.id}_${selectedPollingStation?.pollingStationId}_${formId}`,
  });

  const { mutate: setFormSubmissionCompletionStatus } =
    useMarkFormSubmissionCompletionStatusMutation({
      electionRoundId: activeElectionRound?.id,
      pollingStationId: selectedPollingStation?.pollingStationId,
      // BR: Both updateSubmission and setFormSubmissionCompletionStatus use the same scopeId because you can't markCompletionStatus without an existing submissionId
      scopeId: `Submit_Answers_${activeElectionRound?.id}_${selectedPollingStation?.pollingStationId}_${formId}`,
    });

  const {
    data: currentForm,
    isLoading: isLoadingCurrentForm,
    error: currentFormError,
    refetch: refetchCurrentForm,
    isRefetching: isRefetchingCurrentForm,
  } = useFormById(activeElectionRound?.id, formId);

  const {
    data: currentFormSubmission,
    isLoading: isLoadingAnswers,
    error: answersError,
    refetch: refetchAnswers,
    isRefetching: isRefetchingAnswers,
  } = useFormSubmissionByFormId(
    activeElectionRound?.id,
    selectedPollingStation?.pollingStationId,
    formId,
  );

  const answers = useMemo(() => currentFormSubmission?.answers, [currentFormSubmission]);
  const isCompleted = useMemo(
    () => currentFormSubmission?.isCompleted || false,
    [currentFormSubmission],
  );

  const {
    data: attachments,
    refetch: refetchAttachments,
    isRefetching: isRefetchingAttachments,
  } = useAttachments(activeElectionRound?.id, selectedPollingStation?.pollingStationId, formId);

  const {
    data: notes,
    refetch: refetchNotes,
    isRefetching: isRefetchingNotes,
  } = useNotesForFormId(activeElectionRound?.id, selectedPollingStation?.pollingStationId, formId);

  const isRefetching = useMemo(
    () =>
      isRefetchingCurrentForm ||
      isRefetchingAnswers ||
      isRefetchingAttachments ||
      isRefetchingNotes,
    [isRefetchingCurrentForm, isRefetchingAnswers, isRefetchingAttachments, isRefetchingNotes],
  );

  const handleRefetch = () => {
    refetchCurrentForm();
    refetchAnswers();
    refetchAttachments();
    refetchNotes();
  };

  const questions = useMemo(() => {
    return currentForm?.questions
      .filter((q) => shouldDisplayQuestion(q, answers))
      .map((q) => {
        let answer = answers?.[q.id];

        // if answer type is singleSelect, we need to get the selected option text, based on the optionId
        // we take it from the question options, because that's the only place we have access to the option text
        if (
          answer &&
          answer.$answerType === "singleSelectAnswer" &&
          q.$questionType === "singleSelectQuestion"
        ) {
          const selectedOptionId = answer?.selection.optionId;
          const selectedOptionText = q.options.find((o) => o.id === selectedOptionId)?.text[
            language
          ];

          answer = {
            ...answer,
            selection: {
              ...answer.selection,
              value: selectedOptionText,
            },
          };
        }

        // if answer type is multiSelect, we need to get the texts for all of the options, based on the optionIds
        // we take them from the question options, because that's the only place we have access to the option text
        if (
          answer &&
          answer.$answerType === "multiSelectAnswer" &&
          q.$questionType === "multiSelectQuestion"
        ) {
          const answerIds = answer.selection.map((selection) => selection.optionId);
          const selectedAnswersTexts = q.options
            .filter((o) => answerIds.includes(o.id))
            .map((selection) => selection.text[language]);
          answer = { ...answer, selectionValues: selectedAnswersTexts };
        }

        // sort the notes by createdAt date in order to extract the last added note
        const lastNote =
          notes && notes[q.id]
            ? notes[q.id]
                .slice()
                .sort((a, b) => +new Date(a.createdAt) - +new Date(b.createdAt))
                .pop()
            : undefined;

        return {
          question: q.text[language],
          status: answers?.[q.id] ? QuestionStatus.ANSWERED : QuestionStatus.NOT_ANSWERED,
          answer,
          numberOfNotes: notes?.[q.id]?.length || 0,
          lastNoteText: lastNote && lastNote.text,
          numberOfAttachments: attachments?.[q.id]?.length || 0,
          // array with the types of attachments for this question
          attachmentTypes: attachments?.[q.id] && [
            ...new Set(attachments?.[q.id].map((attachment) => attachment.mimeType)),
          ],
          id: q.id,
        };
      });
  }, [currentForm, answers, attachments, notes]);

  const { numberOfQuestions, formTitle, languages } = useMemo(() => {
    return {
      numberOfQuestions: questions?.length || 0,
      formTitle: `${currentForm?.code} - ${currentForm?.name[language]} (${language})`,
      languages: currentForm?.languages,
    };
  }, [currentForm, questions]);

  const disableMarkAsDone = useMemo(() => {
    const answersLength = Object.keys(answers || {}).length;
    return (answersLength === 0 || answersLength === numberOfQuestions) && !isCompleted;
  }, [answers, numberOfQuestions, isCompleted]);

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

  const onClearFormPress = () => {
    setOptionSheetOpen(false);
    setClearingForm(true);
  };

  const onClearAnswersPress = () => {
    if (selectedPollingStation?.pollingStationId && activeElectionRound) {
      updateSubmission({
        pollingStationId: selectedPollingStation?.pollingStationId,
        electionRoundId: activeElectionRound?.id,
        formId: currentForm?.id as string,
        answers: [],
      });
      setClearingForm(false);
    }
  };

  if (isLoadingCurrentForm || isLoadingAnswers) {
    return (
      <Screen preset="fixed" contentContainerStyle={{ flexGrow: 1 }}>
        <Header
          title={`${formId}`}
          titleColor="white"
          barStyle="light-content"
          leftIcon={<Icon icon="chevronLeft" color="white" />}
          onLeftPress={() => router.back()}
        />
        <YStack justifyContent="center" alignItems="center" flex={1}>
          <Spinner size="large" color="$purple5" />
        </YStack>
      </Screen>
    );
  }

  if (currentFormError || answersError) {
    return (
      <Screen preset="fixed" contentContainerStyle={{ flexGrow: 1 }}>
        <Header
          title={`${formId}`}
          titleColor="white"
          barStyle="light-content"
          leftIcon={<Icon icon="chevronLeft" color="white" />}
          onLeftPress={() => router.back()}
        />
        <ScrollView
          contentContainerStyle={{ flex: 1, alignItems: "center" }}
          paddingVertical="$xxl"
          showsVerticalScrollIndicator={false}
          refreshControl={<RefreshControl refreshing={isRefetching} onRefresh={handleRefetch} />}
        >
          <Typography>{t("error")}</Typography>
        </ScrollView>
      </Screen>
    );
  }

  return (
    <Screen preset="fixed" contentContainerStyle={{ flexGrow: 1 }}>
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
      <YStack paddingTop={28} gap="$xl" paddingHorizontal="$md" flex={1}>
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
                isCompleted={isCompleted}
              />
              <Typography preset="body1" fontWeight="700" gap="$xxs">
                {t("questions.title")}
              </Typography>
            </YStack>
          }
          showsVerticalScrollIndicator={false}
          bounces={true}
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
          estimatedItemSize={ESTIMATED_ITEM_SIZE}
          estimatedListSize={{ height: ESTIMATED_ITEM_SIZE * 5, width: width - 32 }}
          refreshControl={<RefreshControl refreshing={isRefetching} onRefresh={handleRefetch} />}
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
      {clearingForm && (
        <WarningDialog
          title={t("clear_answers_modal.title", { value: formTitle })}
          description={
            <YStack gap="$md">
              <Typography preset="body1" color="$gray6">
                {t("clear_answers_modal.description.p1")}
              </Typography>
              <Typography preset="body1" color="$gray6">
                {t("clear_answers_modal.description.p2")}
              </Typography>
            </YStack>
          }
          actionBtnText={t("clear_answers_modal.actions.clear")}
          cancelBtnText={t("clear_answers_modal.actions.cancel")}
          onCancel={setClearingForm.bind(null, false)}
          action={onClearAnswersPress}
        />
      )}
      {/* //todo: change this once tamagui fixes sheet issue #2585 */}
      {optionSheetOpen && (
        <OptionsSheet open setOpen={setOptionSheetOpen}>
          <YStack paddingHorizontal="$sm" gap="$xxs">
            <Typography
              preset="body1"
              color={disableMarkAsDone ? "$gray3" : "$gray7"}
              lineHeight={24}
              onPress={() => {
                if (activeElectionRound?.id && selectedPollingStation?.pollingStationId) {
                  setFormSubmissionCompletionStatus({
                    electionRoundId: activeElectionRound?.id,
                    pollingStationId: selectedPollingStation?.pollingStationId,
                    formId: currentForm?.id as string,
                    isCompleted: !isCompleted,
                  });
                  setOptionSheetOpen(false);
                }
              }}
              disabled={disableMarkAsDone}
            >
              {!isCompleted
                ? t("forms.mark_as_done", { ns: "common" })
                : t("forms.mark_as_in_progress", { ns: "common" })}
            </Typography>
            <Typography preset="body1" paddingVertical="$md" onPress={onChangeLanguagePress}>
              {t("menu.change_language")}
            </Typography>
            <Typography preset="body1" paddingVertical="$md" onPress={onClearFormPress}>
              {t("menu.clear")}
            </Typography>
          </YStack>
        </OptionsSheet>
      )}
    </Screen>
  );
};

export default FormDetails;
