import React, { useMemo } from "react";
import { XStack, YStack } from "tamagui";
import { Typography } from "../../components/Typography";
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

const mapVisitsToSelectPollingStations = (visits: PollingStationVisitVM[] | undefined) => {
  const pollingStationsForSelect = visits
    ? visits.map((visit) => {
        return {
          id: visit.pollingStationId,
          value: visit.pollingStationId,
          label: `${visit.number} - ${visit.address}`,
        };
      })
    : [];

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

  const insets = useSafeAreaInsets();

  // TODO: default values?
  const { control, handleSubmit } = useForm({});

  const onSubmit = (formData: Record<string, string>) => {
    console.log("💞💞💞💞💞💞 FORM DATA 💞💞💞💞💞", formData);
  };

  return (
    <>
      <Screen
        preset="scroll"
        backgroundColor="white"
        ScrollViewProps={{
          stickyHeaderIndices: [0],
          bounces: false,
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
              name="polling-station"
              control={control}
              render={({
                field: {
                  onChange,
                  value = { id: undefined, issueCategory: undefined, details: undefined },
                },
              }) => (
                <>
                  <YStack gap="$xxs">
                    <Typography fontWeight="500">Polling station</Typography>
                    <Select
                      value={value.id}
                      options={pollingStations}
                      placeholder="Select polling station"
                      onValueChange={(id) =>
                        onChange({ ...value, id, issueCategory: null, details: null })
                      }
                    />
                  </YStack>
                  {value.id === "other" || value.id === "not_related_to_polling_station" ? (
                    <YStack gap="$xxs">
                      <Typography fontWeight="500">Issue category</Typography>
                      <Select
                        value={value.issueCategory}
                        options={mockOptions}
                        placeholder="Select category"
                        onValueChange={(issueCategory) => onChange({ ...value, issueCategory })}
                      />
                    </YStack>
                  ) : null}
                  {value.id === "other" && (
                    <FormInput
                      title="Polling station details *"
                      type="textarea"
                      placeholder="Please write here some identification details for this polling station (such as address, name, number, etc.)"
                      value={value.details}
                      onChangeText={(details) => onChange({ ...value, details })}
                    />
                  )}
                </>
              )}
            />

            {/* //TODO: does the '*' need to be red? */}
            <Controller
              key="issue_title"
              name="issue_title"
              control={control}
              rules={{
                required: true,
              }}
              render={({ field: { onChange, value } }) => (
                <FormInput
                  title="Title of issue *"
                  placeholder="Write a title for this issue."
                  type="text"
                  value={value}
                  onChangeText={onChange}
                />
              )}
            />

            <Controller
              key="issue_description"
              name="issue_description"
              control={control}
              rules={{
                required: true,
              }}
              render={({ field: { onChange, value } }) => (
                <FormInput
                  title="Description *"
                  type="textarea"
                  placeholder="Describe the situation in detail here."
                  value={value}
                  onChangeText={onChange}
                />
              )}
            />
          </YStack>
        </YStack>
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
        {/* //TODO: clear form */}
        <Button preset="chromeless">Clear</Button>
        <Button flex={1} onPress={handleSubmit(onSubmit)}>
          Submit issue
        </Button>
      </XStack>
    </>
  );
};

const mockOptions = [
  { id: "1", value: "fraud", label: "Fraud" },
  { id: "2", value: "error", label: "Error" },
  { id: "3", value: "other", label: "Other" },
  { id: "4", value: "issue", label: "Issue" },
  { id: "5", value: "bug", label: "Bug" },
  { id: "6", value: "defect", label: "Defect" },
  { id: "7", value: "problem", label: "Problem" },
  { id: "8", value: "glitch", label: "Glitch" },
  { id: "9", value: "inaccuracy", label: "Inaccuracy" },
  { id: "10", value: "mistake", label: "Mistake" },
];

export default ReportIssue;
