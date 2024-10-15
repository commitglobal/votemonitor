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
import i18n from "../common/config/i18n";
import * as Crypto from "expo-crypto";
import { usePostCitizenFormMutation } from "../services/mutations/citizen/post-citizen-form.mutation";
import { useCitizenUserData } from "../contexts/citizen-user/CitizenUserContext.provider";
import { useRouter } from "expo-router";
import Toast from "react-native-toast-message";

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
  selectedLocationId,
}: {
  currentForm: FormAPIModel | undefined;
  answers: Record<string, ApiFormAnswer | undefined> | undefined;
  questions: ApiFormQuestion[] | undefined;
  setIsReviewSheetOpen: Dispatch<SetStateAction<boolean>>;
  selectedLocationId: string;
}) {
  const { t } = useTranslation("citizen_form");
  const insets = useSafeAreaInsets();
  const router = useRouter();

  const { mutate: postCitizenForm, isPending } = usePostCitizenFormMutation();
  const { selectedElectionRound } = useCitizenUserData();
  const currentLanguage = useMemo(
    () =>
      i18n.language.toLocaleUpperCase() ||
      currentForm?.defaultLanguage ||
      currentForm?.languages[0] ||
      i18n.languages[0],
    [currentForm],
  );
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
            currentLanguage
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
              const optionText = option.text.EN || "";
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

  const handleSubmit = () => {
    if (!currentForm || !selectedElectionRound || !answers) {
      console.log("⛔️ Missing data for sending review citizen form. ⛔️");
      return;
    }

    postCitizenForm(
      {
        electionRoundId: selectedElectionRound,
        citizenReportId: Crypto.randomUUID(),
        formId: currentForm.id,
        locationId: selectedLocationId,
        answers: Object.values(answers).filter(Boolean) as ApiFormAnswer[],
      },
      {
        onSuccess: (response) => {
          console.log("🔵 [CitizenForm] form submitted successfully, redirect to success page");
          router.replace(`/citizen/main/form/success?submissionId=${response.id}`);
        },
        onError: (error) => {
          console.log("🔴 [CitizenForm] error submitting form", error);
          // close review modal and display error toast
          setIsReviewSheetOpen(false);
          return Toast.show({
            type: "error",
            text2: t("error"),
            text2Style: { textAlign: "center" },
          });
        },
      },
    );
  };

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
                      {getAnswerDisplay(mappedAnswers[question.id] as ApiFormAnswer, true)}
                    </Typography>
                  )}
                </YStack>
              ))}
            </Sheet.ScrollView>
          )}

          <YStack paddingHorizontal="$lg" paddingTop="$md">
            <Button onPress={handleSubmit} disabled={isPending}>
              {t("review.send")}
            </Button>
          </YStack>
        </YStack>
      </Sheet.Frame>
    </Sheet>
  );
}
