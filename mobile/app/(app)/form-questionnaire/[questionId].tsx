import { router, useLocalSearchParams } from "expo-router";
import { Screen } from "../../../components/Screen";
import { Icon } from "../../../components/Icon";
import Header from "../../../components/Header";
import { Typography } from "../../../components/Typography";
import { XStack, YStack, Spinner } from "tamagui";
import LinearProgress from "../../../components/LinearProgress";
import { useMemo, useState } from "react";
import { useUserData } from "../../../contexts/user/UserContext.provider";
import WizzardControls from "../../../components/WizzardControls";
import { Keyboard, Platform, ViewStyle } from "react-native";
import { Controller, useForm } from "react-hook-form";
import WizardFormInput from "../../../components/WizardFormInputs/WizardFormInput";
import {
  mapFormSubmissionDataToAPIFormSubmissionAnswer,
  setFormDefaultValues,
  shouldDisplayQuestion,
} from "../../../services/form.parser";
import { ApiFormAnswer } from "../../../services/interfaces/answer.type";
import WizardDateFormInput from "../../../components/WizardFormInputs/WizardDateFormInput";
import WizardRadioFormInput from "../../../components/WizardFormInputs/WizardRadioFormInput";
import WizardFormElement from "../../../components/WizardFormInputs/WizardFormElement";
import CheckboxInput from "../../../components/Inputs/CheckboxInput";
import WizardRatingFormInput from "../../../components/WizardFormInputs/WizardRatingFormInput";
import { useFormSubmissionMutation } from "../../../services/mutations/form-submission.mutation";
import OptionsSheet from "../../../components/OptionsSheet";
import AddAttachment from "../../../components/AddAttachment";
import { FileMetadata, useCamera } from "../../../hooks/useCamera";
import {
  UploadAttachmentProgress,
  useUploadAttachmentMutation,
  useUploadAttachmentProgressQuery,
} from "../../../services/mutations/attachments/add-attachment.mutation";
import QuestionAttachments from "../../../components/QuestionAttachments";
import QuestionNotes from "../../../components/QuestionNotes";
import AddNoteSheetContent from "../../../components/AddNoteSheetContent";
import { useFormById } from "../../../services/queries/forms.query";
import { useFormAnswers } from "../../../services/queries/form-submissions.query";
import { useNotesForQuestionId } from "../../../services/queries/notes.query";
import * as Crypto from "expo-crypto";
import { useTranslation } from "react-i18next";
import { onlineManager, useQueryClient } from "@tanstack/react-query";
import { ApiFormQuestion } from "../../../services/interfaces/question.type";
import FormInput from "../../../components/FormInputs/FormInput";
import WarningDialog from "../../../components/WarningDialog";
import { MULTIPART_FILE_UPLOAD_SIZE } from "../../../common/constants";
import * as DocumentPicker from "expo-document-picker";
import { KeyboardAwareScrollView } from "react-native-keyboard-aware-scroll-view";
import { AttachmentsKeys } from "../../../services/queries/attachments.query";

type SearchParamType = {
  questionId: string;
  formId: string;
  language: string;
};

const FormQuestionnaire = () => {
  const { t } = useTranslation(["polling_station_form_wizard", "common"]);
  const { questionId, formId, language } = useLocalSearchParams<SearchParamType>();
  const queryClient = useQueryClient();

  const { data: uploadAttachmentProgress } = useUploadAttachmentProgressQuery();

  console.log("❌ Upload Progress", uploadAttachmentProgress);

  if (!questionId || !formId || !language) {
    return <Typography>Incorrect page params</Typography>;
  }

  const { activeElectionRound, selectedPollingStation } = useUserData();
  const [isOptionsSheetOpen, setIsOptionsSheetOpen] = useState(false);
  const [addingNote, setAddingNote] = useState(false);
  const [deletingAnswer, setDeletingAnswer] = useState(false);
  const [isPreparingFile, setIsPreparingFile] = useState(false);

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

  const { data: notes } = useNotesForQuestionId(
    activeElectionRound?.id,
    selectedPollingStation?.pollingStationId,
    formId,
    questionId,
  );

  const {
    control,
    handleSubmit,
    formState: { isDirty },
    setValue,
  } = useForm({
    defaultValues: setFormDefaultValues(questionId, answers?.[questionId]) as any,
  });

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
  }, [displayedQuestions]);

  const formTitle = useMemo(() => {
    return `${currentForm?.code} - ${currentForm?.name[language || currentForm.defaultLanguage]} (${language || currentForm?.defaultLanguage})`;
  }, [currentForm]);

  const { mutate: updateSubmission } = useFormSubmissionMutation({
    electionRoundId: activeElectionRound?.id,
    pollingStationId: selectedPollingStation?.pollingStationId,
    scopeId: `Submit_Answers_${activeElectionRound?.id}_${selectedPollingStation?.pollingStationId}_${formId}`,
  });

  const onSubmitAnswer = (formValues: any) => {
    const questionId = activeQuestion?.question.id as string;

    if (activeElectionRound?.id && selectedPollingStation?.pollingStationId && activeQuestion) {
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

      // Send to server only if the answer is different then the saved one
      if (isDirty) {
        // Find dependent questions for the current one and remove their answers
        currentForm?.questions
          ?.filter((q) => q.displayLogic?.parentQuestionId === questionId)
          .map((q) => q.id)
          .forEach((qId) => {
            if (updatedAnswers) delete updatedAnswers[qId];
          });

        updateSubmission({
          pollingStationId: selectedPollingStation?.pollingStationId,
          electionRoundId: activeElectionRound?.id,
          formId: currentForm?.id as string,
          answers: Object.values(updatedAnswers).filter(Boolean) as ApiFormAnswer[],
        });
      }

      const nextQuestion = findNextQuestion(activeQuestion.indexInAllQuestions, updatedAnswers);

      if (nextQuestion) {
        router.replace(
          `/form-questionnaire/${nextQuestion.id}?formId=${formId}&language=${language}`,
        );
      } else {
        return router.back();
      }
    }
  };

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
    answers: Record<string, ApiFormAnswer> | undefined,
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

  const onBackButtonPress = () => {
    if (currentForm?.questions && activeQuestion.indexInAllQuestions === 0) {
      return;
    }

    const prevQ = findPreviousQuestion(activeQuestion.indexInAllQuestions, answers);

    if (prevQ) {
      router.replace(`/form-questionnaire/${prevQ.id}?formId=${formId}&language=${language}`);
    }
  };

  const onClearForm = () => {
    if (selectedPollingStation?.pollingStationId && activeElectionRound?.id) {
      // Remove current answer
      const updatedAnswers = {
        ...answers,
      };

      delete updatedAnswers[questionId];

      // 1. Find dependent questions for the current one and remove their answers
      currentForm?.questions
        ?.filter((q) => q.displayLogic?.parentQuestionId === questionId)
        .forEach((q) => {
          if (updatedAnswers) delete updatedAnswers[q.id];
        });

      updateSubmission({
        pollingStationId: selectedPollingStation?.pollingStationId,
        electionRoundId: activeElectionRound?.id,
        formId: currentForm?.id as string,
        answers: Object.values(updatedAnswers).filter(Boolean) as ApiFormAnswer[],
      });
    }

    // TODO: @radulescuandrew maybe we can get rid of this, or at least shouldDirty: false. To be checked after API is ready
    setValue(activeQuestion?.question?.id, "", { shouldDirty: true });
    setDeletingAnswer(false);
  };

  if (isLoadingCurrentForm || isLoadingAnswers) {
    return <Typography>{t("loading", { ns: "common" })}</Typography>;
  }

  if (currentFormError || answersError) {
    return (
      <Screen preset="fixed">
        <Header
          title={`${questionId}`}
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
  const { uploadCameraOrMedia } = useCamera();

  const {
    mutate: addAttachmentMultipart,
    isPaused,
    isPending: isUploadingAttachments,
  } = useUploadAttachmentMutation(
    `Attachment_${questionId}_${selectedPollingStation?.pollingStationId}_${formId}_${questionId}`,
  );

  const handleCameraUpload = async (type: "library" | "cameraPhoto") => {
    await queryClient.setQueryData(AttachmentsKeys.addAttachments(), {
      progress: 0,
      status: "idle",
    });
    setIsPreparingFile(true);

    const cameraResult = await uploadCameraOrMedia(type);

    if (!cameraResult) {
      setIsPreparingFile(false);
      return;
    }

    if (
      activeElectionRound &&
      selectedPollingStation?.pollingStationId &&
      formId &&
      activeQuestion.question.id &&
      cameraResult.size
    ) {
      addAttachmentMultipart(
        {
          id: Crypto.randomUUID(),
          electionRoundId: activeElectionRound.id,
          pollingStationId: selectedPollingStation.pollingStationId,
          formId,
          questionId: activeQuestion.question.id,
          fileName: cameraResult.name,
          contentType: cameraResult.type,
          numberOfUploadParts: Math.ceil(cameraResult.size / MULTIPART_FILE_UPLOAD_SIZE), // Calculate the number of parts that will be sent to the S3 Bucket. It is +1 (thus we use ceil).
          filePath: cameraResult.uri,
        },
        {
          onError: () => console.log("🔴🔴🔴🔴🔴🔴🔴🔴🔴🔴ERORR🔴🔴🔴🔴🔴🔴🔴🔴🔴🔴"),
          onSettled: () => {
            setIsOptionsSheetOpen(false);
            setIsPreparingFile(false);
          },
        },
      );

      if (!onlineManager.isOnline()) {
        setIsOptionsSheetOpen(false);
        setIsPreparingFile(false);
      }
    }
  };

  const handleUploadAudio = async () => {
    await queryClient.setQueryData(AttachmentsKeys.addAttachments(), {
      progress: 0,
      status: "idle",
    });
    setIsPreparingFile(true);

    const doc = await DocumentPicker.getDocumentAsync({
      type: "audio/*",
      multiple: false,
    });

    console.log(doc);

    if (doc?.canceled) {
      console.log("canceling");
      setIsPreparingFile(false);
      return;
    }

    if (doc?.assets?.[0]) {
      const file = doc?.assets?.[0];

      const fileMetadata: FileMetadata = {
        name: file.name,
        type: file.mimeType || "audio/mpeg",
        uri: file.uri,
      };

      console.log("FileMetadata", fileMetadata);

      if (
        activeElectionRound &&
        selectedPollingStation?.pollingStationId &&
        formId &&
        activeQuestion.question.id &&
        fileMetadata.size
      ) {
        await addAttachmentMultipart(
          {
            id: Crypto.randomUUID(),
            electionRoundId: activeElectionRound.id,
            pollingStationId: selectedPollingStation.pollingStationId,
            formId,
            questionId: activeQuestion.question.id,
            fileName: fileMetadata.name,
            contentType: fileMetadata.type,
            numberOfUploadParts: Math.ceil(fileMetadata.size / MULTIPART_FILE_UPLOAD_SIZE),
            filePath: fileMetadata.uri,
          },
          {
            onError: () => console.log("🔴🔴🔴🔴🔴🔴🔴🔴🔴🔴ERORR🔴🔴🔴🔴🔴🔴🔴🔴🔴🔴"),
            onSettled: () => {
              setIsOptionsSheetOpen(false);
              setIsPreparingFile(false);
            },
          },
        );

        if (!onlineManager.isOnline()) {
          setIsOptionsSheetOpen(false);
          setIsPreparingFile(false);
        }
      }
    }
  };

  return (
    <Screen
      preset="fixed"
      backgroundColor="white"
      style={$screenStyle}
      contentContainerStyle={$containerStyle}
    >
      <Header
        title={`${formTitle}`}
        titleColor="white"
        barStyle="light-content"
        leftIcon={<Icon icon="chevronLeft" color="white" />}
        onLeftPress={() => router.back()}
      />

      <KeyboardAwareScrollView
        contentContainerStyle={{ flexGrow: 1 }}
        keyboardShouldPersistTaps="handled"
        // aproximate size of textarea
        extraHeight={200}
      >
        <YStack gap="$xxs" padding="$md">
          <XStack justifyContent="space-between">
            <Typography>{t("progress_bar.label")}</Typography>
            <Typography justifyContent="space-between">{`${activeQuestion?.indexInDisplayedQuestions + 1} /${displayedQuestions.length}`}</Typography>
          </XStack>
          <LinearProgress
            current={activeQuestion?.indexInDisplayedQuestions + 1}
            total={displayedQuestions?.length || 0}
          />

          <XStack
            justifyContent="flex-end"
            alignSelf="flex-end"
            paddingLeft="$md"
            paddingBottom="$md"
            pressStyle={{ opacity: 0.5 }}
            onPress={() => {
              Keyboard.dismiss();
              setDeletingAnswer(true);
            }}
          >
            <Typography color="$red10"> {t("progress_bar.clear_answer")}</Typography>
          </XStack>
          {/* delete answer button */}
          {deletingAnswer && (
            <WarningDialog
              title={t("warning_modal.question.title", { value: activeQuestion.question.code })}
              description={t("warning_modal.question.description")}
              actionBtnText={t("warning_modal.question.actions.clear")}
              cancelBtnText={t("warning_modal.question.actions.cancel")}
              action={onClearForm}
              onCancel={() => setDeletingAnswer(false)}
            />
          )}
        </YStack>
        <YStack paddingHorizontal="$md" paddingBottom="$md" justifyContent="center" flex={1}>
          <Controller
            key={activeQuestion?.question?.id}
            name={activeQuestion?.question?.id}
            rules={{ required: false }}
            control={control}
            render={({ field: { value, onChange } }) => {
              if (!activeQuestion) return <></>;

              const { question } = activeQuestion;
              switch (question.$questionType) {
                case "numberQuestion":
                  return (
                    <WizardFormInput
                      type="numeric"
                      label={`${question.code}. ${question.text[language]}`}
                      placeholder={question?.inputPlaceholder?.[language] || ""}
                      paragraph={question.helptext?.[language] || ""}
                      onChangeText={onChange}
                      value={value}
                      maxLength={10}
                      helper={t("form.max", {
                        value: 10,
                      })}
                    />
                  );
                case "textQuestion":
                  return (
                    <WizardFormInput
                      type="textarea"
                      label={`${question.code}. ${question.text[language]}`}
                      placeholder={question?.inputPlaceholder?.[language] || ""}
                      paragraph={question.helptext?.[language] || ""}
                      onChangeText={onChange}
                      maxLength={1024}
                      helper={t("form.max", {
                        value: 1024,
                      })}
                      value={value}
                    />
                  );
                case "dateQuestion":
                  return (
                    <WizardDateFormInput
                      label={`${question.code}. ${question.text[language]}`}
                      placeholder={t("form.date_placeholder")}
                      paragraph={question.helptext?.[language] || ""}
                      onChange={onChange}
                      value={value}
                    />
                  );
                case "singleSelectQuestion":
                  return (
                    <>
                      <WizardRadioFormInput
                        label={`${question.code}. ${question.text[language]}`}
                        paragraph={question.helptext?.[language] || ""}
                        options={question.options.map((option) => ({
                          id: option.id,
                          value: option.id,
                          label: option.text[language],
                        }))}
                        onValueChange={(radioValue) =>
                          onChange({ ...value, radioValue, textValue: null })
                        }
                        value={value.radioValue || ""}
                      />
                      {question.options.map((option) => {
                        if (option.isFreeText && option.id === value.radioValue) {
                          return (
                            <FormInput
                              key={option.id + "free"}
                              type="textarea"
                              marginTop="$md"
                              value={value.textValue || ""}
                              placeholder={t("form.text_placeholder")}
                              onChangeText={(textValue) => {
                                onChange({ ...value, textValue });
                              }}
                              maxLength={1024}
                              helper={t("form.max", {
                                value: 1024,
                              })}
                            />
                          );
                        }
                        return false;
                      })}
                    </>
                  );
                case "multiSelectQuestion":
                  return (
                    <WizardFormElement
                      key={question.id}
                      label={`${question.code}. ${question.text[language]}`}
                      paragraph={question.helptext?.[language] || ""}
                    >
                      {question.options.map((option) => {
                        const selections: Record<string, { optionId: string; text: string }> =
                          value || {};
                        return (
                          <YStack key={option.id}>
                            <CheckboxInput
                              marginBottom="$md"
                              id={option.id}
                              label={option.text[language]}
                              checked={selections[option.id]?.optionId === option.id}
                              onCheckedChange={(checked) => {
                                if (checked) {
                                  return onChange({
                                    ...selections,
                                    [option.id]: { optionId: option.id, text: null },
                                  });
                                } else {
                                  const { [option.id]: _toRemove, ...rest } = selections;
                                  return onChange(
                                    Object.values(rest).filter(Boolean).length > 0 ? rest : "",
                                  );
                                }
                              }}
                            />
                            {selections[option.id]?.optionId === option.id && option.isFreeText && (
                              <FormInput
                                type="textarea"
                                marginTop="$md"
                                value={selections[option.id]?.text}
                                placeholder={t("form.text_placeholder")}
                                onChangeText={(textValue) => {
                                  selections[option.id] = {
                                    optionId: option.id,
                                    text: textValue,
                                  };
                                  onChange(selections);
                                }}
                                maxLength={1024}
                                helper={t("form.max", {
                                  value: 1024,
                                })}
                              />
                            )}
                          </YStack>
                        );
                      })}
                    </WizardFormElement>
                  );
                case "ratingQuestion":
                  return (
                    <WizardRatingFormInput
                      type="single"
                      id={question.id}
                      label={`${question.code}. ${question.text[language]}`}
                      paragraph={question.helptext?.[language] || ""}
                      scale={question.scale}
                      onValueChange={onChange}
                      value={value}
                    />
                  );
              }
            }}
          />

          {/* notes section */}
          {notes && activeElectionRound?.id && selectedPollingStation?.pollingStationId && (
            <QuestionNotes // TODO: @luciatugui add loading and error state for Notes and Attachments
              notes={notes}
              electionRoundId={activeElectionRound.id}
              pollingStationId={selectedPollingStation.pollingStationId}
              formId={formId}
              questionId={questionId}
            />
          )}

          {/* attachments */}
          {activeElectionRound?.id && selectedPollingStation?.pollingStationId && formId && (
            <QuestionAttachments
              electionRoundId={activeElectionRound.id}
              pollingStationId={selectedPollingStation.pollingStationId}
              formId={formId}
              questionId={questionId}
            />
          )}

          <AddAttachment
            label={t("attachments.add")}
            marginTop="$sm"
            onPress={() => {
              Keyboard.dismiss();
              return setIsOptionsSheetOpen(true);
            }}
          />
        </YStack>
      </KeyboardAwareScrollView>

      <WizzardControls
        isFirstElement={activeQuestion?.indexInAllQuestions === 0}
        isLastElement={
          currentForm?.questions &&
          activeQuestion?.indexInAllQuestions === currentForm?.questions?.length - 1
        }
        isNextDisabled={false}
        onNextButtonPress={handleSubmit(onSubmitAnswer)}
        onPreviousButtonPress={onBackButtonPress}
      />
      {/* //todo: remove this once tamagui fixes sheet issue #2585 */}
      {(isOptionsSheetOpen || Platform.OS === "ios") && (
        <OptionsSheet
          open={isOptionsSheetOpen}
          setOpen={(open) => {
            setIsOptionsSheetOpen(open);
            addingNote && setAddingNote(false);
          }}
          isLoading={(isUploadingAttachments && !isPaused) || isPreparingFile}
          // seems that this behaviour is handled differently and the sheet will move with keyboard even if this props is set to false on android
          moveOnKeyboardChange={Platform.OS === "android"}
          disableDrag={addingNote}
        >
          {(isUploadingAttachments && !isPaused) || isPreparingFile ? (
            <MediaLoading uploadProgress={uploadAttachmentProgress as UploadAttachmentProgress} />
          ) : addingNote ? (
            <AddNoteSheetContent
              setAddingNote={setAddingNote}
              questionId={questionId}
              pollingStationId={selectedPollingStation?.pollingStationId as string}
              formId={formId}
              electionRoundId={activeElectionRound?.id}
              setIsOptionsSheetOpen={setIsOptionsSheetOpen}
            />
          ) : (
            <YStack paddingHorizontal="$sm" gap="$xxs">
              <Typography
                preset="body1"
                paddingVertical="$md"
                pressStyle={{ color: "$purple5" }}
                onPress={() => {
                  setAddingNote(true);
                }}
              >
                {t("attachments.menu.add_note")}
              </Typography>
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

export default FormQuestionnaire;

const MediaLoading = ({
  uploadProgress,
}: {
  uploadProgress: UploadAttachmentProgress | undefined;
}) => {
  const { t } = useTranslation("polling_station_form_wizard");

  const message = useMemo(() => {
    if (!uploadProgress) {
      return "";
    }
    switch (uploadProgress?.status) {
      case "starting":
        return t("attachments.upload.starting");
      case "compressing":
        return `Compressing progress ${uploadProgress.progress}%`;
      case "inprogress":
        return `${t("attachments.upload.progress")} ${uploadProgress.progress} %`;
      case "completed":
        return t("attachments.upload.completed");
      case "aborted":
        return t("attachments.upload.aborted");
      default:
        return "";
    }
  }, [uploadProgress]);

  return (
    <YStack alignItems="center" gap="$lg" paddingHorizontal="$lg">
      <Spinner size="large" color="$purple5" />
      <Typography preset="subheading" fontWeight="500" color="$purple5" minHeight="$lg">
        {message || ""}
      </Typography>
    </YStack>
  );
};
