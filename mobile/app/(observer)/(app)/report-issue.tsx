import React, { useMemo, useState } from "react";
import { XStack, YStack } from "tamagui";
import { Screen } from "../../../components/Screen";
import { Icon } from "../../../components/Icon";
import { router } from "expo-router";
import Header from "../../../components/Header";
import { useSafeAreaInsets } from "react-native-safe-area-context";
import Button from "../../../components/Button";
import FormInput from "../../../components/FormInputs/FormInput";
import Select from "../../../components/Select";
import { useUserData } from "../../../contexts/user/UserContext.provider";
import { Controller, useForm } from "react-hook-form";
import { PollingStationVisitVM } from "../../../common/models/polling-station.model";
import FormElement from "../../../components/FormInputs/FormElement";
import AddAttachment from "../../../components/AddAttachment";
import { Keyboard } from "react-native";
import OptionsSheet from "../../../components/OptionsSheet";
import { Typography } from "../../../components/Typography";
import { useAddQuickReport } from "../../../services/mutations/quick-report/add-quick-report.mutation";
import * as Crypto from "expo-crypto";
import { FileMetadata, useCamera } from "../../../hooks/useCamera";
import { useUploadAttachmentQuickReportMutation } from "../../../services/mutations/quick-report/add-attachment-quick-report.mutation";
import { QuickReportLocationType } from "../../../services/api/quick-report/post-quick-report.api";
import * as DocumentPicker from "expo-document-picker";
import { onlineManager, useMutationState, useQueryClient } from "@tanstack/react-query";
import Card from "../../../components/Card";
import { QuickReportKeys } from "../../../services/queries/quick-reports.query";
import * as Sentry from "@sentry/react-native";
import {
  addAttachmentQuickReportMultipartAbort,
  addAttachmentQuickReportMultipartComplete,
  AddAttachmentQuickReportStartAPIPayload,
} from "../../../services/api/quick-report/add-attachment-quick-report.api";
import { useTranslation } from "react-i18next";
import i18n from "../../../common/config/i18n";
import { KeyboardAwareScrollView } from "react-native-keyboard-aware-scroll-view";
import Toast from "react-native-toast-message";
import { uploadS3Chunk } from "../../../services/api/add-attachment.api";
// import { t } from "i18next";
import { MULTIPART_FILE_UPLOAD_SIZE } from "../../../common/constants";
import * as FileSystem from "expo-file-system";
import { Buffer } from "buffer";
import MediaLoading from "../../../components/MediaLoading";
import { useNetInfoContext } from "../../../contexts/net-info-banner/NetInfoContext";

const mapVisitsToSelectPollingStations = (visits: PollingStationVisitVM[] = []) => {
  const pollingStationsForSelect = visits.map((visit) => {
    return {
      id: visit.pollingStationId,
      value: visit.pollingStationId,
      label: `${visit.number} - ${visit.address}`,
    };
  });

  //   adding the 'other' and 'not related to a polling station' options
  pollingStationsForSelect.push(
    {
      id: "other",
      value: QuickReportLocationType.OtherPollingStation,
      label: i18n.t("form.polling_station_id.options.other", { ns: "report_new_issue" }),
    },
    {
      id: "not_related_to_polling_station",
      value: QuickReportLocationType.NotRelatedToAPollingStation,
      label: i18n.t("form.polling_station_id.options.not_related", { ns: "report_new_issue" }),
    },
  );
  return pollingStationsForSelect;
};

type ReportIssueFormType = {
  polling_station_id: string;
  polling_station_details: string;
  issue_title: string;
  issue_description: string;
};

const ReportIssue = () => {
  const insets = useSafeAreaInsets();
  const queryClient = useQueryClient();
  const { visits, activeElectionRound } = useUserData();
  const pollingStations = useMemo(() => mapVisitsToSelectPollingStations(visits), [visits]);
  const [optionsSheetOpen, setOptionsSheetOpen] = useState(false);
  const { t } = useTranslation("report_new_issue");
  const [isLoadingAttachment, setIsLoadingAttachment] = useState(false);
  const [isPreparingFile, setIsPreparingFile] = useState(false);
  const [uploadProgress, setUploadProgress] = useState("");
  const { isOnline } = useNetInfoContext();

  const [attachments, setAttachments] = useState<Array<{ fileMetadata: FileMetadata; id: string }>>(
    [],
  );

  const {
    mutate,
    isPending: isPendingAddQuickReport,
    isPaused: isPausedAddQuickReport,
  } = useAddQuickReport();

  const { mutateAsync: addAttachmentQReport } = useUploadAttachmentQuickReportMutation(
    `Quick_Report_${activeElectionRound?.id}}`,
  );

  const addAttachmentsMutationState = useMutationState({
    filters: { mutationKey: QuickReportKeys.addAttachment() },
    select: (mutation) => mutation.state,
  });

  const isUploadingAttachments = useMemo(
    () =>
      addAttachmentsMutationState.some(
        (mutation) => mutation.status === "pending" && !mutation.isPaused,
      ),
    [addAttachmentsMutationState],
  );

  const {
    control,
    handleSubmit,
    reset,
    watch,
    formState: { errors },
  } = useForm<ReportIssueFormType>({
    defaultValues: {
      polling_station_id: "",
      polling_station_details: "",
      issue_title: "",
      issue_description: "",
    },
  });

  const pollingStationIdWatch = watch("polling_station_id");

  const { uploadCameraOrMedia } = useCamera();

  const onCompressionProgress = (progress: number) => {
    setUploadProgress(`${t("upload.compressing")} ${Math.ceil(progress * 100)}%`);
  };

  const handleCameraUpload = async (type: "library" | "cameraPhoto" | "cameraVideo") => {
    setIsPreparingFile(true);
    setUploadProgress(t("upload.preparing"));

    const cameraResult = await uploadCameraOrMedia(type, onCompressionProgress);

    if (!cameraResult || !activeElectionRound) {
      setUploadProgress("");
      setIsPreparingFile(false);
      return;
    }

    setOptionsSheetOpen(false);
    setAttachments((attachments) => [
      ...attachments,
      { fileMetadata: cameraResult, id: Crypto.randomUUID() },
    ]);
    setIsPreparingFile(false);
  };

  const handleUploadAudio = async () => {
    setIsPreparingFile(true);
    setUploadProgress(t("upload.preparing"));
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

      setOptionsSheetOpen(false);
      setAttachments((attachments) => [...attachments, { fileMetadata, id: Crypto.randomUUID() }]);
      setIsPreparingFile(false);
    } else {
      // Cancelled
    }

    setIsPreparingFile(false);
  };

  const handleChunkUpload = async (
    filePath: string,
    uploadUrls: Record<string, string>,
    uploadId: string,
    attachmentId: string,
    quickReportId: string,
    uploadedPartsNo: number,
    totalParts: number,
  ) => {
    try {
      let etags: Record<number, string> = {};
      const urls = Object.values(uploadUrls);
      for (const [index, url] of urls.entries()) {
        const chunk = await FileSystem.readAsStringAsync(filePath, {
          length: MULTIPART_FILE_UPLOAD_SIZE,
          position: index * MULTIPART_FILE_UPLOAD_SIZE,
          encoding: FileSystem.EncodingType.Base64,
        });
        const buffer = Buffer.from(chunk, "base64");
        const data = await uploadS3Chunk(url, buffer);
        setUploadProgress(
          `${t("upload.progress")} ${Math.ceil(((uploadedPartsNo + index) / totalParts) * 100)}%`,
        );
        etags = { ...etags, [index + 1]: data.ETag };
      }

      if (activeElectionRound?.id) {
        await addAttachmentQuickReportMultipartComplete({
          uploadId,
          etags,
          electionRoundId: activeElectionRound?.id,
          id: attachmentId,
          quickReportId,
        });
      }
    } catch (err) {
      Sentry.captureException(err, { data: activeElectionRound });
      if (activeElectionRound?.id) {
        setUploadProgress(t("upload.aborted"));
        await addAttachmentQuickReportMultipartAbort({
          id: attachmentId,
          uploadId,
          electionRoundId: activeElectionRound.id,
          quickReportId,
        });
      }
    } finally {
      if (activeElectionRound?.id) {
        queryClient.invalidateQueries({
          queryKey: QuickReportKeys.byElectionRound(activeElectionRound.id),
        });
      }
    }
  };

  const onSubmit = async (formData: ReportIssueFormType) => {
    if (!visits || !activeElectionRound) {
      return;
    }

    let quickReportLocationType = QuickReportLocationType.VisitedPollingStation;
    let pollingStationId: string | null = formData.polling_station_id;

    if (
      [
        QuickReportLocationType.OtherPollingStation,
        QuickReportLocationType.NotRelatedToAPollingStation,
      ].includes(pollingStationId as QuickReportLocationType)
    ) {
      quickReportLocationType = pollingStationId as QuickReportLocationType;
      pollingStationId = null;
    }

    const uuid = Crypto.randomUUID();

    // Use the attachments to optimistically update the UI
    const optimisticAttachments: AddAttachmentQuickReportStartAPIPayload[] = [];

    if (attachments.length > 0) {
      setOptionsSheetOpen(true);
      setIsLoadingAttachment(true);
      try {
        const totalParts = attachments.reduce((acc, attachment) => {
          return acc + Math.ceil(attachment.fileMetadata.size! / MULTIPART_FILE_UPLOAD_SIZE);
        }, 0);
        let uploadedPartsNo = 0;
        // Upload each attachment
        setUploadProgress(`${t("upload.starting")}`);
        for (const [, attachment] of attachments.entries()) {
          const payload: AddAttachmentQuickReportStartAPIPayload = {
            id: attachment.id,
            fileName: attachment.fileMetadata.name,
            filePath: attachment.fileMetadata.uri,
            contentType: attachment.fileMetadata.type,
            numberOfUploadParts: Math.ceil(
              attachment.fileMetadata.size! / MULTIPART_FILE_UPLOAD_SIZE,
            ),
            electionRoundId: activeElectionRound.id,
            quickReportId: uuid,
          };

          const data = await addAttachmentQReport(payload);
          await handleChunkUpload(
            attachment.fileMetadata.uri,
            data.uploadUrls,
            data.uploadId,
            attachment.id,
            uuid,
            uploadedPartsNo,
            totalParts,
          );
          uploadedPartsNo += payload.numberOfUploadParts;
          optimisticAttachments.push(payload);
        }
        setUploadProgress(t("upload.completed"));
      } catch (err) {
        Sentry.captureException(err);
      }
    }

    mutate(
      {
        id: uuid,
        electionRoundId: activeElectionRound?.id,
        description: formData.issue_description,
        title: formData.issue_title,
        quickReportLocationType,
        pollingStationDetails: formData.polling_station_details,
        ...(pollingStationId ? { pollingStationId } : {}),
        attachments: optimisticAttachments,
      },
      {
        onError: () => {
          Toast.show({
            type: "error",
            text2: t("form.error"),
          });
        },
        onSuccess: () => {
          setIsPreparingFile(false);
          setOptionsSheetOpen(false);
          router.back();
        },
      },
    );

    if (!onlineManager.isOnline()) {
      router.back();
    }
  };

  const removeAttachmentLocal = (id: string): void => {
    setAttachments((attachments) => attachments.filter((attachment) => attachment.id !== id));
  };

  const handleOnShowAttachementSheet = () => {
    if (isOnline) {
      setOptionsSheetOpen(true);
    } else {
      Toast.show({
        type: "error",
        text2: t("upload.offline"),
        visibilityTime: 5000,
        text2Style: { textAlign: "center" },
      });
    }
  };

  return (
    <>
      <Screen
        preset="scroll"
        backgroundColor="white"
        keyboardShouldPersistTaps="never"
        ScrollViewProps={{
          stickyHeaderIndices: [0],
          bounces: false,
          keyboardShouldPersistTaps: "handled",
        }}
      >
        <Header
          title={t("title")}
          titleColor="white"
          barStyle="light-content"
          leftIcon={<Icon icon="chevronLeft" color="white" />}
          onLeftPress={() => router.back()}
        />
        <KeyboardAwareScrollView contentContainerStyle={{ flexGrow: 1 }}>
          <YStack paddingVertical="$lg" paddingHorizontal="$md" flex={1}>
            {/* questions container */}

            <YStack gap="$lg">
              <Controller
                key="polling_station_id"
                name="polling_station_id"
                control={control}
                rules={{
                  required: {
                    value: true,
                    message: t("form.polling_station_id.required"),
                  },
                }}
                render={({ field: { onChange, value } }) => (
                  <>
                    {/* select polling station */}
                    <FormElement
                      title={t("form.polling_station_id.label")}
                      error={errors.polling_station_id?.message}
                    >
                      <Select
                        value={value}
                        options={pollingStations}
                        placeholder={t("form.polling_station_id.placeholder")}
                        onValueChange={onChange}
                        error={errors.polling_station_id?.message}
                      />
                    </FormElement>
                  </>
                )}
              />

              {/* polling station details */}
              {pollingStationIdWatch === QuickReportLocationType.OtherPollingStation && (
                <Controller
                  key="polling_station_details"
                  name="polling_station_details"
                  control={control}
                  rules={{
                    required: { value: true, message: t("form.polling_station_details.required") },
                    maxLength: {
                      value: 1024,
                      message: t("form.polling_station_details.max", { value: 1024 }),
                    },
                  }}
                  render={({ field: { onChange, value } }) => (
                    <FormInput
                      title={t("form.polling_station_details.label")}
                      type="textarea"
                      placeholder={t("form.polling_station_details.placeholder")}
                      value={value}
                      onChangeText={onChange}
                      error={errors.polling_station_details?.message}
                    />
                  )}
                />
              )}

              {/* issue title */}
              <Controller
                key="issue_title"
                name="issue_title"
                control={control}
                rules={{
                  required: { value: true, message: t("form.issue_title.required") },
                  maxLength: {
                    value: 1024,
                    message: t("form.issue_title.max", { value: 1024 }),
                  },
                }}
                render={({ field: { onChange, value } }) => (
                  <FormInput
                    title={t("form.issue_title.label")}
                    placeholder={t("form.issue_title.placeholder")}
                    type="text"
                    value={value}
                    onChangeText={onChange}
                    error={errors.issue_title?.message}
                  />
                )}
              />

              {/* issue description */}
              <Controller
                key="issue_description"
                name="issue_description"
                control={control}
                rules={{
                  required: { value: true, message: t("form.issue_description.required") },
                  maxLength: {
                    value: 10000,
                    message: t("form.issue_description.max", { value: 10000 }),
                  },
                }}
                render={({ field: { onChange, value } }) => (
                  <FormInput
                    title={t("form.issue_description.label")}
                    type="textarea"
                    placeholder={t("form.issue_description.placeholder")}
                    value={value}
                    onChangeText={onChange}
                    error={errors.issue_description?.message}
                  />
                )}
              />
              {attachments.length ? (
                <YStack gap="$xxs">
                  <Typography fontWeight="500">{t("media.heading")}</Typography>
                  <YStack gap="$xxs">
                    {attachments.map((attachment) => {
                      return (
                        <Card
                          padding="$0"
                          paddingLeft="$md"
                          key={attachment.id}
                          flexDirection="row"
                          justifyContent="space-between"
                          alignItems="center"
                        >
                          <Typography
                            preset="body1"
                            fontWeight="700"
                            maxWidth="85%"
                            numberOfLines={1}
                          >
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
                label={t("media.add")}
                onPress={() => {
                  Keyboard.dismiss();
                  handleOnShowAttachementSheet();
                }}
              />
            </YStack>
          </YStack>
        </KeyboardAwareScrollView>

        <OptionsSheet
          open={optionsSheetOpen}
          setOpen={setOptionsSheetOpen}
          isLoading={isLoadingAttachment || isPreparingFile}
        >
          {isLoadingAttachment || isPreparingFile ? (
            <MediaLoading progress={uploadProgress} />
          ) : (
            <YStack paddingHorizontal="$sm">
              <Typography
                onPress={handleCameraUpload.bind(null, "library")}
                preset="body1"
                paddingVertical="$md"
                pressStyle={{ color: "$purple5" }}
              >
                {t("media.menu.load")}
              </Typography>
              <Typography
                onPress={handleCameraUpload.bind(null, "cameraPhoto")}
                preset="body1"
                paddingVertical="$md"
                pressStyle={{ color: "$purple5" }}
              >
                {t("media.menu.take_picture")}
              </Typography>
              <Typography
                onPress={handleUploadAudio.bind(null)}
                preset="body1"
                paddingVertical="$md"
                pressStyle={{ color: "$purple5" }}
              >
                {t("media.menu.upload_audio")}
              </Typography>
            </YStack>
          )}
        </OptionsSheet>
      </Screen>

      <XStack
        backgroundColor="white"
        justifyContent="space-between"
        alignItems="center"
        paddingTop="$xs"
        paddingBottom={insets.bottom + 10}
        paddingHorizontal="$md"
        elevation={2}
        gap="$sm"
      >
        {/* this will reset form to defaultValues */}
        <Button preset="chromeless" onPress={() => reset()}>
          {t("form.clear")}
        </Button>
        <Button
          flex={1}
          height="100%"
          textStyle={{ textAlign: "center" }}
          onPress={handleSubmit(onSubmit)}
          disabled={(isPendingAddQuickReport && !isPausedAddQuickReport) || isUploadingAttachments}
        >
          {(!isPendingAddQuickReport && !isPausedAddQuickReport) || !isUploadingAttachments
            ? t("form.submit")
            : t("form.loading")}
        </Button>
      </XStack>
    </>
  );
};

export default ReportIssue;
