import React from "react";
import { Dimensions, ViewStyle } from "react-native";
import { router } from "expo-router";
import * as ReactotronCommands from "../../../../helpers/reactotron-custom-commands";
import { Screen } from "../../../../components/Screen";
import { useUserData } from "../../../../contexts/user/UserContext.provider";
import { Typography } from "../../../../components/Typography";
import Button from "../../../../components/Button";
import { Card, Stack, XStack, YStack } from "tamagui";
import { Icon } from "../../../../components/Icon";
import { ListView } from "../../../../components/ListView";
import TimeSelect from "../../../../components/TimeSelect";
import CardFooter from "../../../../components/CardFooter";
import PollingStationInfoDefault from "../../../../components/PollingStationInfoDefault";
import FormCard from "../../../../components/FormCard";
import {
  pollingStationsKeys,
  upsertPollingStationGeneralInformationMutation,
  usePollingStationInformation,
} from "../../../../services/queries.service";
import { ApiFormAnswer } from "../../../../services/interfaces/answer.type";
import { useQueryClient } from "@tanstack/react-query";
import SelectPollingStation from "../../../../components/SelectPollingStation";

ReactotronCommands.default();

const MissingElectionRounds = () => (
  <Screen preset="fixed">
    <Stack height="100%" backgroundColor="white" justifyContent="center" alignItems="center">
      <YStack width={312} alignItems="center">
        <Icon icon="peopleAddingVote" marginBottom="$md" />
        <Typography preset="subheading" textAlign="center" marginBottom="$xxxs">
          No election event to observe yet
        </Typography>
        <Typography preset="body1" textAlign="center" color="$gray5">
          You will be able to use the app once you will be assigned to an election event by your
          organization
        </Typography>
      </YStack>
    </Stack>
  </Screen>
);

const MissingVisits = () => (
  <Screen preset="fixed">
    <Stack height="100%" backgroundColor="white" justifyContent="center" alignItems="center">
      <YStack width={312} alignItems="center" gap="$md">
        <Icon icon="missingPollingStation" />
        <YStack gap="$xxxs">
          <Typography preset="subheading" textAlign="center">
            No visited polling stations yet
          </Typography>
          <Typography preset="body1" textAlign="center" color="$gray5">
            Start configuring your first polling station before completing observation forms.
          </Typography>
        </YStack>
        <Button
          preset="outlined"
          backgroundColor="white"
          width="100%"
          onPress={router.push.bind(null, "/polling-station-wizzard")}
        >
          Add your first polling station
        </Button>
      </YStack>
    </Stack>
  </Screen>
);

type FormItemStatus = "not started" | "in progress" | "completed";

type FormListItem = {
  id: string;
  name: string;
  options: string;
  numberOfQuestions: number;
  numberOfCompletedQuestions: number;
  status: FormItemStatus;
};

// Function to generate a random status
function getRandomStatus(): FormItemStatus {
  const statuses = ["not started", "in progress", "completed"];
  const randomIndex = Math.floor(Math.random() * statuses.length);
  return statuses[randomIndex] as FormItemStatus;
}

// Generate an array of 25 elements
const formList: FormListItem[] = Array.from({ length: 25 }, (_, index) => ({
  id: `id_${index + 1}`,
  name: `Form ${index + 1}`,
  options: `Option ${index + 1}`,
  numberOfQuestions: Math.floor(Math.random() * 10) + 1,
  numberOfCompletedQuestions: Math.floor(Math.random() * 10),
  status: getRandomStatus(),
}));

const FormList = () => {
  return (
    <YStack gap="$xxs">
      <Typography>Flashlist</Typography>
      {/* To do validate the height of this list */}
      <YStack height={Dimensions.get("screen").height}>
        <ListView<FormListItem>
          data={formList}
          showsVerticalScrollIndicator={false}
          bounces={false}
          renderItem={({ item, index }) => {
            return (
              <FormCard
                key={index}
                form={item}
                onPress={() => console.log("Form Card action")}
                marginBottom="$xxs"
              />
            );
          }}
          estimatedItemSize={100}
        />
      </YStack>
    </YStack>
  );
};

type PollingStationInformationVM = {
  arrivalTime: string;
  departureTime: string;
  answers: ApiFormAnswer[];
};

const Index = () => {
  const { isAssignedToEllectionRound, visits, selectedPollingStation, activeElectionRound } =
    useUserData();
  const { selectedPollingStation: _selectedPollingStation } = useUserData();
  // TODO: how do we want to manage the time?

  const { data } = usePollingStationInformation(
    activeElectionRound?.id,
    selectedPollingStation?.pollingStationId,
  );

  const { mutate } = upsertPollingStationGeneralInformationMutation();

  const queryClient = useQueryClient();

  const updateGeneralData = (payload: Partial<PollingStationInformationVM>) => {
    // Set query data
    queryClient.setQueryData(
      pollingStationsKeys.pollingStationInformation(
        activeElectionRound?.id,
        selectedPollingStation?.pollingStationId,
      ),
      (current: any) => {
        return {
          ...current,
          ...payload,
        };
      },
    );

    if (data && selectedPollingStation && activeElectionRound) {
      mutate(
        {
          electionRoundId: activeElectionRound?.id,
          pollingStationId: selectedPollingStation?.pollingStationId as string,
          arrivalTime: data.arrivalTime,
          departureTime: data.departureTime,
          answers: data.answers,
          ...payload,
        },
        {
          onSuccess: () => {
            // QueryClient setQurydata
            console.log("OK");
          },
        },
      );
    }
  };

  console.log("Polling Station Information", data);

  if (!isAssignedToEllectionRound) {
    return <MissingElectionRounds />;
  }

  if (visits.length === 0) {
    return <MissingVisits />;
  }

  return (
    <Screen
      preset="scroll"
      contentContainerStyle={$containerStyle}
      ScrollViewProps={{
        showsVerticalScrollIndicator: false,
        stickyHeaderIndices: [0],
        bounces: false,
      }}
    >
      <SelectPollingStation />
      <YStack paddingHorizontal="$md" gap="$lg">
        <YStack gap="$xxs">
          <XStack gap="$xxs">
            <Card flex={0.5} paddingHorizontal="$md" paddingVertical="$xs" backgroundColor="white">
              <TimeSelect
                type="arrival"
                time={data?.arrivalTime ? new Date(data?.arrivalTime) : undefined}
                setTime={(data: Date) => updateGeneralData({ arrivalTime: data.toISOString() })}
              />
            </Card>
            <Card flex={0.5} paddingHorizontal="$md" paddingVertical="$xs" backgroundColor="white">
              <TimeSelect
                type="departure"
                time={data?.departureTime ? new Date(data?.departureTime) : undefined}
                setTime={(data: Date) => updateGeneralData({ departureTime: data.toISOString() })}
              />
            </Card>
          </XStack>
          <Card padding="$md" gap="$md" backgroundColor="white">
            <PollingStationInfoDefault />
            <CardFooter text="Polling station information"></CardFooter>
          </Card>
          <Card padding="$md">
            <Button onPress={() => router.push("/form-questionnaire/1")}>Go Form wizzard</Button>
          </Card>
        </YStack>
        <FormList />
      </YStack>
    </Screen>
  );
};

const $containerStyle: ViewStyle = {
  gap: 20,
};

export default Index;
