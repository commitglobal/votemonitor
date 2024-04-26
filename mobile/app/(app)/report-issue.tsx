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
    { id: "other", value: "other", label: "Other polling station" },
    {
      id: "not_related_to_polling_station",
      value: "not_related_to_polling_station",
      label: "Not related to a polling station",
    },
  );
  return pollingStationsForSelect;
};

const ReportIssue = () => {
  const { visits } = useUserData();
  const pollingStations = useMemo(() => mapVisitsToSelectPollingStations(visits), [visits]);
  const [optionsSheetOpen, setOptionsSheetOpen] = useState(false);

  const insets = useSafeAreaInsets();

  const {
    control,
    handleSubmit,
    reset,
    formState: { errors },
  } = useForm({
    defaultValues: {
      polling_station: { details: "", id: "" },
      issue_title: "",
      issue_description: "",
    },
  });

  const onSubmit = (formData: Record<string, string | Record<string, string>>) => {
    // TODO: waiting for API to be ready
    console.log("💞💞💞💞💞💞 FORM DATA 💞💞💞💞💞", formData);
    router.back();
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
                  if (id === "other" && !details) return "This field is required";
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
                  {console.log(errors)}
                  {value.id === "other" && (
                    <YStack gap="$xxs">
                      <FormInput
                        title="Polling station details *"
                        type="textarea"
                        placeholder="Please write here some identification details for this polling station (such as address, name, number, etc.)"
                        value={value.details}
                        onChangeText={(details) => onChange({ ...value, details })}
                        error={errors.polling_station}
                      />
                      {errors.polling_station && (
                        <Typography color="$red5">{errors.polling_station.message}</Typography>
                      )}
                    </YStack>
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
                <YStack gap="$xxs">
                  <FormInput
                    title="Title of issue *"
                    placeholder="Write a title for this issue."
                    type="text"
                    value={value}
                    onChangeText={onChange}
                    error={errors.issue_title}
                  />
                  {errors.issue_title && (
                    <Typography color="$red5">{errors.issue_title.message}</Typography>
                  )}
                </YStack>
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
                <YStack gap="$xxs">
                  <FormInput
                    title="Description *"
                    type="textarea"
                    placeholder="Describe the situation in detail here."
                    value={value}
                    onChangeText={onChange}
                    error={errors.issue_description}
                  />
                  {errors.issue_description && (
                    <Typography color="$red5">{errors.issue_description.message}</Typography>
                  )}
                </YStack>
              )}
            />

            <AddAttachment
              paddingVertical="$xxs"
              onPress={() => {
                Keyboard.dismiss();
                setOptionsSheetOpen(true);
              }}
            />
          </YStack>
        </YStack>

        {/* //TODO: handle press on options */}
        <OptionsSheet open={optionsSheetOpen} setOpen={setOptionsSheetOpen}>
          <YStack paddingHorizontal="$sm">
            <Typography
              preset="body1"
              marginBottom="$md"
              paddingVertical="$sm"
              onPress={() => setOptionsSheetOpen(false)}
            >
              Load from gallery
            </Typography>
            <Typography
              preset="body1"
              marginBottom="$md"
              paddingVertical="$sm"
              onPress={() => setOptionsSheetOpen(false)}
            >
              Take a photo
            </Typography>
            <Typography
              preset="body1"
              marginBottom="$md"
              paddingVertical="$sm"
              onPress={() => setOptionsSheetOpen(false)}
            >
              Record a video
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
        <Button flex={1} onPress={handleSubmit(onSubmit)}>
          Submit issue
        </Button>
      </XStack>
    </>
  );
};

export default ReportIssue;
