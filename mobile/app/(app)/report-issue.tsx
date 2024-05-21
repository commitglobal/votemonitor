import React, { useMemo, useState } from "react";
import { XStack, YStack } from "tamagui";
import { Screen } from "../../components/Screen";
import { Icon } from "../../components/Icon";
import { router } from "expo-router";
import Header from "../../components/Header";
import { useSafeAreaInsets } from "react-native-safe-area-context";
import Button from "../../components/Button";
import FormInput from "../../components/FormInputs/FormInput";
import Select from "../../components/Select";
import { useUserData } from "../../contexts/user/UserContext.provider";
import { Controller, useForm } from "react-hook-form";
import { PollingStationVisitVM } from "../../common/models/polling-station.model";
import FormElement from "../../components/FormInputs/FormElement";
import AddAttachment from "../../components/AddAttachment";
import { Keyboard } from "react-native";
import OptionsSheet from "../../components/OptionsSheet";
import { Typography } from "../../components/Typography";
import { useAddQuickReport } from "../../services/mutations/quick-report/add-quick-report.mutation";
import * as Crypto from "expo-crypto";
import { FileMetadata, useCamera } from "../../hooks/useCamera";
import { addAttachmentQuickReportMutation } from "../../services/mutations/quick-report/add-attachment-quick-report.mutation";
import { QuickReportLocationType } from "../../services/api/quick-report/post-quick-report.api";
import * as DocumentPicker from "expo-document-picker";
import { onlineManager, useMutationState, useQueryClient } from "@tanstack/react-query";
import Card from "../../components/Card";
import { QuickReportKeys } from "../../services/queries/quick-reports.query";
import * as Sentry from "@sentry/react-native";
import { AddAttachmentQuickReportAPIPayload } from "../../services/api/quick-report/add-attachment-quick-report.api";
import { useTranslation } from "react-i18next";

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
      label: "Other polling station",
    },
    {
      id: "not_related_to_polling_station",
      value: QuickReportLocationType.NotRelatedToAPollingStation,
      label: "Not related to a polling station",
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

  const [attachments, setAttachments] = useState<Array<{ fileMetadata: FileMetadata; id: string }>>(
    [],
  );

  const {
    mutate,
    isPending: isPendingAddQuickReport,
    isPaused: isPausedAddQuickReport,
  } = useAddQuickReport();
  const { mutateAsync: addAttachmentQReport } = addAttachmentQuickReportMutation();

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

  const handleCameraUpload = async (type: "library" | "cameraPhoto" | "cameraVideo") => {
    const cameraResult = await uploadCameraOrMedia(type);

    if (!cameraResult || !activeElectionRound) {
      return;
    }

    setOptionsSheetOpen(false);
    setAttachments((attachments) => [
      ...attachments,
      { fileMetadata: cameraResult, id: Crypto.randomUUID() },
    ]);
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
      };

      setOptionsSheetOpen(false);
      setAttachments((attachments) => [...attachments, { fileMetadata, id: Crypto.randomUUID() }]);
    } else {
      // Cancelled
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
    const optimisticAttachments: AddAttachmentQuickReportAPIPayload[] = [];

    if (attachments.length > 0) {
      const attachmentsMutations = attachments.map(
        ({ fileMetadata, id }: { fileMetadata: FileMetadata; id: string }) => {
          const payload: AddAttachmentQuickReportAPIPayload = {
            id,
            fileMetadata,
            electionRoundId: activeElectionRound.id,
            quickReportId: uuid,
          };
          optimisticAttachments.push(payload);
          return addAttachmentQReport(payload);
        },
      );
      try {
        Promise.all(attachmentsMutations).then(() => {
          queryClient.invalidateQueries({
            queryKey: QuickReportKeys.byElectionRound(activeElectionRound.id),
          });
        });
      } catch (err) {
        Sentry.captureMessage("Failed to upload some attachments");
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
        onError: (err) => {
          // TODO: Display toast
          console.log("❌❌❌❌❌❌❌❌❌❌");
          console.log(err);
        },
        onSuccess: () => {
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
          title={t("header.title")}
          titleColor="white"
          barStyle="light-content"
          leftIcon={<Icon icon="chevronLeft" color="white" />}
          onLeftPress={() => router.back()}
        />
        <YStack paddingVertical="$lg" paddingHorizontal="$md">
          {/* questions container */}

          <YStack gap="$lg">
            <Controller
              key="polling_station_id"
              name="polling_station_id"
              control={control}
              rules={{
                required: {
                  value: true,
                  message: "This field is required.",
                },
              }}
              render={({ field: { onChange, value } }) => (
                <>
                  {/* select polling station */}
                  <FormElement title="Polling station *" error={errors.polling_station_id?.message}>
                    <Select
                      value={value}
                      options={pollingStations}
                      placeholder="Select polling station"
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
                  required: { value: true, message: "This field is required." },
                  maxLength: {
                    value: 1024,
                    // todo: translation
                    message: "Input cannot exceed 1024 characters",
                  },
                }}
                render={({ field: { onChange, value } }) => (
                  <FormInput
                    title="Polling station details *"
                    type="textarea"
                    placeholder="Please write here some identification details for this polling station (such as address, name, number, etc.)"
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
                required: { value: true, message: "This field is required." },
                maxLength: {
                  value: 1024,
                  message: "Input cannot exceed 1024 characters",
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
                required: { value: true, message: "This field is required." },
                maxLength: {
                  value: 10000,
                  message: "Input cannot exceed 10,000 characters",
                },
              }}
              render={({ field: { onChange, value } }) => (
                <FormInput
                  title={t("form.description.label")}
                  type="textarea"
                  placeholder={t("form.description.placeholder")}
                  value={value}
                  onChangeText={onChange}
                  error={errors.issue_description?.message}
                />
              )}
            />
            {attachments.length ? (
              <YStack gap="$xxs">
                <Typography fontWeight="500">{t("media.title")}</Typography>
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
              label={t("form.actions.add_media")}
              onPress={() => {
                Keyboard.dismiss();
                setOptionsSheetOpen(true);
              }}
            />
          </YStack>
        </YStack>

        <OptionsSheet open={optionsSheetOpen} setOpen={setOptionsSheetOpen}>
          <YStack paddingHorizontal="$sm">
            <Typography
              onPress={handleCameraUpload.bind(null, "library")}
              preset="body1"
              paddingVertical="$md"
              pressStyle={{ color: "$purple5" }}
            >
              {t("form.actions.load")}
            </Typography>
            <Typography
              onPress={handleCameraUpload.bind(null, "cameraPhoto")}
              preset="body1"
              paddingVertical="$md"
              pressStyle={{ color: "$purple5" }}
            >
              {t("form.actions.take_picture")}
            </Typography>
            <Typography
              onPress={handleCameraUpload.bind(null, "cameraVideo")}
              preset="body1"
              paddingVertical="$md"
              pressStyle={{ color: "$purple5" }}
            >
              {t("form.actions.record_video")}
            </Typography>
            <Typography
              onPress={handleUploadAudio.bind(null)}
              preset="body1"
              paddingVertical="$md"
              pressStyle={{ color: "$purple5" }}
            >
              {t("form.actions.upload_audio")}
            </Typography>
          </YStack>
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
          {t("form.actions.clear")}
        </Button>
        <Button
          flex={1}
          onPress={handleSubmit(onSubmit)}
          disabled={(isPendingAddQuickReport && !isPausedAddQuickReport) || isUploadingAttachments}
        >
          {(!isPendingAddQuickReport && !isPausedAddQuickReport) || !isUploadingAttachments
            ? t("form.actions.submit")
            : t("form.actions.loading")}
        </Button>
      </XStack>
    </>
  );
};

export default ReportIssue;
