import React, { useState } from "react";
import { Dimensions, ViewStyle } from "react-native";
// import { useAuth } from "../../../../hooks/useAuth";
import { router } from "expo-router";
import * as ReactotronCommands from "../../../../helpers/reactotron-custom-commands";
import { Screen } from "../../../../components/Screen";
import { useUserData } from "../../../../contexts/user/UserContext.provider";
import { Typography } from "../../../../components/Typography";
import Button from "../../../../components/Button";
import { Card, Stack, Text, XStack, YStack } from "tamagui";
import { Icon } from "../../../../components/Icon";
import { ListView } from "../../../../components/ListView";
import TimeSelect from "../../../../components/TimeSelect";
import CardFooter from "../../../../components/CardFooter";
import PollingStationInfoDefault from "../../../../components/PollingStationInfoDefault";
import FormCard from "../../../../components/FormCard";

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

const MyVisitsSection = () => {
  const { visits } = useUserData();

  return (
    <YStack elevation={1} paddingHorizontal="$md" paddingVertical={11} backgroundColor="white">
      <Text>{JSON.stringify(visits)}</Text>
    </YStack>
  );
};

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

const Index = () => {
  const { isAssignedToEllectionRound, visits } = useUserData();
  const { selectedPollingStation: _selectedPollingStation } = useUserData();
  //TODO: how do we want to manage the time?
  const [arrivalTime, setArrivalTime] = useState();
  const [departureTime, setDeparturetime] = useState();

  //
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
      <MyVisitsSection />
      <YStack paddingHorizontal="$md" gap="$lg">
        <YStack gap="$xxs">
          <XStack gap="$xxs">
            <Card flex={0.5} paddingHorizontal="$md" paddingVertical="$xs" backgroundColor="white">
              <TimeSelect type="arrival" time={arrivalTime} setTime={setArrivalTime} />
            </Card>
            <Card flex={0.5} paddingHorizontal="$md" paddingVertical="$xs" backgroundColor="white">
              <TimeSelect type="departure" time={departureTime} setTime={setDeparturetime} />
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
