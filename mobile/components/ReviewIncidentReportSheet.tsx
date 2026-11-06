import React, { useMemo } from "react";
import { useTranslation } from "react-i18next";
import { useSafeAreaInsets } from "react-native-safe-area-context";
import { Separator, Sheet, Spinner, YStack } from "tamagui";
import { getAnswerDisplay, mapAnswersForDisplay } from "../common/utils/answers";
import { useDateTimeFormat } from "../contexts/date-time-format/DateTimeFormatContext.provider";
import { AttachmentData } from "../services/api/add-attachment.api";
import { FormAPIModel } from "../services/definitions.api";
import { ApiFormAnswer } from "../services/interfaces/answer.type";
import Button from "./Button";
import { Icon } from "./Icon";
import MediaLoading from "./MediaLoading";
import { Typography } from "./Typography";

interface ReviewIncidentReportSheetProps {
  currentForm: FormAPIModel;
  language: string;
  locationLabel: string;
  answers: Record<string, ApiFormAnswer | undefined> | undefined;
  attachments: Record<string, AttachmentData[]>;
  notes: Record<string, { id: string; text: string }[]>;
  isSubmitting: boolean;
  isUploading: boolean;
  uploadProgress: string;
  onSubmit: () => void;
  onAbortUpload: () => void;
  onClose: () => void;
}

const ReviewIncidentReportSheet = ({
  currentForm,
  language,
  locationLabel,
  answers,
  attachments,
  notes,
  isSubmitting,
  isUploading,
  uploadProgress,
  onSubmit,
  onAbortUpload,
  onClose,
}: ReviewIncidentReportSheetProps) => {
  const { t } = useTranslation(["incident_report", "polling_station_form_wizard"]);
  const dateTimeFormat = useDateTimeFormat();
  const insets = useSafeAreaInsets();

  const mappedAnswers = useMemo(
    () => mapAnswersForDisplay(answers, currentForm.questions, language),
    [answers, currentForm.questions, language],
  );

  // only the questions with an answer, notes or media (with skip logic, some were never shown)
  const answeredQuestions = useMemo(
    () =>
      currentForm.questions.filter(
        (question) =>
          mappedAnswers[question.id] ||
          notes[question.id]?.length ||
          attachments[question.id]?.length,
      ),
    [currentForm.questions, mappedAnswers, notes, attachments],
  );

  const isBusy = isSubmitting || isUploading;

  return (
    <Sheet
      modal
      open
      zIndex={100_001}
      snapPoints={isUploading ? [25] : [90]}
      dismissOnSnapToBottom={!isBusy}
      dismissOnOverlayPress={!isBusy}
      onOpenChange={(open: boolean) => {
        if (!open) {
          onClose();
        }
      }}
    >
      <Sheet.Overlay />
      <Sheet.Frame>
        {isUploading ? (
          <YStack flex={1} justifyContent="center" alignItems="center">
            <MediaLoading
              progress={uploadProgress}
              isUploading={isUploading}
              uploadedAttachments={
                Object.values(attachments)
                  .flat()
                  .filter((attachment) => attachment.uploaded).length
              }
              onAbortUpload={onAbortUpload}
              confirmAbort
            />
          </YStack>
        ) : (
          <>
            <Icon paddingVertical="$md" alignSelf="center" icon="dragHandle" />
            <YStack flex={1} marginBottom={insets.bottom + 16}>
              {isSubmitting ? (
                <YStack flex={1} alignItems="center" gap="$lg">
                  <Spinner size="large" color="$purple5" />
                  <Typography preset="body2" textAlign="center">
                    {t("review.sending_report")}
                  </Typography>
                </YStack>
              ) : (
                <Sheet.ScrollView
                  contentContainerStyle={{ flexGrow: 1, paddingHorizontal: 24 }}
                  bounces={false}
                  showsVerticalScrollIndicator={false}
                >
                  <Typography preset="heading">{t("review.heading")}</Typography>

                  <YStack marginTop="$md">
                    <Typography fontWeight="500">{t("location.label")}</Typography>
                    <Typography marginTop="$xs" color="$gray5">
                      {locationLabel}
                    </Typography>
                    <Separator marginTop="$xs" />
                  </YStack>

                  {answeredQuestions.map((question) => (
                    <YStack key={question.id} marginTop="$md">
                      <Typography fontWeight="500">{question.text[language]}</Typography>
                      {mappedAnswers[question.id] ? (
                        <Typography marginTop="$xs" color="$gray5">
                          {getAnswerDisplay(mappedAnswers[question.id], true, dateTimeFormat)}
                        </Typography>
                      ) : (
                        false
                      )}
                      {notes[question.id]?.map((note) => (
                        <Typography key={note.id} marginTop="$xs" fontStyle="italic">
                          {`${t("notes.heading", { ns: "polling_station_form_wizard" })}: ${note.text}`}
                        </Typography>
                      ))}
                      {attachments[question.id]?.length ? (
                        <Typography fontWeight="500" paddingTop="$lg">
                          {t("attachments.heading")}: {attachments[question.id].length}
                        </Typography>
                      ) : (
                        false
                      )}
                      <Separator marginTop="$xs" />
                    </YStack>
                  ))}
                </Sheet.ScrollView>
              )}

              <YStack paddingHorizontal="$lg" paddingTop="$md">
                <Button onPress={onSubmit} disabled={isBusy}>
                  {t("review.send")}
                </Button>
              </YStack>
            </YStack>
          </>
        )}
      </Sheet.Frame>
    </Sheet>
  );
};

export default ReviewIncidentReportSheet;
