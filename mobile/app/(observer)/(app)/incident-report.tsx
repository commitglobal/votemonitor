import * as Sentry from "@sentry/react-native";
import { onlineManager, useQueryClient } from "@tanstack/react-query";
import * as Crypto from "expo-crypto";
import * as DocumentPicker from "expo-document-picker";
import { router, useLocalSearchParams } from "expo-router";
import { TFunction } from "i18next";
import React, { useEffect, useMemo, useRef, useState } from "react";
import { Controller, useForm } from "react-hook-form";
import { useTranslation } from "react-i18next";
import { BackHandler, Keyboard, ViewStyle } from "react-native";
import Toast from "react-native-toast-message";
import { Spinner, XStack, YStack } from "tamagui";
import { PollingStationVisitVM } from "../../../common/models/polling-station.model";
import {
  MULTIPART_FILE_UPLOAD_SIZE,
  MUTATION_SCOPE_DO_NOT_HYDRATE,
} from "../../../common/constants";
import AddAttachment from "../../../components/AddAttachment";
import Card from "../../../components/Card";
import FormElement from "../../../components/FormInputs/FormElement";
import FormInput from "../../../components/FormInputs/FormInput";
import Header from "../../../components/Header";
import { Icon } from "../../../components/Icon";
import LinearProgress from "../../../components/LinearProgress";
import { MediaDialog } from "../../../components/MediaDialog";
import MediaLoading from "../../../components/MediaLoading";
import NoteForm from "../../../components/NoteForm";
import OptionsSheet from "../../../components/OptionsSheet";
import QuestionForm from "../../../components/QuestionForm";
import ReviewIncidentReportSheet from "../../../components/ReviewIncidentReportSheet";
import { Screen } from "../../../components/Screen";
import { ScreenScrollView } from "../../../components/ScreenScrollView";
import Select from "../../../components/Select";
import { Typography } from "../../../components/Typography";
import WarningDialog from "../../../components/WarningDialog";
import WizzardControls from "../../../components/WizzardControls";
import { useNetInfoContext } from "../../../contexts/net-info-banner/NetInfoContext";
import { useUserData } from "../../../contexts/user/UserContext.provider";
import { FileMetadata, useCamera } from "../../../hooks/useCamera";
import { AttachmentData } from "../../../services/api/add-attachment.api";
import { AttachmentMimeType } from "../../../services/api/get-attachments.api";
import {
  IncidentReportAttachmentAPIResponse,
  IncidentReportLocationType,
} from "../../../services/api/incident-report/incident-report.api";
import {
  mapFormSubmissionDataToAPIFormSubmissionAnswer,
  removeAnswersForHiddenQuestions,
  setFormDefaultValues,
  shouldDisplayQuestion,
  withAnswerTimestamps,
} from "../../../services/form.parser";
import { ApiFormAnswer } from "../../../services/interfaces/answer.type";
import { ApiFormQuestion } from "../../../services/interfaces/question.type";
import {
  removeMutationByScopeId,
  useUploadS3ChunkMutation,
} from "../../../services/mutations/attachments/add-attachment.mutation";
import {
  useUploadAttachmentIncidentReportAbortMutation,
  useUploadAttachmentIncidentReportCompleteMutation,
  useUploadAttachmentIncidentReportMutation,
} from "../../../services/mutations/incident-report/add-attachment-incident-report.mutation";
import {
  useAddIncidentReport,
  useAddIncidentReportNote,
} from "../../../services/mutations/incident-report/add-incident-report.mutation";
import { useFormById } from "../../../services/queries/forms.query";
import {
  getFormDisplayName,
  getFormLanguage,
  getIncidentReportLocationLabel,
} from "../../../helpers/incidentReport";
import { IncidentReportKeys } from "../../../services/queries/incident-reports.query";

type LocalNote = { id: string; text: string };

type LocationFormType = {
  // a visited polling station id, or one of the OtherPollingStation / NotRelatedToAPollingStation location types
  location: string;
  locationDescription: string;
};

const mapVisitsToLocationOptions = (visits: PollingStationVisitVM[] = [], t: TFunction) => [
  ...visits.map((visit) => ({
    id: visit.pollingStationId,
    value: visit.pollingStationId,
    label: `${visit.number} - ${visit.address}`,
  })),
  {
    id: "other",
    value: IncidentReportLocationType.OtherPollingStation,
    label: t("form.polling_station_id.options.other", { ns: "report_new_issue" }),
  },
  {
    id: "not_related_to_polling_station",
    value: IncidentReportLocationType.NotRelatedToAPollingStation,
    label: t("form.polling_station_id.options.not_related", { ns: "report_new_issue" }),
  },
];

const isLocationType = (location: string) =>
  location === IncidentReportLocationType.OtherPollingStation ||
  location === IncidentReportLocationType.NotRelatedToAPollingStation;

const IncidentReport = () => {
  const { formId, language: languageParam } = useLocalSearchParams<{
    formId: string;
    language?: string;
  }>();
  const { t } = useTranslation([
    "incident_report",
    "report_new_issue",
    "polling_station_form_wizard",
  ]);
  const queryClient = useQueryClient();
  const cancelRef = useRef(false);

  const { visits, activeElectionRound } = useUserData();
  const { isOnline } = useNetInfoContext();
  const { uploadCameraOrMedia } = useCamera();

  const { data: currentForm, isLoading: isLoadingForm } = useFormById(
    activeElectionRound?.id,
    formId,
  );

  // the report id is generated once, so retrying a submission upserts the same report
  const [incidentReportId] = useState(Crypto.randomUUID());
  const [step, setStep] = useState<"location" | "questions">("location");
  const [questionId, setQuestionId] = useState<string | undefined>();
  const [answers, setAnswers] = useState<Record<string, ApiFormAnswer | undefined>>({});
  const [attachments, setAttachments] = useState<Record<string, AttachmentData[]>>({});
  // notes are kept here until the report is sent
  const [notes, setNotes] = useState<Record<string, LocalNote[]>>({});
  // when set, the options sheet shows the note form (a new note, or editing the one with noteId)
  const [noteEditor, setNoteEditor] = useState<{ noteId?: string; text?: string } | null>(null);
  const [selectedNoteToDelete, setSelectedNoteToDelete] = useState<string | null>(null);

  const [isReviewSheetOpen, setIsReviewSheetOpen] = useState(false);
  const [showWarningDialog, setShowWarningDialog] = useState(false);
  const [isOptionsSheetOpen, setIsOptionsSheetOpen] = useState(false);
  const [isPreparingFile, setIsPreparingFile] = useState(false);
  const [isUploading, setIsUploading] = useState(false);
  const [uploadProgress, setUploadProgress] = useState("");
  const [selectedAttachmentToDelete, setSelectedAttachmentToDelete] = useState<string | null>(null);
  const [previewAttachment, setPreviewAttachment] = useState<AttachmentData | null>(null);

  const {
    mutate: addIncidentReport,
    mutateAsync: addIncidentReportAsync,
    isPending,
  } = useAddIncidentReport(incidentReportId);
  const { mutate: addIncidentReportNote } = useAddIncidentReportNote(incidentReportId);
  const { mutateAsync: initAttachment } = useUploadAttachmentIncidentReportMutation();
  const { mutateAsync: completeAttachment } = useUploadAttachmentIncidentReportCompleteMutation();
  const { mutateAsync: abortAttachment } = useUploadAttachmentIncidentReportAbortMutation();
  const { mutateAsync: uploadS3Chunk } = useUploadS3ChunkMutation();

  const locationOptions = useMemo(() => mapVisitsToLocationOptions(visits, t), [visits, t]);

  const {
    control: locationControl,
    handleSubmit: handleLocationSubmit,
    watch: watchLocation,
    getValues: getLocationValues,
    formState: { errors: locationErrors, isDirty: isLocationDirty },
  } = useForm<LocationFormType>({
    defaultValues: { location: "", locationDescription: "" },
  });
  const locationWatch = watchLocation("location");

  // the language picked when choosing the form, if the form still has it
  const language = useMemo(
    () =>
      languageParam && currentForm?.languages.includes(languageParam)
        ? languageParam
        : getFormLanguage(currentForm),
    [currentForm, languageParam],
  );
  const formTitle = useMemo(
    () => (currentForm ? `${getFormDisplayName(currentForm, language)} (${language})` : ""),
    [currentForm, language],
  );

  const displayedQuestions: ApiFormQuestion[] = useMemo(
    () => currentForm?.questions?.filter((q) => shouldDisplayQuestion(q, answers)) || [],
    [currentForm, answers],
  );

  const activeQuestion = useMemo(() => {
    const question = currentForm?.questions.find((q) => q.id === questionId);
    if (!currentForm || !question) return undefined;
    return {
      indexInAllQuestions: currentForm.questions.findIndex((q) => q.id === questionId),
      indexInDisplayedQuestions: displayedQuestions.findIndex((q) => q.id === questionId),
      question,
    };
  }, [currentForm, displayedQuestions, questionId]);

  const {
    control: questionControl,
    handleSubmit: handleQuestionSubmit,
    reset: resetQuestionForm,
    formState: { isDirty: isQuestionDirty },
  } = useForm<any>();

  // one screen walks through all the questions, so load the saved answer whenever the question changes
  useEffect(() => {
    if (questionId) {
      resetQuestionForm(setFormDefaultValues(questionId, answers[questionId]));
    }
  }, [questionId]);

  const hasProgress =
    isLocationDirty ||
    isQuestionDirty ||
    Object.keys(answers).length > 0 ||
    Object.values(attachments).some((a) => a.length > 0) ||
    Object.values(notes).some((n) => n.length > 0);

  const handleGoBack = () => {
    if (hasProgress) {
      Keyboard.dismiss();
      setShowWarningDialog(true);
    } else {
      router.back();
    }
  };

  useEffect(() => {
    const backHandler = BackHandler.addEventListener("hardwareBackPress", () => {
      handleGoBack();
      return true;
    });
    return () => backHandler.remove();
  }, [hasProgress]);

  const getLocation = () => {
    const { location, locationDescription } = getLocationValues();
    return isLocationType(location)
      ? {
          locationType: location as IncidentReportLocationType,
          locationDescription,
        }
      : { locationType: IncidentReportLocationType.PollingStation, pollingStationId: location };
  };

  const findNextQuestion = (
    index: number,
    updatedAnswers: Record<string, ApiFormAnswer | undefined>,
  ): ApiFormQuestion | null => {
    if (!currentForm || index + 1 >= currentForm.questions.length) return null;
    const nextQuestion = currentForm.questions[index + 1];
    if (shouldDisplayQuestion(nextQuestion, updatedAnswers)) return nextQuestion;
    return findNextQuestion(index + 1, updatedAnswers);
  };

  const findPreviousQuestion = (index: number): ApiFormQuestion | null => {
    if (!currentForm || index - 1 < 0) return null;
    const prevQuestion = currentForm.questions[index - 1];
    if (shouldDisplayQuestion(prevQuestion, answers)) return prevQuestion;
    return findPreviousQuestion(index - 1);
  };

  const onSubmitLocation = () => {
    const firstQuestion = findNextQuestion(-1, answers);
    if (!firstQuestion) {
      // a form without questions: nothing to fill in, go straight to the review
      Keyboard.dismiss();
      setIsReviewSheetOpen(true);
      return;
    }
    setQuestionId((current) => current ?? firstQuestion.id);
    setStep("questions");
  };

  const goToPrevious = () => {
    if (!activeQuestion) return;
    const prevQuestion = findPreviousQuestion(activeQuestion.indexInAllQuestions);
    if (prevQuestion) {
      setQuestionId(prevQuestion.id);
    } else {
      setStep("location");
    }
  };

  const onSubmitAnswer = (formValues: any) => {
    if (!activeQuestion) return;
    const id = activeQuestion.question.id;

    const updatedAnswer = withAnswerTimestamps(
      mapFormSubmissionDataToAPIFormSubmissionAnswer(
        id,
        activeQuestion.question.$questionType,
        formValues[id],
      ),
      answers[id],
    );

    // drop the answers of dependent questions that are no longer displayed
    const updatedAnswers = removeAnswersForHiddenQuestions(currentForm?.questions, {
      ...answers,
      [id]: updatedAnswer,
    }) as Record<string, ApiFormAnswer | undefined>;
    setAnswers(updatedAnswers);

    const nextQuestion = findNextQuestion(activeQuestion.indexInAllQuestions, updatedAnswers);
    if (nextQuestion) {
      setQuestionId(nextQuestion.id);
    } else {
      Keyboard.dismiss();
      setIsReviewSheetOpen(true);
    }
  };

  const uploadAttachment = async (
    attachment: AttachmentData,
    questionId: string,
    uploadedPartsNo: number,
    totalParts: number,
  ) => {
    if (!activeElectionRound || !currentForm) return;

    const { uploadId, uploadUrls } = await initAttachment({
      id: attachment.id,
      electionRoundId: activeElectionRound.id,
      incidentReportId,
      formId: currentForm.id,
      questionId,
      fileName: attachment.fileMetadata.name,
      contentType: attachment.fileMetadata.type,
      numberOfUploadParts: Math.ceil(attachment.fileMetadata.size! / MULTIPART_FILE_UPLOAD_SIZE),
    });

    try {
      let etags: Record<number, string> = {};
      for (const [index, url] of Object.values(uploadUrls).entries()) {
        if (cancelRef.current) {
          throw new Error("Upload aborted");
        }
        const data = await uploadS3Chunk({
          url,
          filePath: attachment.fileMetadata.uri,
          partIndex: index,
          partSize: MULTIPART_FILE_UPLOAD_SIZE,
        });
        setUploadProgress(
          `${t("upload.progress", { ns: "report_new_issue" })} ${Math.ceil(((uploadedPartsNo + index) / totalParts) * 100)}%`,
        );
        etags = { ...etags, [index + 1]: data.ETag };
      }

      await completeAttachment({
        uploadId,
        etags,
        electionRoundId: activeElectionRound.id,
        id: attachment.id,
        incidentReportId,
      });

      setAttachments((current) => ({
        ...current,
        [questionId]: current[questionId].map((a) =>
          a.id === attachment.id ? { ...a, uploaded: true } : a,
        ),
      }));
    } catch (err) {
      Sentry.captureException(err, { data: { incidentReportId, questionId } });
      setUploadProgress(t("upload.aborted", { ns: "report_new_issue" }));
      await abortAttachment({
        id: attachment.id,
        uploadId,
        electionRoundId: activeElectionRound.id,
        incidentReportId,
      });
    }
  };

  const uploadAttachments = async () => {
    // only the attachments of questions that are still displayed
    const pending = displayedQuestions.flatMap((question) =>
      (attachments[question.id] || [])
        .filter((attachment) => !attachment.uploaded)
        .map((attachment) => ({ attachment, questionId: question.id })),
    );
    if (!pending.length) return;

    setIsUploading(true);
    cancelRef.current = false;
    setUploadProgress(t("upload.starting", { ns: "report_new_issue" }));

    const totalParts = pending.reduce(
      (acc, { attachment }) =>
        acc + Math.ceil(attachment.fileMetadata.size! / MULTIPART_FILE_UPLOAD_SIZE),
      0,
    );
    let uploadedPartsNo = 0;

    for (const { attachment, questionId } of pending) {
      if (cancelRef.current) break;
      try {
        await uploadAttachment(attachment, questionId, uploadedPartsNo, totalParts);
      } catch (err) {
        Sentry.captureException(err);
      }
      uploadedPartsNo += Math.ceil(attachment.fileMetadata.size! / MULTIPART_FILE_UPLOAD_SIZE);
    }

    setUploadProgress(t("upload.completed", { ns: "report_new_issue" }));
    setIsUploading(false);
    queryClient.invalidateQueries({
      queryKey: IncidentReportKeys.byElectionRound(activeElectionRound?.id),
    });
  };

  const onSubmitReport = async () => {
    if (!activeElectionRound || !currentForm) return;

    const optimisticAttachments: IncidentReportAttachmentAPIResponse[] = displayedQuestions.flatMap(
      (question) =>
        (attachments[question.id] || []).map((attachment) => ({
          submissionId: incidentReportId,
          questionId: question.id,
          fileName: attachment.fileMetadata.name,
          mimeType: attachment.fileMetadata.type,
          presignedUrl: attachment.fileMetadata.uri,
          urlValidityInSeconds: 0,
        })),
    );

    const payload = {
      id: incidentReportId,
      electionRoundId: activeElectionRound.id,
      formId: currentForm.id,
      ...getLocation(),
      answers: Object.values(answers).filter(Boolean) as ApiFormAnswer[],
      isCompleted: true,
      lastUpdatedAt: new Date().toISOString(),
      attachments: optimisticAttachments,
      notes: displayedQuestions.flatMap((question) =>
        (notes[question.id] || []).map((note) => ({
          submissionId: incidentReportId,
          questionId: question.id,
          text: note.text,
          timeSubmitted: new Date().toISOString(),
        })),
      ),
    };

    // the notes share the report's mutation scope, so they are sent (also after being offline)
    // only once the report was created
    const sendNotes = () => {
      displayedQuestions.forEach((question) =>
        (notes[question.id] || []).forEach((note) =>
          addIncidentReportNote({
            id: note.id,
            electionRoundId: activeElectionRound.id,
            incidentReportId,
            formId: currentForm.id,
            questionId: question.id,
            text: note.text,
          }),
        ),
      );
    };

    const onError = () => {
      Toast.show({ type: "error", text2: t("error") });
    };

    if (!optimisticAttachments.length) {
      // like quick reports: without attachments the report is queued while offline and sent later
      addIncidentReport(payload, {
        onError,
        onSuccess: () => {
          setIsReviewSheetOpen(false);
          router.back();
        },
      });
      sendNotes();
      if (!onlineManager.isOnline()) {
        setIsReviewSheetOpen(false);
        router.back();
      }
      return;
    }

    // attachments reference the incident report, so it has to exist on the server before uploading them
    if (!onlineManager.isOnline()) {
      Toast.show({
        type: "error",
        text2: t("upload.offline", { ns: "report_new_issue" }),
        visibilityTime: 5000,
        text2Style: { textAlign: "center" },
      });
      return;
    }

    try {
      await addIncidentReportAsync(payload);
    } catch {
      onError();
      return;
    }
    sendNotes();

    await uploadAttachments();

    if (!cancelRef.current) {
      setIsReviewSheetOpen(false);
      router.back();
    }
  };

  const onAbortUpload = () => {
    // the report itself is already sent, only the remaining attachments are dropped
    cancelRef.current = true;
    setIsUploading(false);
    setUploadProgress("");
    removeMutationByScopeId(queryClient, MUTATION_SCOPE_DO_NOT_HYDRATE);
    setIsReviewSheetOpen(false);
    router.back();
  };

  const addLocalAttachment = (fileMetadata: FileMetadata) => {
    if (!questionId) return;
    setAttachments((current) => ({
      ...current,
      [questionId]: [
        ...(current[questionId] || []),
        { fileMetadata, id: Crypto.randomUUID(), uploaded: false },
      ],
    }));
  };

  const removeLocalAttachment = (id: string) => {
    if (!questionId) return;
    setAttachments((current) => ({
      ...current,
      [questionId]: (current[questionId] || []).filter((attachment) => attachment.id !== id),
    }));
  };

  const handleCameraUpload = async (type: "library" | "cameraPhoto") => {
    setIsPreparingFile(true);
    setUploadProgress(t("upload.preparing", { ns: "report_new_issue" }));
    const cameraResult = await uploadCameraOrMedia(type, (progress: number) =>
      setUploadProgress(
        `${t("upload.compressing", { ns: "report_new_issue" })} ${Math.ceil(progress * 100)}%`,
      ),
    );
    if (cameraResult) {
      setIsOptionsSheetOpen(false);
      addLocalAttachment(cameraResult);
    }
    setIsPreparingFile(false);
  };

  const handleUploadAudio = async () => {
    const doc = await DocumentPicker.getDocumentAsync({ type: "audio/*", multiple: false });
    const file = doc?.assets?.[0];
    if (file) {
      setIsOptionsSheetOpen(false);
      addLocalAttachment({
        name: file.name,
        type: file.mimeType || "audio/mpeg",
        uri: file.uri,
        size: file.size || 0,
      });
    }
  };

  const saveLocalNote = (text: string) => {
    if (!questionId || !noteEditor) return;
    const { noteId } = noteEditor;
    setNotes((current) => {
      const questionNotes = current[questionId] || [];
      return {
        ...current,
        [questionId]: noteId
          ? questionNotes.map((note) => (note.id === noteId ? { ...note, text } : note))
          : [...questionNotes, { id: Crypto.randomUUID(), text }],
      };
    });
    Keyboard.dismiss();
    setNoteEditor(null);
    setIsOptionsSheetOpen(false);
  };

  const removeLocalNote = (id: string) => {
    if (!questionId) return;
    setNotes((current) => ({
      ...current,
      [questionId]: (current[questionId] || []).filter((note) => note.id !== id),
    }));
  };

  const openNoteEditor = (note?: LocalNote) => {
    Keyboard.dismiss();
    setNoteEditor(note ? { noteId: note.id, text: note.text } : {});
    setIsOptionsSheetOpen(true);
  };

  const handleOnShowAttachmentSheet = () => {
    Keyboard.dismiss();
    setIsOptionsSheetOpen(true);
  };

  // adding media needs a connection (the files are uploaded when the report is sent)
  const whenOnline = (action: () => void) => () => {
    if (isOnline) {
      action();
    } else {
      Toast.show({
        type: "error",
        text2: t("upload.offline", { ns: "report_new_issue" }),
        visibilityTime: 5000,
        text2Style: { textAlign: "center" },
      });
    }
  };

  if (!formId) {
    return <Typography>Incorrect page params</Typography>;
  }

  if (isLoadingForm) {
    return (
      <YStack justifyContent="center" alignItems="center" flex={1}>
        <Spinner size="large" color="$purple5" />
      </YStack>
    );
  }

  if (!currentForm) {
    return (
      <Screen preset="fixed" contentContainerStyle={$containerStyle}>
        <Header
          title={t("title")}
          leftIcon={<Icon icon="chevronLeft" color="white" />}
          onLeftPress={() => router.back()}
        />
        <Typography padding="$md">{t("form_not_found")}</Typography>
      </Screen>
    );
  }

  const questionAttachments = (questionId && attachments[questionId]) || [];
  const questionNotes = (questionId && notes[questionId]) || [];

  return (
    <Screen
      preset="fixed"
      backgroundColor="white"
      style={$screenStyle}
      contentContainerStyle={$containerStyle}
      footer={
        step === "location" ? (
          <WizzardControls
            isFirstElement
            isLastElement={displayedQuestions.length === 0}
            onActionButtonPress={handleLocationSubmit(onSubmitLocation)}
          />
        ) : (
          <WizzardControls
            isFirstElement={false}
            isLastElement={
              !!activeQuestion &&
              findNextQuestion(activeQuestion.indexInAllQuestions, answers) === null
            }
            onActionButtonPress={handleQuestionSubmit(onSubmitAnswer)}
            onPreviousButtonPress={goToPrevious}
          />
        )
      }
    >
      <Header
        title={formTitle}
        leftIcon={<Icon icon="chevronLeft" color="white" />}
        onLeftPress={handleGoBack}
      />
      <ScreenScrollView contentContainerStyle={{ flexGrow: 1 }} bounces={false}>
        {step === "location" && (
          <YStack paddingVertical="$lg" paddingHorizontal="$md" gap="$lg">
            <Controller
              name="location"
              control={locationControl}
              rules={{
                required: {
                  value: true,
                  message: t("form.polling_station_id.required", { ns: "report_new_issue" }),
                },
              }}
              render={({ field: { onChange, value } }) => (
                <FormElement
                  title={t("form.polling_station_id.label", { ns: "report_new_issue" })}
                  error={locationErrors.location?.message}
                >
                  <Select
                    value={value}
                    options={locationOptions}
                    placeholder={t("form.polling_station_id.placeholder", {
                      ns: "report_new_issue",
                    })}
                    onValueChange={onChange}
                    error={locationErrors.location?.message}
                  />
                </FormElement>
              )}
            />

            {/* the server requires a description for any location other than a visited polling station */}
            {isLocationType(locationWatch) && (
              <Controller
                key={locationWatch}
                name="locationDescription"
                control={locationControl}
                rules={{
                  required: { value: true, message: t("location_description.required") },
                  maxLength: {
                    value: 1024,
                    message: t("location_description.max", { value: 1024 }),
                  },
                }}
                render={({ field: { onChange, value } }) => (
                  <FormInput
                    title={
                      locationWatch === IncidentReportLocationType.OtherPollingStation
                        ? t("location_description.label_other_polling_station")
                        : t("location_description.label_not_related")
                    }
                    type="textarea"
                    placeholder={t("location_description.placeholder")}
                    value={value}
                    onChangeText={onChange}
                    error={locationErrors.locationDescription?.message}
                  />
                )}
              />
            )}
          </YStack>
        )}

        {step === "questions" && activeQuestion && (
          <>
            <YStack gap="$xxs" padding="$md">
              <XStack justifyContent="space-between">
                <Typography>{t("progress_bar_label")}</Typography>
                <Typography>{`${activeQuestion.indexInDisplayedQuestions + 1}/${displayedQuestions.length}`}</Typography>
              </XStack>
              <LinearProgress
                current={activeQuestion.indexInDisplayedQuestions + 1}
                total={displayedQuestions.length}
              />
            </YStack>

            <YStack paddingHorizontal="$md" paddingBottom="$md" justifyContent="center" flex={1}>
              <QuestionForm
                key={activeQuestion.question.id}
                control={questionControl}
                activeQuestion={activeQuestion}
                language={language}
              />

              {questionNotes.length ? (
                <YStack gap="$xxs" paddingTop="$lg">
                  <Typography fontWeight="500">
                    {t("notes.heading", { ns: "polling_station_form_wizard" })}
                  </Typography>
                  {questionNotes.map((note) => (
                    <Card
                      key={note.id}
                      flexDirection="row"
                      justifyContent="space-between"
                      padding="$0"
                      paddingLeft="$md"
                      onPress={() => openNoteEditor(note)}
                    >
                      <Typography paddingVertical="$md" maxWidth="85%" numberOfLines={5}>
                        {note.text}
                      </Typography>
                      <YStack
                        padding="$md"
                        onPress={() => {
                          Keyboard.dismiss();
                          setSelectedNoteToDelete(note.id);
                        }}
                        pressStyle={{ opacity: 0.5 }}
                      >
                        <Icon icon="xCircle" size={24} color="$gray5" />
                      </YStack>
                    </Card>
                  ))}
                </YStack>
              ) : (
                false
              )}

              {questionAttachments.length ? (
                <YStack gap="$xxs" paddingTop="$lg">
                  <Typography fontWeight="500">{t("attachments.heading")}</Typography>
                  {questionAttachments.map((attachment) => (
                    <Card
                      padding="$0"
                      paddingLeft="$md"
                      key={attachment.id}
                      flexDirection="row"
                      justifyContent="space-between"
                      alignItems="center"
                      onPress={() => setPreviewAttachment(attachment)}
                    >
                      <Typography preset="body1" fontWeight="700" maxWidth="85%" numberOfLines={1}>
                        {attachment.fileMetadata.name}
                      </Typography>
                      <YStack
                        padding="$md"
                        onPress={() => {
                          Keyboard.dismiss();
                          setSelectedAttachmentToDelete(attachment.id);
                        }}
                        pressStyle={{ opacity: 0.5 }}
                      >
                        <Icon icon="xCircle" size={24} color="$gray5" />
                      </YStack>
                    </Card>
                  ))}
                </YStack>
              ) : (
                false
              )}

              <AddAttachment
                label={t("attachments.add", { ns: "polling_station_form_wizard" })}
                marginTop="$sm"
                onPress={handleOnShowAttachmentSheet}
              />
            </YStack>
          </>
        )}
      </ScreenScrollView>

      {previewAttachment && previewAttachment.fileMetadata.type === AttachmentMimeType.IMG && (
        <MediaDialog
          media={{
            type: previewAttachment.fileMetadata.type,
            src: previewAttachment.fileMetadata.uri,
          }}
          onClose={() => setPreviewAttachment(null)}
        />
      )}

      {selectedAttachmentToDelete && (
        <WarningDialog
          title={t("attachments.delete.title")}
          description={t("attachments.delete.description")}
          actionBtnText={t("attachments.delete.actions.clear")}
          cancelBtnText={t("attachments.delete.actions.cancel")}
          action={() => {
            removeLocalAttachment(selectedAttachmentToDelete);
            setSelectedAttachmentToDelete(null);
          }}
          onCancel={() => setSelectedAttachmentToDelete(null)}
        />
      )}

      {selectedNoteToDelete && (
        <WarningDialog
          title={t("notes.delete.title", { ns: "polling_station_form_wizard" })}
          description={t("notes.delete.description", { ns: "polling_station_form_wizard" })}
          actionBtnText={t("notes.delete.actions.delete", { ns: "polling_station_form_wizard" })}
          cancelBtnText={t("notes.delete.actions.cancel", { ns: "polling_station_form_wizard" })}
          action={() => {
            removeLocalNote(selectedNoteToDelete);
            setSelectedNoteToDelete(null);
          }}
          onCancel={() => setSelectedNoteToDelete(null)}
        />
      )}

      {isReviewSheetOpen && (
        <ReviewIncidentReportSheet
          currentForm={currentForm}
          language={language}
          locationLabel={getIncidentReportLocationLabel(getLocation(), visits)}
          answers={answers}
          attachments={attachments}
          notes={notes}
          isSubmitting={isPending}
          isUploading={isUploading}
          uploadProgress={uploadProgress}
          onSubmit={onSubmitReport}
          onAbortUpload={onAbortUpload}
          onClose={() => setIsReviewSheetOpen(false)}
        />
      )}

      {isOptionsSheetOpen && (
        <OptionsSheet
          open
          setOpen={(open) => {
            setIsOptionsSheetOpen(open);
            if (!open) setNoteEditor(null);
          }}
          isLoading={isPreparingFile}
          disableDrag={!!noteEditor}
        >
          {isPreparingFile ? (
            <MediaLoading progress={uploadProgress} />
          ) : noteEditor ? (
            <NoteForm
              key={noteEditor.noteId ?? "new"}
              heading={
                noteEditor.noteId
                  ? t("notes.edit.heading", { ns: "polling_station_form_wizard" })
                  : t("notes.add.heading", { ns: "polling_station_form_wizard" })
              }
              initialText={noteEditor.text}
              onSave={saveLocalNote}
              onCancel={() => setNoteEditor(null)}
            />
          ) : (
            <YStack paddingHorizontal="$sm" gap="$xxs">
              <Typography
                onPress={() => setNoteEditor({})}
                preset="body1"
                paddingVertical="$md"
                pressStyle={{ color: "$purple5" }}
              >
                {t("attachments.menu.add_note", { ns: "polling_station_form_wizard" })}
              </Typography>
              <Typography
                onPress={whenOnline(() => handleCameraUpload("library"))}
                preset="body1"
                paddingVertical="$md"
                pressStyle={{ color: "$purple5" }}
              >
                {t("media.menu.load", { ns: "report_new_issue" })}
              </Typography>
              <Typography
                onPress={whenOnline(() => handleCameraUpload("cameraPhoto"))}
                preset="body1"
                paddingVertical="$md"
                pressStyle={{ color: "$purple5" }}
              >
                {t("media.menu.take_picture", { ns: "report_new_issue" })}
              </Typography>
              <Typography
                onPress={whenOnline(handleUploadAudio)}
                preset="body1"
                paddingVertical="$md"
                pressStyle={{ color: "$purple5" }}
              >
                {t("media.menu.upload_audio", { ns: "report_new_issue" })}
              </Typography>
            </YStack>
          )}
        </OptionsSheet>
      )}

      {showWarningDialog && (
        <WarningDialog
          theme="info"
          title={t("unsaved_changes_dialog.title")}
          description={t("unsaved_changes_dialog.description")}
          action={() => setShowWarningDialog(false)}
          onCancel={() => {
            setShowWarningDialog(false);
            router.back();
          }}
          actionBtnText={t("unsaved_changes_dialog.actions.save")}
          cancelBtnText={t("unsaved_changes_dialog.actions.discard")}
        />
      )}
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

export default IncidentReport;
