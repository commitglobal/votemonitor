import React, { Dispatch, SetStateAction, useMemo } from "react";
import { Icon } from "./Icon";
import { Typography } from "./Typography";
import Button from "./Button";
import { useSafeAreaInsets } from "react-native-safe-area-context";
import { FormAPIModel } from "../services/definitions.api";
import {
  ApiFormAnswer,
  MultiSelectAnswer,
  SingleSelectAnswer,
} from "../services/interfaces/answer.type";
import { Sheet, Spinner, YStack } from "tamagui";
import { getAnswerDisplay } from "../common/utils/answers";
import { ApiFormQuestion } from "../services/interfaces/question.type";
import { useTranslation } from "react-i18next";

const LoadingScreen = () => {
  const { t } = useTranslation("citizen_form");

  return (
    <YStack flex={1} alignItems="center" gap="$lg">
      <Spinner size="large" color="$purple5" />
      <Typography preset="body2" textAlign="center">
        {t("review.sending_report")}
      </Typography>
    </YStack>
  );
};

export default function ReviewCitizenFormSheet({
  currentForm,
  answers,
  questions,
  setIsReviewSheetOpen,
  onSubmit,
  isPending,
}: {
  currentForm: FormAPIModel | undefined;
  answers: Record<string, ApiFormAnswer | undefined> | undefined;
  questions: ApiFormQuestion[] | undefined;
  setIsReviewSheetOpen: Dispatch<SetStateAction<boolean>>;
  onSubmit: () => void;
  isPending: boolean;
}) {
  const { t } = useTranslation("citizen_form");
  const insets = useSafeAreaInsets();

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
          // todo: add language support
          const selectedOptionText = question.options.find((o) => o.id === selectedOptionId)?.text
            .EN;

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
          const selectedOptionsTexts = question.options
            .filter((o) => selectedOptionIds.includes(o.id))
            .map((o) => o.text.EN);

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

  return (
    <Sheet
      modal
      open
      zIndex={100_001}
      snapPoints={[90]}
      dismissOnSnapToBottom={!isPending}
      onOpenChange={(open: boolean) => {
        if (!open) {
          setIsReviewSheetOpen(false);
        }
      }}
    >
      <Sheet.Overlay />
      <Sheet.Frame>
        <Icon paddingVertical="$md" alignSelf="center" icon="dragHandle" />
        <YStack flex={1} marginBottom={insets.bottom + 16}>
          {isPending ? (
            <LoadingScreen />
          ) : (
            <Sheet.ScrollView
              contentContainerStyle={{ flexGrow: 1, paddingHorizontal: 24 }}
              bounces={false}
              showsVerticalScrollIndicator={false}
            >
              <Typography preset="heading">{t("review.heading")}</Typography>
              {displayedQuestions.map((question) => (
                <YStack key={question.id} marginTop="$md">
                  <Typography fontWeight="500">{question.text.EN}</Typography>
                  {mappedAnswers && mappedAnswers[question.id] && (
                    <Typography marginTop="$xs" color="$gray5">
                      {getAnswerDisplay(mappedAnswers[question.id] as ApiFormAnswer)}
                    </Typography>
                  )}
                </YStack>
              ))}
            </Sheet.ScrollView>
          )}

          <YStack paddingHorizontal="$lg" paddingTop="$md">
            <Button onPress={onSubmit} disabled={isPending}>
              {t("review.send")}
            </Button>
          </YStack>
        </YStack>
      </Sheet.Frame>
    </Sheet>
  );
}
