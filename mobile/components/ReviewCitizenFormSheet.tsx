import React, { Dispatch, SetStateAction, useMemo, useRef, useState } from "react";
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
import { Separator, Sheet, Spinner, YStack } from "tamagui";
import { getAnswerDisplay } from "../common/utils/answers";
import { ApiFormQuestion } from "../services/interfaces/question.type";
import { useTranslation } from "react-i18next";
import * as Crypto from "expo-crypto";
import { usePostCitizenFormMutation } from "../services/mutations/citizen/post-citizen-form.mutation";
import { useCitizenUserData } from "../contexts/citizen-user/CitizenUserContext.provider";
import { useRouter } from "expo-router";
import Toast from "react-native-toast-message";
import { FileMetadata } from "../hooks/useCamera";
import { MULTIPART_FILE_UPLOAD_SIZE, MUTATION_SCOPE_DO_NOT_HYDRATE } from "../common/constants";
import {
  useUploadAttachmentCitizenAbortMutation,
  useUploadAttachmentCitizenCompleteMutation,
  useUploadAttachmentCitizenMutation,
} from "../services/mutations/citizen/add-attachment-citizen.mutation";
import { AddAttachmentCitizenStartAPIPayload } from "../services/api/citizen/attachments.api";
import * as FileSystem from "expo-file-system";
import { Buffer } from "buffer";
import * as Sentry from "@sentry/react-native";
import MediaLoading from "./MediaLoading";
import {
  removeMutationByScopeId,
  useUploadS3ChunkMutation,
} from "../services/mutations/attachments/add-attachment.mutation";
import { useNetInfoContext } from "../contexts/net-info-banner/NetInfoContext";
import { useQueryClient } from "@tanstack/react-query";
import { AttachmentData } from "../services/api/add-attachment.api";

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
  attachments,
  setAttachments,
  setIsReviewSheetOpen,
  selectedLocationId,
  language,
}: {
  currentForm: FormAPIModel | undefined;
  answers: Record<string, ApiFormAnswer | undefined> | undefined;
  questions: ApiFormQuestion[] | undefined;
  attachments: Record<string, AttachmentData[]> | undefined;
  setAttachments: Dispatch<SetStateAction<Record<string, AttachmentData[]>>>
  setIsReviewSheetOpen: Dispatch<SetStateAction<boolean>>;
  selectedLocationId: string;
  language: string;
}) {
  const cancelRef = useRef<boolean>(false);
  const { t } = useTranslation("citizen_form");
  const insets = useSafeAreaInsets();
  const router = useRouter();
  const { isOnline } = useNetInfoContext();
  const queryClient = useQueryClient();

  const { mutate: postCitizenForm, isPending } = usePostCitizenFormMutation();
  const { mutateAsync: uploadS3Chunk } = useUploadS3ChunkMutation();
  const { mutateAsync: addAttachmentCitizenComplete } =
    useUploadAttachmentCitizenCompleteMutation();
  const { mutateAsync: addAttachmentCitizenAbort } = useUploadAttachmentCitizenAbortMutation();

  const { selectedElectionRound } = useCitizenUserData();

  const [isUploading, setIsUploading] = useState<boolean>(false);
  const [uploadProgress, setUploadProgress] = useState<string>("");
  const [responseId, setResponseId] = useState<string | null>(null);

  const { mutateAsync: addAttachmentCitizen } = useUploadAttachmentCitizenMutation();

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

  const handleSubmit = async () => {
    if (!currentForm || !selectedElectionRound || !answers) {
      return;
    }

    const citizenReportId = Crypto.randomUUID();

    postCitizenForm(
      {
        electionRoundId: selectedElectionRound,
        citizenReportId,
        formId: currentForm.id,
        locationId: selectedLocationId,
        answers: Object.values(answers).filter(Boolean) as ApiFormAnswer[],
      },
      {
        onSuccess: async (response) => {
          setResponseId(response.id);
          await uploadAttachments(citizenReportId);


          if (cancelRef.current === true) {
            return;
          } else {
            router.replace(
              `/citizen/main/form/success?formId=${currentForm.id}&submissionId=${response.id}`,
            );
          }

          setIsUploading(false);
        },
        onError: (error) => {
          Sentry.captureException(error);
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

  const uploadAttachments = async (citizenReportId: string) => {
    if (!currentForm || !selectedElectionRound || !answers) {
      return;
    }

    cancelRef.current = false;

    if (attachments && Object.keys(attachments).length > 0) {
      setIsUploading(true);
      const attachmentArray: { questionId: string; fileMetadata: FileMetadata; id: string, uploaded: boolean }[] =
        Object.entries(attachments)
          .map(([questionId, attachments]) => attachments.map((a) => ({ ...a, questionId })))
          .flat();
      try {
        const totalParts = attachmentArray.reduce((acc, attachment) => {
          return acc + Math.ceil(attachment.fileMetadata.size! / MULTIPART_FILE_UPLOAD_SIZE);
        }, 0);
        let uploadedPartsNo = 0;
        // Upload each attachment
        setUploadProgress(`${t("success.title")}\n${t("attachments.upload.starting")}`);
        for (const attachment of attachmentArray) {
          try {
            if (cancelRef.current) {
              throw new Error("Upload aborted");
            }

            if (attachment.uploaded) {
              continue;
            }

            const payload: AddAttachmentCitizenStartAPIPayload = {
              id: attachment.id,
              fileName: attachment.fileMetadata.name,
              contentType: attachment.fileMetadata.type,
              numberOfUploadParts: Math.ceil(
                attachment.fileMetadata.size! / MULTIPART_FILE_UPLOAD_SIZE,
              ),
              electionRoundId: selectedElectionRound,
              citizenReportId,
              formId: currentForm.id,
              questionId: attachment.questionId,
            };

            const data = await addAttachmentCitizen(payload);
            await handleChunkUpload(
              attachment.fileMetadata.uri,
              data.uploadUrls,
              data.uploadId,
              attachment.id,
              citizenReportId,
              currentForm.id,
              attachment.questionId,
              uploadedPartsNo,
              totalParts,
            );
            uploadedPartsNo += payload.numberOfUploadParts;
          } catch (err) {
            Sentry.captureException(err);
          }
        }
        setUploadProgress(t("attachments.upload.completed"));
      } catch (err) {
        Sentry.captureException(err);
      } finally {
        setIsUploading(false);
      }
    }
  };

  const handleChunkUpload = async (
    filePath: string,
    uploadUrls: Record<string, string>,
    uploadId: string,
    attachmentId: string,
    citizenReportId: string,
    formId: string,
    questionId: string,
    uploadedPartsNo: number,
    totalParts: number,
  ) => {
    try {
      if (cancelRef.current === true) {
        throw new Error("Upload aborted");
      }

      let etags: Record<number, string> = {};
      const urls = Object.values(uploadUrls);
      for (const [index, url] of urls.entries()) {
        if (cancelRef.current) {
          throw new Error("Upload aborted");
        }
        const chunk = await FileSystem.readAsStringAsync(filePath, {
          length: MULTIPART_FILE_UPLOAD_SIZE,
          position: index * MULTIPART_FILE_UPLOAD_SIZE,
          encoding: FileSystem.EncodingType.Base64,
        });
        const buffer = Buffer.from(chunk, "base64");

        const data = await uploadS3Chunk({ url, chunk: buffer });

        setUploadProgress(
          `${t("attachments.upload.progress")} ${Math.ceil(((uploadedPartsNo + index) / totalParts) * 100)}%`,
        );
        etags = { ...etags, [index + 1]: data.ETag };
      }

      if (selectedElectionRound) {
        await addAttachmentCitizenComplete({
          uploadId,
          etags,
          electionRoundId: selectedElectionRound,
          id: attachmentId,
          citizenReportId,
        });
        setAttachments((attachments) =>
        ({
          ...attachments,
          [questionId]: attachments[questionId].map(
            (attachment) => attachment.id === attachmentId ? { ...attachment, uploaded: true } : attachment)
        })
        );
      }
    } catch (err) {
      Sentry.captureException(err, {
        data: { selectedElectionRound, citizenReportId, formId, questionId },
      });
      if (selectedElectionRound) {
        setUploadProgress(t("attachments.upload.aborted"));
        await addAttachmentCitizenAbort({
          id: attachmentId,
          uploadId,
          electionRoundId: selectedElectionRound,
          citizenReportId,
        });
      }
    }
  };

  const onAbortUpload = () => {
    cancelRef.current = true;
    setUploadProgress("");
    setIsUploading(false);
    setIsReviewSheetOpen(false);
    removeMutationByScopeId(queryClient, MUTATION_SCOPE_DO_NOT_HYDRATE);
    if (responseId && currentForm) {
      router.replace(
        `/citizen/main/form/success?formId=${currentForm.id}&submissionId=${responseId}`,
      );
    }
  };

  return (
    <Sheet
      modal
      open
      zIndex={100_001}
      snapPoints={isUploading ? (isOnline ? [25] : [65]) : [90]}
      dismissOnSnapToBottom={!isPending && !isUploading}
      dismissOnOverlayPress={!isPending && !isUploading}
      onOpenChange={(open: boolean) => {
        if (!open) {
          setIsReviewSheetOpen(false);
        }
      }}
    >
      <Sheet.Overlay />
      <Sheet.Frame>
        {isUploading && attachments ? (
          <YStack flex={1} justifyContent="center" alignItems="center">
            <MediaLoading
              progress={uploadProgress}
              isUploading={isUploading}
              uploadedAttachments={Object.entries(attachments).map(([, attachments]) => attachments.filter((a) => a.uploaded)).flat().length}
              onAbortUpload={onAbortUpload}
              confirmAbort
            />
          </YStack>
        ) : (
          <>
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
                      <Typography fontWeight="500">{question.text[language]}</Typography>
                      {mappedAnswers && mappedAnswers[question.id] && (
                        <Typography marginTop="$xs" color="$gray5">
                          {getAnswerDisplay(mappedAnswers[question.id] as ApiFormAnswer, true)}
                        </Typography>
                      )}
                      {attachments && attachments[question.id] ? (
                        <YStack gap="$xxs" paddingTop="$lg">
                          <Typography fontWeight="500">
                            {t("attachments.heading")}: {attachments[question.id].length}
                          </Typography>
                        </YStack>
                      ) : (
                        false
                      )}
                      <Separator marginTop="$xs" />
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
          </>
        )}
      </Sheet.Frame>
    </Sheet>
  );
}
