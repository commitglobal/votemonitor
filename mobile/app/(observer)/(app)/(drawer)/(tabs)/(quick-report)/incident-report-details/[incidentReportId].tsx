import { router, useLocalSearchParams } from "expo-router";
import { useEffect, useMemo, useRef, useState } from "react";
import { useTranslation } from "react-i18next";
import { RefreshControl, ScrollView as NativeScrollView } from "react-native";
import { ScrollView, Separator, YStack } from "tamagui";
import UnansweredWarning from "../../../../../../../components/UnansweredWarning";
import { getUnansweredQuestions } from "../../../../../../../services/unanswered-questions";
import { getAnswerDisplay, mapAnswersForDisplay } from "../../../../../../../common/utils/answers";
import { getFormLanguagePreference } from "../../../../../../../common/language.preferences";
import Card from "../../../../../../../components/Card";
import Header from "../../../../../../../components/Header";
import { Icon } from "../../../../../../../components/Icon";
import { MediaDialog } from "../../../../../../../components/MediaDialog";
import { Screen } from "../../../../../../../components/Screen";
import { Typography } from "../../../../../../../components/Typography";
import { useDateTimeFormat } from "../../../../../../../contexts/date-time-format/DateTimeFormatContext.provider";
import { useUserData } from "../../../../../../../contexts/user/UserContext.provider";
import {
  getFormDisplayName,
  getFormLanguage,
  getIncidentReportLocationLabel,
} from "../../../../../../../helpers/incidentReport";
import { AttachmentMimeType } from "../../../../../../../services/api/get-attachments.api";
import { IncidentReportAttachmentAPIResponse } from "../../../../../../../services/api/incident-report/incident-report.api";
import { useFormById } from "../../../../../../../services/queries/forms.query";
import { useIncidentReportById } from "../../../../../../../services/queries/incident-reports.query";

const IncidentReportDetails = () => {
  // questionId: the question to scroll to (the unanswered warning on a sent report leads there)
  const { incidentReportId, questionId: questionIdToShow } = useLocalSearchParams<{
    incidentReportId: string;
    questionId?: string;
  }>();
  const scrollRef = useRef<NativeScrollView>(null);
  // where the question list and the question to show are (layout events can come in either order)
  const questionsTopRef = useRef<number | undefined>(undefined);
  const questionToShowYRef = useRef<number | undefined>(undefined);
  const hasScrolledRef = useRef(false);
  const { t } = useTranslation([
    "incident_report",
    "report_details",
    "polling_station_form_wizard",
    "common",
  ]);
  const dateTimeFormat = useDateTimeFormat();
  const { activeElectionRound, visits } = useUserData();

  const [previewAttachment, setPreviewAttachment] =
    useState<IncidentReportAttachmentAPIResponse | null>(null);

  const {
    data: incidentReport,
    isLoading,
    error,
    refetch,
    isRefetching,
  } = useIncidentReportById(activeElectionRound?.id, incidentReportId ?? "");
  const { data: form } = useFormById(activeElectionRound?.id, incidentReport?.formId ?? "");

  // the language the observer picked for this form, if any
  const [preferredLanguage, setPreferredLanguage] = useState<string | null>(null);
  useEffect(() => {
    if (form?.id) {
      getFormLanguagePreference({ formId: form.id }).then(setPreferredLanguage);
    }
  }, [form?.id]);

  const language = useMemo(
    () =>
      preferredLanguage && form?.languages.includes(preferredLanguage)
        ? preferredLanguage
        : getFormLanguage(form),
    [form, preferredLanguage],
  );

  const answersByQuestionId = useMemo(
    () =>
      mapAnswersForDisplay(
        Object.fromEntries((incidentReport?.answers || []).map((a) => [a.questionId, a])),
        form?.questions,
        language,
      ),
    [incidentReport?.answers, form?.questions, language],
  );

  const unansweredQuestionIds = useMemo(
    () =>
      new Set(
        getUnansweredQuestions(
          form?.questions,
          Object.fromEntries((incidentReport?.answers || []).map((a) => [a.questionId, a])),
        ).map((question) => question.id),
      ),
    [form?.questions, incidentReport?.answers],
  );

  // the answered questions (or with notes), and the unanswered ones with a warning, in form order
  const shownQuestions = useMemo(
    () =>
      form?.questions.filter(
        (question) =>
          answersByQuestionId[question.id] ||
          unansweredQuestionIds.has(question.id) ||
          incidentReport?.notes.some((note) => note.questionId === question.id),
      ) || [],
    [form?.questions, answersByQuestionId, unansweredQuestionIds, incidentReport?.notes],
  );

  const scrollToQuestionWhenLaidOut = () => {
    const top = questionsTopRef.current;
    const y = questionToShowYRef.current;
    if (hasScrolledRef.current || top === undefined || y === undefined) return;
    hasScrolledRef.current = true;
    scrollRef.current?.scrollTo({ y: top + y, animated: true });
  };

  const onQuestionLayout = (questionId: string, y: number) => {
    if (questionId !== questionIdToShow) return;
    questionToShowYRef.current = y;
    scrollToQuestionWhenLaidOut();
  };

  const attachments = incidentReport?.attachments || [];

  if (isLoading) {
    return <Typography>{t("loading", { ns: "common" })}</Typography>;
  }

  return (
    <Screen preset="fixed" contentContainerStyle={{ flexGrow: 1 }} backgroundColor="white">
      <Header
        title={getFormDisplayName(form, language) || t("title")}
        leftIcon={<Icon icon="chevronLeft" color="white" />}
        onLeftPress={() => router.back()}
      />

      <YStack flex={1}>
        <ScrollView
          ref={scrollRef as any}
          contentContainerStyle={{
            paddingVertical: 32,
            gap: 32,
            flexGrow: 1,
            paddingHorizontal: 16,
          }}
          showsVerticalScrollIndicator={false}
          refreshControl={<RefreshControl refreshing={isRefetching} onRefresh={refetch} />}
        >
          {error || !incidentReport ? (
            <Typography>{t("error", { ns: "report_details" })}</Typography>
          ) : (
            <>
              <YStack gap="$xs">
                <Typography fontWeight="500">{t("location.label")}</Typography>
                <Typography preset="body1" color="$gray8">
                  {getIncidentReportLocationLabel(incidentReport, visits)}
                </Typography>
              </YStack>

              <YStack
                gap="$md"
                onLayout={(event) => {
                  questionsTopRef.current = event.nativeEvent.layout.y;
                  scrollToQuestionWhenLaidOut();
                }}
              >
                {shownQuestions.map((question) => (
                  <YStack
                    key={question.id}
                    gap="$xs"
                    onLayout={(event) => onQuestionLayout(question.id, event.nativeEvent.layout.y)}
                  >
                    <Typography fontWeight="500">{question.text[language]}</Typography>
                    {unansweredQuestionIds.has(question.id) && (
                      <UnansweredWarning
                        label={t("questions.not_answered", { ns: "form_overview" })}
                      />
                    )}
                    {answersByQuestionId[question.id] ? (
                      <Typography color="$gray8">
                        {getAnswerDisplay(answersByQuestionId[question.id], true, dateTimeFormat)}
                      </Typography>
                    ) : (
                      false
                    )}
                    {incidentReport.notes
                      .filter((note) => note.questionId === question.id)
                      .map((note, index) => (
                        <Typography key={index} fontStyle="italic">
                          {`${t("notes.heading", { ns: "polling_station_form_wizard" })}: ${note.text}`}
                        </Typography>
                      ))}
                    <Separator />
                  </YStack>
                ))}
              </YStack>

              {attachments.length === 0 ? (
                <Typography fontWeight="500">{t("no_files", { ns: "report_details" })}</Typography>
              ) : (
                <YStack gap={16}>
                  <Typography fontWeight="500" color="$gray10">
                    {t("uploaded_media", { ns: "report_details" })}
                  </Typography>
                  {attachments.map((attachment, index) => (
                    <Card
                      key={attachment.uploadedFileName ?? index}
                      onPress={() => setPreviewAttachment(attachment)}
                    >
                      <Typography preset="body1" fontWeight="700">
                        {attachment.fileName}
                      </Typography>
                    </Card>
                  ))}
                </YStack>
              )}
            </>
          )}

          {previewAttachment && previewAttachment.mimeType === AttachmentMimeType.IMG && (
            <MediaDialog
              media={{ type: previewAttachment.mimeType, src: previewAttachment.presignedUrl }}
              onClose={() => setPreviewAttachment(null)}
            />
          )}
        </ScrollView>
      </YStack>
    </Screen>
  );
};

export default IncidentReportDetails;
