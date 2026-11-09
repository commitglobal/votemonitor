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
import ChangeLanguageDialog from "../../../components/ChangeLanguageDialog";
import { setFormLanguagePreference } from "../../../common/language.preferences";
import QuestionForm from "../../../components/QuestionForm";
import { Screen } from "../../../components/Screen";
import { ScreenScrollView } from "../../../components/ScreenScrollView";
import Select from "../../../components/Select";
import { Typography } from "../../../components/Typography";
import Button from "../../../components/Button";
import WarningDialog from "../../../components/WarningDialog";
import WizzardControls from "../../../components/WizzardControls";
import { useNetInfoContext } from "../../../contexts/net-info-banner/NetInfoContext";
import { useUserData } from "../../../contexts/user/UserContext.provider";
import { getFormDisplayName, getFormLanguage } from "../../../helpers/incidentReport";
import { FileMetadata, useCamera } from "../../../hooks/useCamera";
import { AttachmentMimeType } from "../../../services/api/get-attachments.api";
import { IncidentReportLocationType } from "../../../services/api/incident-report/incident-report.api";
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
  useDeleteIncidentReportAttachment,
  useDeleteIncidentReportNote,
} from "../../../services/mutations/incident-report/add-incident-report.mutation";
import { useFormById } from "../../../services/queries/forms.query";
import {
  IncidentReportKeys,
  useIncidentReportAttachments,
  useIncidentReportById,
} from "../../../services/queries/incident-reports.query";

// without an id (from a server that does not send it) a note can only be shown, not edited or deleted
type ReportNote = { id?: string; text: string };
// without an id (when it could not be loaded with its id) a file can only be shown, not deleted
type ReportAttachment = { id?: string; fileName: string; mimeType: string; uri: string };

// the active question's value, keyed by question id (the shape setFormDefaultValues gives)
type QuestionFormValues = NonNullable<ReturnType<typeof setFormDefaultValues>>;

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

const groupByQuestion = <T extends { questionId: string }, R>(
  items: T[],
  map: (item: T) => R,
): Record<string, R[]> =>
  items.reduce(
    (acc, item) => {
      acc[item.questionId] = [...(acc[item.questionId] || []), map(item)];
      return acc;
    },
    {} as Record<string, R[]>,
  );

/*
  An incident report is saved while it is filled in, so the organization has whatever the observer
  managed to send even if they can not finish it:
  - the draft (isCompleted: false) is created once the location is chosen, and saved again with all
    the answers on every "Next step" / "Back" (queued while offline, sent in order later)
  - notes are sent when they are saved, edited or deleted (queued while offline, after the report)
  - media files are uploaded as soon as they are picked (that needs a connection)
  - "Send report" (any question, one tap) saves it with isCompleted: true, which locks
    it (the server refuses further changes)
  A draft can be continued later from the report list (incidentReportId param).
*/
const IncidentReport = () => {
  const params = useLocalSearchParams<{
    formId: string;
    language?: string;
    // set when continuing a draft
    incidentReportId?: string;
  }>();
  const { formId, language: languageParam } = params;
  const isResuming = !!params.incidentReportId;

  const { t } = useTranslation([
    "incident_report",
    "report_new_issue",
    "report_details",
    "polling_station_form_wizard",
    "languages",
  ]);
  const queryClient = useQueryClient();
  const cancelRef = useRef(false);

  const { visits, activeElectionRound } = useUserData();
  const { isOnline } = useNetInfoContext();
  const { uploadCameraOrMedia } = useCamera();

  const [incidentReportId] = useState(params.incidentReportId ?? Crypto.randomUUID());

  const { data: currentForm, isLoading: isLoadingForm } = useFormById(
    activeElectionRound?.id,
    formId,
  );
  const { data: existingReport, isLoading: isLoadingReport } = useIncidentReportById(
    activeElectionRound?.id,
    incidentReportId,
  );
  const { data: existingAttachments, isFetchedAfterMount: areExistingAttachmentsFresh } =
    useIncidentReportAttachments(
      activeElectionRound?.id,
      incidentReportId,
      formId,
      isResuming && isOnline,
    );

  // once the draft is saved (or queued while offline) the location is fixed: the server keeps the first one
  const [isSaved, setIsSaved] = useState(isResuming);
  // whether the report is known to exist on the server, which uploading media needs
  const isOnServerRef = useRef(isResuming);
  // once sent the report is locked: nothing is saved or sent again from this screen (e.g. a second tap)
  const isSentRef = useRef(false);
  const [isInitialized, setIsInitialized] = useState(!isResuming);

  const [step, setStep] = useState<"location" | "questions">(isResuming ? "questions" : "location");
  const [questionId, setQuestionId] = useState<string | undefined>();
  const [answers, setAnswers] = useState<Record<string, ApiFormAnswer | undefined>>({});
  const [notes, setNotes] = useState<Record<string, ReportNote[]>>({});
  const [attachments, setAttachments] = useState<Record<string, ReportAttachment[]>>({});

  // when set, the options sheet shows the note form (a new note, or editing the one with noteId)
  const [noteEditor, setNoteEditor] = useState<{ noteId?: string; text?: string } | null>(null);
  const [selectedNoteToDelete, setSelectedNoteToDelete] = useState<string | null>(null);
  const [selectedAttachmentToDelete, setSelectedAttachmentToDelete] = useState<string | null>(null);
  const [previewAttachment, setPreviewAttachment] = useState<ReportAttachment | null>(null);

  const [showWarningDialog, setShowWarningDialog] = useState(false);
  const [isOptionsSheetOpen, setIsOptionsSheetOpen] = useState(false);
  const [isPreparingFile, setIsPreparingFile] = useState(false);
  const [isUploading, setIsUploading] = useState(false);
  const [uploadProgress, setUploadProgress] = useState("");

  const { mutate: saveReport, mutateAsync: saveReportAsync } =
    useAddIncidentReport(incidentReportId);
  const { mutate: saveNoteOnServer } = useAddIncidentReportNote(incidentReportId);
  const { mutate: deleteNoteOnServer } = useDeleteIncidentReportNote(incidentReportId);
  const { mutateAsync: deleteAttachmentOnServer } = useDeleteIncidentReportAttachment();
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
    reset: resetLocationForm,
    formState: { errors: locationErrors, isDirty: isLocationDirty },
  } = useForm<LocationFormType>({
    defaultValues: { location: "", locationDescription: "" },
  });
  const locationWatch = watchLocation("location");

  // the language picked when choosing the form (or switched to here), if the form still has it
  const [selectedLanguage, setSelectedLanguage] = useState(languageParam);
  const [isChangeLanguageOpen, setIsChangeLanguageOpen] = useState(false);
  const [isFormMenuOpen, setIsFormMenuOpen] = useState(false);
  const language = useMemo(
    () =>
      selectedLanguage && currentForm?.languages.includes(selectedLanguage)
        ? selectedLanguage
        : getFormLanguage(currentForm),
    [currentForm, selectedLanguage],
  );

  // only changes how the questions are shown: answers are kept by question / option id
  const onChangeLanguage = (formId: string, newLanguage: string) => {
    setFormLanguagePreference({ formId, language: newLanguage });
    setSelectedLanguage(newLanguage);
    setIsChangeLanguageOpen(false);
  };
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
  } = useForm<QuestionFormValues>();

  // one screen walks through all the questions, so load the saved answer whenever the question changes
  useEffect(() => {
    if (questionId) {
      resetQuestionForm(setFormDefaultValues(questionId, answers[questionId]));
    }
  }, [questionId]);

  const findNextQuestion = (
    index: number,
    updatedAnswers: Record<string, ApiFormAnswer | undefined>,
  ): ApiFormQuestion | null => {
    if (!currentForm || index + 1 >= currentForm.questions.length) return null;
    const nextQuestion = currentForm.questions[index + 1];
    if (shouldDisplayQuestion(nextQuestion, updatedAnswers)) return nextQuestion;
    return findNextQuestion(index + 1, updatedAnswers);
  };

  const findPreviousQuestion = (
    index: number,
    updatedAnswers: Record<string, ApiFormAnswer | undefined>,
  ): ApiFormQuestion | null => {
    if (!currentForm || index - 1 < 0) return null;
    const prevQuestion = currentForm.questions[index - 1];
    if (shouldDisplayQuestion(prevQuestion, updatedAnswers)) return prevQuestion;
    return findPreviousQuestion(index - 1, updatedAnswers);
  };

  // continuing a draft: fill in what was already sent
  useEffect(() => {
    if (isInitialized || !currentForm || !existingReport) return;

    if (existingReport.isCompleted) {
      // a sent report is locked, it can only be viewed
      router.replace(`/incident-report-details/${incidentReportId}`);
      return;
    }

    resetLocationForm({
      location:
        existingReport.locationType === IncidentReportLocationType.PollingStation
          ? existingReport.pollingStationId || ""
          : existingReport.locationType,
      locationDescription: existingReport.locationDescription || "",
    });
    const existingAnswers = Object.fromEntries(
      existingReport.answers.map((answer) => [answer.questionId, answer]),
    );
    setAnswers(existingAnswers);
    setNotes(groupByQuestion(existingReport.notes, (note) => ({ id: note.id, text: note.text })));
    setAttachments(
      groupByQuestion(existingReport.attachments, (attachment) => ({
        fileName: attachment.fileName,
        mimeType: attachment.mimeType,
        uri: attachment.presignedUrl,
      })),
    );
    setQuestionId(findNextQuestion(-1, existingAnswers)?.id);
    setIsInitialized(true);
  }, [currentForm, existingReport, isInitialized]);

  // the attachments with their ids (so they can be deleted) replace the ones from the report list
  const appliedExistingAttachmentsRef = useRef(false);
  useEffect(() => {
    // a cached list can be older than the report (e.g. a file uploaded since), so only a fresh one is used
    if (
      !isInitialized ||
      !existingAttachments ||
      !areExistingAttachmentsFresh ||
      appliedExistingAttachmentsRef.current
    ) {
      return;
    }
    appliedExistingAttachmentsRef.current = true;
    setAttachments((current) => {
      const fromServer = groupByQuestion(existingAttachments, (attachment): ReportAttachment => ({
        id: attachment.id,
        fileName: attachment.fileName,
        mimeType: attachment.mimeType,
        uri: attachment.presignedUrl,
      }));
      // keep the ones uploaded in the meantime
      Object.entries(current).forEach(([id, questionAttachments]) => {
        const known = new Set((fromServer[id] || []).map((a) => a.id));
        const added = questionAttachments.filter((a) => a.id && !known.has(a.id));
        if (added.length) fromServer[id] = [...(fromServer[id] || []), ...added];
      });
      return fromServer;
    });
  }, [isInitialized, existingAttachments, areExistingAttachmentsFresh]);

  const handleGoBack = () => {
    // a saved draft can be continued from the report list, only an unsaved location is lost
    if (!isSaved && isLocationDirty) {
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
  }, [isSaved, isLocationDirty]);

  const getLocation = () => {
    const { location, locationDescription } = getLocationValues();
    return isLocationType(location)
      ? {
          locationType: location as IncidentReportLocationType,
          locationDescription,
        }
      : { locationType: IncidentReportLocationType.PollingStation, pollingStationId: location };
  };

  const buildReportPayload = (
    reportAnswers: Record<string, ApiFormAnswer | undefined>,
    isCompleted: boolean,
  ) => {
    const now = new Date().toISOString();
    return {
      id: incidentReportId,
      electionRoundId: activeElectionRound!.id,
      formId: currentForm!.id,
      ...getLocation(),
      answers: Object.values(reportAnswers).filter(Boolean) as ApiFormAnswer[],
      isCompleted,
      lastUpdatedAt: now,
      // only for showing the report in the list until it is reloaded from the server
      attachments: Object.entries(attachments).flatMap(([id, questionAttachments]) =>
        questionAttachments.map((attachment) => ({
          submissionId: incidentReportId,
          questionId: id,
          fileName: attachment.fileName,
          mimeType: attachment.mimeType,
          presignedUrl: attachment.uri,
          urlValidityInSeconds: 0,
        })),
      ),
      notes: Object.entries(notes).flatMap(([id, questionNotes]) =>
        questionNotes.map((note) => ({
          id: note.id,
          submissionId: incidentReportId,
          questionId: id,
          text: note.text,
          timeSubmitted: now,
        })),
      ),
    };
  };

  // saves the draft; queued (and sent in order with the notes) while offline
  const saveDraft = (reportAnswers: Record<string, ApiFormAnswer | undefined>) => {
    if (!activeElectionRound || !currentForm || isSentRef.current) return;
    saveReport(buildReportPayload(reportAnswers, false), {
      onSuccess: () => {
        isOnServerRef.current = true;
      },
    });
    setIsSaved(true);
  };

  // media files reference the report, so it has to exist on the server before uploading one
  const ensureReportOnServer = async () => {
    if (isOnServerRef.current) return;
    await saveReportAsync(buildReportPayload(answers, false));
    isOnServerRef.current = true;
  };

  const onSubmitLocation = () => {
    saveDraft(answers);
    const firstQuestion = findNextQuestion(-1, answers);
    if (!firstQuestion) {
      // a form without questions: nothing to fill in, the report can be sent right away
      sendReport(answers);
      return;
    }
    setQuestionId((current) => current ?? firstQuestion.id);
    setStep("questions");
  };

  // keeps the active question's answer and saves the draft when anything changed
  const commitAnswer = (formValues: QuestionFormValues) => {
    if (!activeQuestion) return answers;
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

    if (JSON.stringify(updatedAnswers) !== JSON.stringify(answers)) {
      setAnswers(updatedAnswers);
      saveDraft(updatedAnswers);
    }
    return updatedAnswers;
  };

  const onSubmitAnswer = (formValues: QuestionFormValues) => {
    if (!activeQuestion) return;
    const updatedAnswers = commitAnswer(formValues);
    const nextQuestion = findNextQuestion(activeQuestion.indexInAllQuestions, updatedAnswers);
    if (nextQuestion) {
      setQuestionId(nextQuestion.id);
    }
  };

  // the report can be sent at any point, in one tap (no review step: an observer in danger must be able
  // to send what they have right away), with whatever was filled in so far
  const onSendNow = (formValues: QuestionFormValues) => {
    sendReport(commitAnswer(formValues));
  };

  const onPreviousAnswer = (formValues: QuestionFormValues) => {
    if (!activeQuestion) return;
    const updatedAnswers = commitAnswer(formValues);
    const prevQuestion = findPreviousQuestion(activeQuestion.indexInAllQuestions, updatedAnswers);
    if (prevQuestion) {
      setQuestionId(prevQuestion.id);
    } else if (!isSaved) {
      setStep("location");
    }
  };

  // never waits for the server: the save is queued (with the earlier ones) and sent as soon as there
  // is a connection, retried while there is none, and kept if the app is closed while offline
  const sendReport = (reportAnswers: Record<string, ApiFormAnswer | undefined>) => {
    if (!activeElectionRound || !currentForm || isSentRef.current) return;
    isSentRef.current = true;

    Keyboard.dismiss();
    saveReport(buildReportPayload(reportAnswers, true));
    Toast.show({
      type: "success",
      text2: isOnline ? t("sending.sent") : t("sending.queued"),
      visibilityTime: 4000,
    });
    router.back();
  };

  const uploadNewAttachment = async (fileMetadata: FileMetadata) => {
    if (!activeElectionRound || !currentForm || !questionId) return;
    const attachmentQuestionId = questionId;
    const attachmentId = Crypto.randomUUID();
    let uploadId: string | undefined;

    cancelRef.current = false;
    setIsUploading(true);
    setUploadProgress(t("upload.starting", { ns: "report_new_issue" }));

    try {
      await ensureReportOnServer();

      const numberOfUploadParts = Math.ceil(fileMetadata.size! / MULTIPART_FILE_UPLOAD_SIZE);
      const upload = await initAttachment({
        id: attachmentId,
        electionRoundId: activeElectionRound.id,
        incidentReportId,
        formId: currentForm.id,
        questionId: attachmentQuestionId,
        fileName: fileMetadata.name,
        contentType: fileMetadata.type,
        numberOfUploadParts,
      });
      uploadId = upload.uploadId;

      let etags: Record<number, string> = {};
      for (const [index, url] of Object.values(upload.uploadUrls).entries()) {
        if (cancelRef.current) {
          throw new Error("Upload aborted");
        }
        const data = await uploadS3Chunk({
          url,
          filePath: fileMetadata.uri,
          partIndex: index,
          partSize: MULTIPART_FILE_UPLOAD_SIZE,
        });
        setUploadProgress(
          `${t("upload.progress", { ns: "report_new_issue" })} ${Math.ceil(((index + 1) / numberOfUploadParts) * 100)}%`,
        );
        etags = { ...etags, [index + 1]: data.ETag };
      }

      await completeAttachment({
        uploadId,
        etags,
        electionRoundId: activeElectionRound.id,
        id: attachmentId,
        incidentReportId,
      });

      setAttachments((current) => ({
        ...current,
        [attachmentQuestionId]: [
          ...(current[attachmentQuestionId] || []),
          {
            id: attachmentId,
            fileName: fileMetadata.name,
            mimeType: fileMetadata.type,
            uri: fileMetadata.uri,
          },
        ],
      }));
      setIsOptionsSheetOpen(false);
      queryClient.invalidateQueries({
        queryKey: IncidentReportKeys.attachments(activeElectionRound.id, incidentReportId),
      });
      queryClient.invalidateQueries({
        queryKey: IncidentReportKeys.byElectionRound(activeElectionRound.id),
      });
    } catch (err) {
      Sentry.captureException(err, { data: { incidentReportId } });
      if (uploadId) {
        abortAttachment({
          id: attachmentId,
          uploadId,
          electionRoundId: activeElectionRound.id,
          incidentReportId,
        }).catch(() => undefined);
      }
      if (!cancelRef.current) {
        Toast.show({ type: "error", text2: t("media.error", { ns: "report_new_issue" }) });
      }
    } finally {
      setIsUploading(false);
      setUploadProgress("");
    }
  };

  const onAbortUpload = () => {
    cancelRef.current = true;
    setIsUploading(false);
    setIsOptionsSheetOpen(false);
    setUploadProgress("");
    removeMutationByScopeId(queryClient, MUTATION_SCOPE_DO_NOT_HYDRATE);
  };

  const removeAttachment = async (id: string) => {
    if (!activeElectionRound || !questionId) return;
    if (!onlineManager.isOnline()) {
      Toast.show({
        type: "error",
        text2: t("upload.offline", { ns: "report_new_issue" }),
        visibilityTime: 5000,
        text2Style: { textAlign: "center" },
      });
      return;
    }
    const attachmentQuestionId = questionId;
    try {
      await deleteAttachmentOnServer({
        id,
        electionRoundId: activeElectionRound.id,
        incidentReportId,
      });
      setAttachments((current) => ({
        ...current,
        [attachmentQuestionId]: (current[attachmentQuestionId] || []).filter((a) => a.id !== id),
      }));
    } catch (err) {
      Sentry.captureException(err);
      Toast.show({ type: "error", text2: t("error") });
    }
  };

  const handleCameraUpload = async (type: "library" | "cameraPhoto") => {
    setIsPreparingFile(true);
    setUploadProgress(t("upload.preparing", { ns: "report_new_issue" }));
    const cameraResult = await uploadCameraOrMedia(type, (progress: number) =>
      setUploadProgress(
        `${t("upload.compressing", { ns: "report_new_issue" })} ${Math.ceil(progress * 100)}%`,
      ),
    );
    setIsPreparingFile(false);
    if (cameraResult) {
      await uploadNewAttachment(cameraResult);
    } else {
      setUploadProgress("");
    }
  };

  const handleUploadAudio = async () => {
    const doc = await DocumentPicker.getDocumentAsync({ type: "audio/*", multiple: false });
    const file = doc?.assets?.[0];
    if (file) {
      await uploadNewAttachment({
        name: file.name,
        type: file.mimeType || "audio/mpeg",
        uri: file.uri,
        size: file.size || 0,
      });
    }
  };

  const saveNote = (text: string) => {
    if (!activeElectionRound || !currentForm || !questionId || !noteEditor) return;
    const noteQuestionId = questionId;
    const id = noteEditor.noteId ?? Crypto.randomUUID();

    setNotes((current) => {
      const questionNotes = current[noteQuestionId] || [];
      return {
        ...current,
        [noteQuestionId]: noteEditor.noteId
          ? questionNotes.map((note) => (note.id === id ? { ...note, text } : note))
          : [...questionNotes, { id, text }],
      };
    });
    // shares the report's mutation scope: sent after the report, also when queued offline
    saveNoteOnServer({
      id,
      electionRoundId: activeElectionRound.id,
      incidentReportId,
      formId: currentForm.id,
      questionId: noteQuestionId,
      text,
    });

    Keyboard.dismiss();
    setNoteEditor(null);
    setIsOptionsSheetOpen(false);
  };

  const removeNote = (id: string) => {
    if (!activeElectionRound || !questionId) return;
    const noteQuestionId = questionId;
    setNotes((current) => ({
      ...current,
      [noteQuestionId]: (current[noteQuestionId] || []).filter((note) => note.id !== id),
    }));
    deleteNoteOnServer({ id, electionRoundId: activeElectionRound.id, incidentReportId });
  };

  const openNoteEditor = (note: ReportNote) => {
    Keyboard.dismiss();
    setNoteEditor({ noteId: note.id, text: note.text });
    setIsOptionsSheetOpen(true);
  };

  const handleOnShowAttachmentSheet = () => {
    Keyboard.dismiss();
    setIsOptionsSheetOpen(true);
  };

  // adding media needs a connection (the files are uploaded right away)
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

  if (isLoadingForm || (isResuming && isLoadingReport)) {
    return (
      <YStack justifyContent="center" alignItems="center" flex={1}>
        <Spinner size="large" color="$purple5" />
      </YStack>
    );
  }

  if (!currentForm || (isResuming && !existingReport)) {
    return (
      <Screen preset="fixed" contentContainerStyle={$containerStyle}>
        <Header
          title={t("title")}
          leftIcon={<Icon icon="chevronLeft" color="white" />}
          onLeftPress={() => router.back()}
        />
        <Typography padding="$md">
          {currentForm ? t("error", { ns: "report_details" }) : t("form_not_found")}
        </Typography>
      </Screen>
    );
  }

  if (!isInitialized) {
    return (
      <YStack justifyContent="center" alignItems="center" flex={1}>
        <Spinner size="large" color="$purple5" />
      </YStack>
    );
  }

  const questionAttachments = (questionId && attachments[questionId]) || [];
  const questionNotes = (questionId && notes[questionId]) || [];
  const isFirstQuestion =
    !!activeQuestion && findPreviousQuestion(activeQuestion.indexInAllQuestions, answers) === null;
  const isLastQuestion =
    !!activeQuestion && findNextQuestion(activeQuestion.indexInAllQuestions, answers) === null;
  // once saved, the location can not be changed any more
  const showPrevious = !isFirstQuestion || !isSaved;
  // the form menu, as on the other forms (for now it only changes the language)
  const hasFormMenu = step === "questions" && (currentForm?.languages.length ?? 0) > 1;

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
            actionBtnLabel={t("next_question")}
            onActionButtonPress={handleLocationSubmit(onSubmitLocation)}
          />
        ) : (
          <WizzardControls
            isFirstElement={!showPrevious}
            previousBtnLabel={t("previous_question")}
            onPreviousButtonPress={handleQuestionSubmit(onPreviousAnswer)}
            actionBtnLabel={isLastQuestion ? t("review.send") : t("next_question")}
            onActionButtonPress={handleQuestionSubmit(isLastQuestion ? onSendNow : onSubmitAnswer)}
          />
        )
      }
    >
      <Header
        title={formTitle}
        leftIcon={<Icon icon="chevronLeft" color="white" />}
        onLeftPress={handleGoBack}
        rightIcon={hasFormMenu ? <Icon icon="dotsVertical" color="white" /> : undefined}
        onRightPress={
          hasFormMenu
            ? () => {
                Keyboard.dismiss();
                setIsFormMenuOpen(true);
              }
            : undefined
        }
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
              {/* one tap sends the report as it is, from any question; kept away from "Next" */}
              <Button
                preset="outlined"
                marginTop="$sm"
                icon={<Icon icon="send" color="$purple5" size={20} />}
                onPress={handleQuestionSubmit(onSendNow)}
              >
                {t("review.send")}
              </Button>
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
                  {questionNotes.map((note, index) => (
                    <Card
                      key={note.id ?? index}
                      flexDirection="row"
                      justifyContent="space-between"
                      padding="$0"
                      paddingLeft="$md"
                      onPress={note.id ? () => openNoteEditor(note) : undefined}
                    >
                      <Typography paddingVertical="$md" maxWidth="85%" numberOfLines={5}>
                        {note.text}
                      </Typography>
                      {note.id ? (
                        <YStack
                          padding="$md"
                          onPress={() => {
                            Keyboard.dismiss();
                            setSelectedNoteToDelete(note.id!);
                          }}
                          pressStyle={{ opacity: 0.5 }}
                        >
                          <Icon icon="xCircle" size={24} color="$gray5" />
                        </YStack>
                      ) : (
                        false
                      )}
                    </Card>
                  ))}
                </YStack>
              ) : (
                false
              )}

              {questionAttachments.length ? (
                <YStack gap="$xxs" paddingTop="$lg">
                  <Typography fontWeight="500">{t("attachments.heading")}</Typography>
                  {questionAttachments.map((attachment, index) => (
                    <Card
                      padding="$0"
                      paddingLeft="$md"
                      key={attachment.id ?? index}
                      flexDirection="row"
                      justifyContent="space-between"
                      alignItems="center"
                      onPress={() => setPreviewAttachment(attachment)}
                    >
                      <Typography
                        preset="body1"
                        fontWeight="700"
                        maxWidth="85%"
                        numberOfLines={1}
                        paddingVertical="$md"
                      >
                        {attachment.fileName}
                      </Typography>
                      {attachment.id ? (
                        <YStack
                          padding="$md"
                          onPress={() => {
                            Keyboard.dismiss();
                            setSelectedAttachmentToDelete(attachment.id!);
                          }}
                          pressStyle={{ opacity: 0.5 }}
                        >
                          <Icon icon="xCircle" size={24} color="$gray5" />
                        </YStack>
                      ) : (
                        false
                      )}
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

      {previewAttachment && previewAttachment.mimeType === AttachmentMimeType.IMG && (
        <MediaDialog
          media={{ type: previewAttachment.mimeType, src: previewAttachment.uri }}
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
            removeAttachment(selectedAttachmentToDelete);
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
            removeNote(selectedNoteToDelete);
            setSelectedNoteToDelete(null);
          }}
          onCancel={() => setSelectedNoteToDelete(null)}
        />
      )}

      {isOptionsSheetOpen && (
        <OptionsSheet
          open
          setOpen={(open) => {
            if (isUploading) return;
            setIsOptionsSheetOpen(open);
            if (!open) setNoteEditor(null);
          }}
          isLoading={isPreparingFile || isUploading}
          disableDrag={!!noteEditor}
        >
          {isPreparingFile || isUploading ? (
            <MediaLoading
              progress={uploadProgress}
              isUploading={isUploading}
              onAbortUpload={onAbortUpload}
            />
          ) : noteEditor ? (
            <NoteForm
              key={noteEditor.noteId ?? "new"}
              heading={
                noteEditor.noteId
                  ? t("notes.edit.heading", { ns: "polling_station_form_wizard" })
                  : t("notes.add.heading", { ns: "polling_station_form_wizard" })
              }
              initialText={noteEditor.text}
              onSave={saveNote}
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

      {isFormMenuOpen && (
        <OptionsSheet open setOpen={setIsFormMenuOpen}>
          <YStack paddingVertical="$xxs" paddingHorizontal="$sm" gap="$xxs">
            <Typography
              preset="body1"
              paddingVertical="$xs"
              onPress={() => {
                setIsFormMenuOpen(false);
                setIsChangeLanguageOpen(true);
              }}
            >
              {t("change_language")}
            </Typography>
          </YStack>
        </OptionsSheet>
      )}

      {isChangeLanguageOpen && currentForm && (
        <ChangeLanguageDialog
          formId={currentForm.id}
          currentLanguage={language}
          languages={currentForm.languages}
          onSelectLanguage={onChangeLanguage}
          onCancel={() => setIsChangeLanguageOpen(false)}
        />
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
