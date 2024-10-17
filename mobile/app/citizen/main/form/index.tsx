import { useEffect, useMemo, useRef, useState } from "react";
import { Card, ScrollView, XStack, YStack } from "tamagui";
import { Screen } from "../../../../components/Screen";
import Header from "../../../../components/Header";
import { useLocalSearchParams, useRouter } from "expo-router";
import { Icon } from "../../../../components/Icon";
import { useGetCitizenReportingFormById } from "../../../../services/queries/citizen.query";
import { useCitizenUserData } from "../../../../contexts/citizen-user/CitizenUserContext.provider";
import { Typography } from "../../../../components/Typography";
import { BackHandler, Keyboard, ViewStyle } from "react-native";
import LinearProgress from "../../../../components/LinearProgress";
import { useTranslation } from "react-i18next";
import { ApiFormQuestion } from "../../../../services/interfaces/question.type";
import { ApiFormAnswer } from "../../../../services/interfaces/answer.type";
import {
  mapFormSubmissionDataToAPIFormSubmissionAnswer,
  setFormDefaultValues,
  shouldDisplayQuestion,
} from "../../../../services/form.parser";
import QuestionForm from "../../../../components/QuestionForm";
import { useForm } from "react-hook-form";
import { scrollToTextarea } from "../../../../helpers/scrollToTextarea";
import WizzardControls from "../../../../components/WizzardControls";
import { useNetInfoContext } from "../../../../contexts/net-info-banner/NetInfoContext";
import Toast from "react-native-toast-message";
import ReviewCitizenFormSheet from "../../../../components/ReviewCitizenFormSheet";
import WarningDialog from "../../../../components/WarningDialog";
import i18n from "../../../../common/config/i18n";
import AddAttachment from "../../../../components/AddAttachment";
import OptionsSheet from "../../../../components/OptionsSheet";
import MediaLoading from "../../../../components/MediaLoading";
import { FileMetadata, useCamera } from "../../../../hooks/useCamera";
import * as DocumentPicker from "expo-document-picker";
import * as Crypto from "expo-crypto";
// import { Buffer } from "buffer";
// import * as FileSystem from "expo-file-system";

const CitizenForm = () => {
  const { t } = useTranslation(["citizen_form", "network_banner"]);
  const router = useRouter();
  const scrollViewRef = useRef(null);
  const textareaRef = useRef(null);

  const { selectedElectionRound } = useCitizenUserData();
  const { isOnline } = useNetInfoContext();

  const [isOptionsSheetOpen, setIsOptionsSheetOpen] = useState(false);
  const [isPreparingFile, setIsPreparingFile] = useState(false);
  const [uploadProgress, setUploadProgress] = useState("");
  const [attachments, setAttachments] = useState<
    Record<string, { fileMetadata: FileMetadata; id: string }[]>
  >({});

  const { uploadCameraOrMedia } = useCamera();

  if (!selectedElectionRound) {
    return (
      <Typography>
        [CitizenForm] There is no selected election round - Incorrect page params
      </Typography>
    );
  }

  const {
    formId,
    selectedLocationId,
    questionId: initialQuestionId,
  } = useLocalSearchParams<{
    formId: string;
    selectedLocationId: string;
    questionId: string;
  }>();

  if (!formId || !selectedLocationId || !initialQuestionId) {
    return <Typography>Incorrect page params</Typography>;
  }

  const handleFocus = () => {
    scrollToTextarea(scrollViewRef, textareaRef);
  };

  const [answers, setAnswers] = useState<Record<string, ApiFormAnswer | undefined> | undefined>();
  const [questionId, setQuestionId] = useState<string>(initialQuestionId);
  const [isReviewSheetOpen, setIsReviewSheetOpen] = useState(false);
  const [showWarningDialog, setShowWarningDialog] = useState(false);

  const {
    data: currentForm,
    isLoading: isLoadingCurrentForm,
    error: currentFormError,
  } = useGetCitizenReportingFormById(selectedElectionRound, formId);

  const language = useMemo(() => {
    if (!currentForm) return i18n.language;
    // if the language of the app can be found in the form, use it, otherwise use the form default language or the first language found in the languages array coming from the form
    if (currentForm?.languages.includes(i18n.language)) {
      return i18n.language;
    }

    return currentForm?.defaultLanguage || currentForm?.languages[0];
  }, [currentForm]);

  const displayedQuestions: ApiFormQuestion[] = useMemo(
    () => currentForm?.questions?.filter((q) => shouldDisplayQuestion(q, answers)) || [],
    [currentForm, answers],
  );

  const activeQuestion: {
    indexInAllQuestions: number;
    indexInDisplayedQuestions: number;
    question: ApiFormQuestion;
  } = useMemo(() => {
    return {
      indexInAllQuestions: currentForm?.questions.findIndex((q) => q.id === questionId) || 0,
      indexInDisplayedQuestions: displayedQuestions.findIndex((q) => q.id === questionId) || 0,
      question: currentForm?.questions.find((q) => q.id === questionId) as ApiFormQuestion,
    };
  }, [displayedQuestions, questionId]);

  const formTitle = useMemo(() => {
    return `${currentForm?.code} - ${currentForm?.name[language || currentForm.defaultLanguage]} (${language || currentForm?.defaultLanguage})`;
  }, [currentForm]);

  const {
    control,
    handleSubmit,
    formState: { isDirty },
  } = useForm({
    defaultValues: setFormDefaultValues(questionId, answers?.[questionId]) as any,
  });

  const handleGoBack = () => {
    if (isDirty) {
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
  }, [isDirty]);

  const findNextQuestion = (
    index: number,
    updatedAnswers: Record<string, ApiFormAnswer | undefined> | undefined,
  ): ApiFormQuestion | null => {
    if (index + 1 === currentForm?.questions.length) {
      // No more questions
      return null;
    }

    const nextQuestion = currentForm?.questions[index + 1];

    if (nextQuestion && shouldDisplayQuestion(nextQuestion, updatedAnswers)) {
      return nextQuestion;
    }

    return findNextQuestion(index + 1, updatedAnswers);
  };

  const findPreviousQuestion = (
    index: number,
    answers: Record<string, ApiFormAnswer | undefined> | undefined,
  ): ApiFormQuestion | null => {
    if (index - 1 < 0) {
      // No more questions
      return null;
    }

    const prevQ = currentForm?.questions[index - 1];

    if (prevQ && shouldDisplayQuestion(prevQ, answers)) {
      return prevQ;
    }

    return findPreviousQuestion(index - 1, answers);
  };

  const goToNextQuestion = (nextQuestion: ApiFormQuestion | null) => {
    if (nextQuestion) {
      setQuestionId(nextQuestion.id);
    }
  };

  const goToPrevQuestion = () => {
    const prevQ = findPreviousQuestion(activeQuestion.indexInAllQuestions, answers);
    if (prevQ) {
      setQuestionId(prevQ.id);
    }
  };

  const onSubmitAnswer = (formValues: any) => {
    const questionId = activeQuestion?.question.id as string;

    if (activeQuestion) {
      // map the answer values
      const updatedAnswer = mapFormSubmissionDataToAPIFormSubmissionAnswer(
        questionId,
        activeQuestion?.question.$questionType,
        formValues[questionId],
      );

      const updatedAnswers = {
        ...answers,
        [activeQuestion.question.id]: updatedAnswer,
      };

      // Find dependent questions for the current one and remove their answers
      currentForm?.questions
        ?.filter((q) => q.displayLogic?.parentQuestionId === questionId)
        .map((q) => q.id)
        .forEach((qId) => {
          if (updatedAnswers) delete updatedAnswers[qId];
        });

      const nextQuestion = findNextQuestion(activeQuestion.indexInAllQuestions, updatedAnswers);
      if (nextQuestion) {
        setAnswers(updatedAnswers);
        goToNextQuestion(nextQuestion);
      } else {
        if (!isOnline) {
          return Toast.show({
            type: "error",
            text2: t("offline_citizen", { ns: "network_banner" }),
            visibilityTime: 5000,
            text2Style: { textAlign: "center" },
          });
        }

        if (currentForm) {
          // open review sheet
          setAnswers(updatedAnswers);
          Keyboard.dismiss();
          setIsReviewSheetOpen(true);
        }
      }
    }
  };

  const onCompressionProgress = (progress: number) => {
    setUploadProgress(`${t("attachments.upload.compressing")} ${Math.ceil(progress * 100)}%`);
  };

  const removeAttachmentLocal = (id: string): void => {
    setAttachments((prevAttachments) => {
      return {
        ...prevAttachments,
        [questionId]: prevAttachments[questionId].filter((attachment) => attachment.id !== id),
      };
    });
  };

  const handleCameraUpload = async (type: "library" | "cameraPhoto") => {
    setIsPreparingFile(true);
    setUploadProgress(t("attachments.upload.preparing"));
    const cameraResult = await uploadCameraOrMedia(type, onCompressionProgress);

    if (!cameraResult) {
      setIsPreparingFile(false);
      return;
    }

    setIsOptionsSheetOpen(false);
    setAttachments((prevAttachments) => ({
      ...prevAttachments,
      [questionId]: prevAttachments[questionId]
        ? [...prevAttachments[questionId], { fileMetadata: cameraResult, id: Crypto.randomUUID() }]
        : [{ fileMetadata: cameraResult, id: Crypto.randomUUID() }],
    }));
    setIsPreparingFile(false);
  };

  const handleUploadAudio = async () => {
    const doc = await DocumentPicker.getDocumentAsync({
      type: "audio/*",
      multiple: false,
    });

    if (doc?.assets?.[0]) {
      const file = doc?.assets?.[0];

      const fileMetadata: FileMetadata = {
        name: file.name,
        type: file.mimeType || "audio/mpeg",
        uri: file.uri,
        size: file.size || 0,
      };

      setIsOptionsSheetOpen(false);
      setAttachments((prevAttachments) => ({
        ...prevAttachments,
        [questionId]: prevAttachments[questionId]
          ? [...prevAttachments[questionId], { fileMetadata, id: Crypto.randomUUID() }]
          : [{ fileMetadata, id: Crypto.randomUUID() }],
      }));
      setIsPreparingFile(false);
    } else {
      // Cancelled
    }

    setIsPreparingFile(false);
  };

  if (isLoadingCurrentForm || !activeQuestion) {
    return <Typography>Loading...</Typography>;
  }

  if (currentFormError) {
    return <Typography>Error loading form {JSON.stringify(currentFormError)}</Typography>;
  }

  return (
    <Screen
      preset="fixed"
      backgroundColor="white"
      style={$screenStyle}
      contentContainerStyle={$containerStyle}
    >
      <Header
        title={formTitle}
        leftIcon={<Icon icon="chevronLeft" color="white" />}
        onLeftPress={handleGoBack}
      />
      <ScrollView
        ref={scrollViewRef}
        contentContainerStyle={{ flexGrow: 1 }}
        keyboardShouldPersistTaps="handled"
      >
        <YStack gap="$xxs" padding="$md">
          <XStack justifyContent="space-between">
            <Typography>{t("progress_bar_label")}</Typography>
            <Typography justifyContent="space-between">{`${activeQuestion?.indexInDisplayedQuestions + 1}/${displayedQuestions.length}`}</Typography>
          </XStack>
          <LinearProgress
            current={activeQuestion?.indexInDisplayedQuestions + 1}
            total={displayedQuestions?.length || 0}
          />
        </YStack>

        <YStack paddingHorizontal="$md" paddingBottom="$md" justifyContent="center" flex={1}>
          <QuestionForm
            control={control}
            activeQuestion={activeQuestion}
            handleFocus={handleFocus}
            ref={textareaRef}
            language={language}
            required={true}
          />

          {attachments[questionId]?.length ? (
            <YStack gap="$xxs" paddingTop="$lg">
              <Typography fontWeight="500">{t("attachments.heading")}</Typography>
              <YStack gap="$xxs">
                {attachments[questionId].map((attachment) => {
                  return (
                    <Card
                      padding="$0"
                      paddingLeft="$md"
                      key={attachment.id}
                      flexDirection="row"
                      justifyContent="space-between"
                      alignItems="center"
                    >
                      <Typography preset="body1" fontWeight="700" maxWidth="85%" numberOfLines={1}>
                        {attachment.fileMetadata.name}
                      </Typography>
                      <YStack
                        padding="$md"
                        onPress={removeAttachmentLocal.bind(null, attachment.id)}
                        pressStyle={{ opacity: 0.5 }}
                      >
                        <Icon icon="xCircle" size={24} color="$gray5" />
                      </YStack>
                    </Card>
                  );
                })}
              </YStack>
            </YStack>
          ) : (
            false
          )}

          <AddAttachment
            label={t("attachments.add")}
            marginTop="$sm"
            onPress={() => {
              Keyboard.dismiss();
              setIsOptionsSheetOpen(true);
            }}
          />
        </YStack>
      </ScrollView>
      <WizzardControls
        isFirstElement={activeQuestion?.indexInAllQuestions === 0}
        isLastElement={
          currentForm?.questions &&
          activeQuestion?.indexInAllQuestions === currentForm?.questions?.length - 1
        }
        isNextDisabled={false}
        onActionButtonPress={() => {
          handleSubmit(onSubmitAnswer)();
        }}
        onPreviousButtonPress={goToPrevQuestion}
      />

      {isReviewSheetOpen && (
        <ReviewCitizenFormSheet
          language={language}
          currentForm={currentForm}
          answers={answers}
          questions={currentForm?.questions}
          attachments={attachments}
          setIsReviewSheetOpen={setIsReviewSheetOpen}
          selectedLocationId={selectedLocationId}
        />
      )}

      {isOptionsSheetOpen && (
        <OptionsSheet setOpen={setIsOptionsSheetOpen} open isLoading={isPreparingFile}>
          {isPreparingFile ? (
            <MediaLoading progress={uploadProgress} />
          ) : (
            <YStack paddingHorizontal="$sm" gap="$xxs">
              <Typography
                onPress={handleCameraUpload.bind(null, "library")}
                preset="body1"
                paddingVertical="$md"
                pressStyle={{ color: "$purple5" }}
              >
                {t("attachments.menu.load")}
              </Typography>
              <Typography
                onPress={handleCameraUpload.bind(null, "cameraPhoto")}
                preset="body1"
                paddingVertical="$md"
                pressStyle={{ color: "$purple5" }}
              >
                {t("attachments.menu.take_picture")}
              </Typography>
              <Typography
                onPress={handleUploadAudio.bind(null)}
                preset="body1"
                paddingVertical="$md"
                pressStyle={{ color: "$purple5" }}
              >
                {t("attachments.menu.upload_audio")}
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
          action={() => {
            // close dialog and remain on this page
            setShowWarningDialog(false);
          }}
          onCancel={() => {
            // close dialog and go back without saving changes
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

export default CitizenForm;
