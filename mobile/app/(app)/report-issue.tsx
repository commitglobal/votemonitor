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
  polling_station: { details: string; id: string };
  issue_title: string;
  issue_description: string;
};

const ReportIssue = () => {
  const insets = useSafeAreaInsets();
  const queryClient = useQueryClient();
  const { visits, activeElectionRound } = useUserData();
  const pollingStations = useMemo(() => mapVisitsToSelectPollingStations(visits), [visits]);
  const [optionsSheetOpen, setOptionsSheetOpen] = useState(false);

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
    formState: { errors },
  } = useForm<ReportIssueFormType>({
    defaultValues: {
      polling_station: { details: "", id: "" },
      issue_title: "",
      issue_description: "",
    },
  });

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
    let pollingStationId: string | null = formData.polling_station.id;

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
        pollingStationDetails: formData.polling_station.details,
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
          title="Report new issue"
          titleColor="white"
          barStyle="light-content"
          leftIcon={<Icon icon="chevronLeft" color="white" />}
          onLeftPress={() => router.back()}
        />
        <YStack paddingVertical="$lg" paddingHorizontal="$md">
          {/* questions container */}

          {/* //TODO: is the polling station required? */}
          <YStack gap="$lg">
            <Controller
              key="polling_station"
              name="polling_station"
              control={control}
              rules={{
                validate: ({ id, details }) => {
                  if (id === QuickReportLocationType.OtherPollingStation && !details)
                    return "This field is required";
                },
              }}
              render={({ field: { onChange, value = { id: "", details: "" } } }) => (
                <>
                  {/* select polling station */}
                  <FormElement title="Polling station">
                    <Select
                      value={value.id}
                      options={pollingStations}
                      placeholder="Select polling station"
                      onValueChange={(id) => onChange({ ...value, id, details: "" })}
                      // onOpenChange={(open) => open && Keyboard.dismiss()}
                    />
                  </FormElement>
                  {/* polling station details */}
                  {value.id === QuickReportLocationType.OtherPollingStation && (
                    <FormInput
                      title="Polling station details *"
                      type="textarea"
                      placeholder="Please write here some identification details for this polling station (such as address, name, number, etc.)"
                      value={value.details}
                      onChangeText={(details) => onChange({ ...value, details })}
                      error={errors.polling_station?.message}
                    />
                  )}
                </>
              )}
            />

            {/* issue title */}
            <Controller
              key="issue_title"
              name="issue_title"
              control={control}
              rules={{
                required: { value: true, message: "This field is required." },
              }}
              render={({ field: { onChange, value } }) => (
                <FormInput
                  title="Title of issue *"
                  placeholder="Write a title for this issue."
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
              }}
              render={({ field: { onChange, value } }) => (
                <FormInput
                  title="Description *"
                  type="textarea"
                  placeholder="Describe the situation in detail here."
                  value={value}
                  onChangeText={onChange}
                  error={errors.issue_description?.message}
                />
              )}
            />
            {attachments.length ? (
              <YStack marginTop="$lg" gap="$xxs">
                <Typography fontWeight="500">Uploaded media</Typography>
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
              label="Add Media"
              paddingVertical="$xxs"
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
              Load from gallery
            </Typography>
            <Typography
              onPress={handleCameraUpload.bind(null, "cameraPhoto")}
              preset="body1"
              paddingVertical="$md"
              pressStyle={{ color: "$purple5" }}
            >
              Take a photo
            </Typography>
            <Typography
              onPress={handleCameraUpload.bind(null, "cameraVideo")}
              preset="body1"
              paddingVertical="$md"
              pressStyle={{ color: "$purple5" }}
            >
              Record a video
            </Typography>
            <Typography
              onPress={handleUploadAudio.bind(null)}
              preset="body1"
              paddingVertical="$md"
              pressStyle={{ color: "$purple5" }}
            >
              Upload audio file
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
          Clear
        </Button>
        <Button
          flex={1}
          onPress={handleSubmit(onSubmit)}
          disabled={(isPendingAddQuickReport && !isPausedAddQuickReport) || isUploadingAttachments}
        >
          {(!isPendingAddQuickReport && !isPausedAddQuickReport) || !isUploadingAttachments
            ? "Submit issue"
            : "Processing..."}
        </Button>
      </XStack>
    </>
  );
};

export default ReportIssue;
