import { router, useLocalSearchParams } from "expo-router";
import { useMemo, useState } from "react";
import { useTranslation } from "react-i18next";
import { RefreshControl } from "react-native";
import { useSafeAreaInsets } from "react-native-safe-area-context";
import { ScrollView, Separator, Spinner, YStack } from "tamagui";
import { getAnswerDisplay } from "../../../../../../../common/utils/answers";
import Button from "../../../../../../../components/Button";
import { QuestionStatus } from "../../../../../../../components/FormQuestionListItem";
import Header from "../../../../../../../components/Header";
import { Icon } from "../../../../../../../components/Icon";
import { Screen } from "../../../../../../../components/Screen";
import { Typography } from "../../../../../../../components/Typography";
import { useNetInfoContext } from "../../../../../../../contexts/net-info-banner/NetInfoContext";
import { useUserData } from "../../../../../../../contexts/user/UserContext.provider";
import { shouldDisplayQuestion } from "../../../../../../../services/form.parser";
import {
  ApiFormAnswer,
  MultiSelectAnswer,
  SingleSelectAnswer,
} from "../../../../../../../services/interfaces/answer.type";
import { useDeleteFormSubmissionMutation } from "../../../../../../../services/mutations/form-submission.mutation";
import { useAttachments } from "../../../../../../../services/queries/attachments.query";
import { useFormSubmissionById } from "../../../../../../../services/queries/form-submissions.query";
import { useFormById } from "../../../../../../../services/queries/forms.query";
import { useNotesForSubmission } from "../../../../../../../services/queries/notes.query";
import WarningDialog from "../../../../../../../components/WarningDialog";

type SearchParamsType = {
  formId: string;
  language: string;
  submissionId: string;
  submissionNumber: string;
};

const DeleteFormSubmission = () => {
  const { t } = useTranslation(["delete_submission", "common"]);
  const { formId, submissionId, language, submissionNumber } =
    useLocalSearchParams<SearchParamsType>();
  const { isOnline } = useNetInfoContext();

  if (!formId || !language || !submissionId) {
    return <Typography>FormSubmissionDetails Incorrect page params</Typography>;
  }

  const { activeElectionRound, selectedPollingStation } = useUserData();
  const [isRefreshing, setIsRefreshing] = useState(false);
  const [showWarningModal, setShowWarningModal] = useState(false);
  const insets = useSafeAreaInsets();

  const { mutate: deleteSubmission } = useDeleteFormSubmissionMutation({
    electionRoundId: activeElectionRound?.id,
    pollingStationId: selectedPollingStation?.pollingStationId,
    scopeId: `Delete_${activeElectionRound?.id}_${submissionId}`,
  });

  const {
    data: currentForm,
    isLoading: isLoadingCurrentForm,
    error: currentFormError,
    refetch: refetchCurrentForm,
  } = useFormById(activeElectionRound?.id, formId);

  const {
    data: currentFormSubmission,
    isLoading: isLoadingAnswers,
    error: answersError,
    refetch: refetchAnswers,
  } = useFormSubmissionById(
    activeElectionRound?.id,
    selectedPollingStation?.pollingStationId,
    submissionId,
  );

  const answers = useMemo(() => currentFormSubmission?.answers, [currentFormSubmission]);
  // const isCompleted = useMemo(
  //   () => currentFormSubmission?.isCompleted || false,
  //   [currentFormSubmission],
  // );

  const { data: attachments, refetch: refetchAttachments } = useAttachments(
    activeElectionRound?.id,
    submissionId,
  );

  const { data: notes, refetch: refetchNotes } = useNotesForSubmission(
    activeElectionRound?.id,
    submissionId,
  );

  const handleRefetch = () => {
    setIsRefreshing(true);
    Promise.all([
      refetchCurrentForm(),
      refetchAnswers(),
      refetchAttachments(),
      refetchNotes(),
    ]).then(() => {
      setIsRefreshing(false);
    });
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

        // sort the notes by lastUpdatedAt date in order to extract the last added note
        const lastNote =
          notes && notes[q.id]
            ? notes[q.id]
                .slice()
                .sort((a, b) => +new Date(a.lastUpdatedAt) - +new Date(b.lastUpdatedAt))
                .pop()
            : undefined;

        return {
          ...q,
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
        };
      });
  }, [currentForm, answers, attachments, notes]);

  const { formTitle } = useMemo(() => {
    return {
      numberOfQuestions: questions?.length || 0,
      formTitle: `${currentForm?.code} - ${currentForm?.name[language]} (${language})`,
      languages: currentForm?.languages,
    };
  }, [currentForm, questions]);

  const mappedAnswers = useMemo(() => {
    if (!answers || !questions) return {};

    return Object.entries(answers).reduce(
      (acc, [questionId, answer]) => {
        if (!answer) return acc;
        const question = questions.find((q) => q.id === questionId);
        if (!question) return acc;

        let mappedAnswer = { ...answer } as SingleSelectAnswer | MultiSelectAnswer;

        if (
          answer.$answerType === "singleSelectAnswer" &&
          question.$questionType === "singleSelectQuestion"
        ) {
          const selectedOptionId = answer.selection.optionId;

          const selectedOptionText = question.options.find((o) => o.id === selectedOptionId)?.text[
            language
          ];

          mappedAnswer = {
            ...mappedAnswer,
            selection: {
              ...mappedAnswer.selection,
              value: selectedOptionText,
            },
          } as SingleSelectAnswer;
        } else if (
          answer.$answerType === "multiSelectAnswer" &&
          question.$questionType === "multiSelectQuestion"
        ) {
          const selectedOptionIds = answer.selection.map((selection) => selection.optionId);
          const selectedOptionsTexts: string[] = [];
          for (const option of question.options) {
            if (selectedOptionIds.includes(option.id)) {
              const selectedOption = answer.selection.find(
                (selection) => selection.optionId === option.id,
              );
              const optionText = option.text[language] || "";
              const userText = selectedOption?.text || "";
              selectedOptionsTexts.push(
                userText ? `${optionText} (${userText.trim()})` : optionText,
              );
            }
          }

          mappedAnswer = {
            ...mappedAnswer,
            selection: answer.selection,
            selectionValues: selectedOptionsTexts,
          } as MultiSelectAnswer;
        }

        acc[questionId] = mappedAnswer;
        return acc;
      },
      {} as Record<string, ApiFormAnswer>,
    );
  }, [answers, questions]);
  //   only display the questions that have an answer to them (because in case of skip logic we would have multiple questions that were not shown to the user)
  const displayedQuestions = useMemo(() => {
    if (!currentForm?.questions || !mappedAnswers) return [];

    return currentForm.questions.filter((question) => mappedAnswers[question.id]);
  }, [currentForm?.questions, mappedAnswers]);

  const handleDelete = async () => {
    if (!activeElectionRound) {
      return;
    }

    deleteSubmission(
      {
        electionRoundId: activeElectionRound.id,
        id: submissionId,
      },
      {
        onSuccess: () => {
          setShowWarningModal(false);
          router.push(`/multi-submission-form-details/${formId}?language=${language}`);
        },
      },
    );
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
          bounces={isOnline}
          refreshControl={<RefreshControl refreshing={isRefreshing} onRefresh={handleRefetch} />}
        >
          <Typography>{t("error")}</Typography>
        </ScrollView>
      </Screen>
    );
  }

  return (
    <Screen preset="fixed" contentContainerStyle={{ flexGrow: 1 }}>
      <Header
        title={t("header", { submissionNumber })}
        titleColor="white"
        barStyle="light-content"
        leftIcon={<Icon icon="chevronLeft" color="white" />}
        onLeftPress={() => router.back()}
      />
      <YStack paddingTop={28} gap="$xl" paddingHorizontal="$md" flex={1}>
        <YStack flex={1} marginBottom={insets.bottom + 16}>
          <Typography preset="heading"> {formTitle}</Typography>
          {displayedQuestions.map((question) => (
            <YStack key={question.id} marginTop="$md">
              <Typography fontWeight="500">
                {question.code} - {question.text[language]}
              </Typography>
              {mappedAnswers && mappedAnswers[question.id] && (
                <Typography marginTop="$xs" color="$gray5">
                  {getAnswerDisplay(mappedAnswers[question.id] as ApiFormAnswer, true)}
                </Typography>
              )}
              {attachments && attachments[question.id] ? (
                <YStack gap="$xxs">
                  <Typography fontWeight="500">
                    {t("attachments", { value: attachments[question.id].length })}
                  </Typography>
                </YStack>
              ) : (
                false
              )}

              {notes && notes[question.id] ? (
                <YStack gap="$xxs">
                  <Typography fontWeight="500">
                    {t("notes", { value: notes[question.id].length })}
                  </Typography>
                </YStack>
              ) : (
                false
              )}
              <Separator marginTop="$xs" />
            </YStack>
          ))}

          <YStack paddingHorizontal="$lg" paddingTop="$md">
            <Button preset="red" onPress={() => setShowWarningModal(true)}>
              {t("delete")}
            </Button>
          </YStack>
        </YStack>
      </YStack>

      {showWarningModal && (
        <WarningDialog
          theme="info"
          title={t("warning_dialog.title")}
          titleProps={{ textAlign: "center" }}
          description={t("warning_dialog.description")}
          actionBtnText={t("warning_dialog.delete")}
          cancelBtnText={t("warning_dialog.cancel")}
          action={handleDelete}
          onCancel={() => setShowWarningModal(false)}
        />
      )}
    </Screen>
  );
};

export default DeleteFormSubmission;
