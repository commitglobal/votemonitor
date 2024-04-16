import React from "react";
import { Dimensions, ViewStyle } from "react-native";
// import { useAuth } from "../../../../hooks/useAuth";
import { router } from "expo-router";
import * as ReactotronCommands from "../../../../helpers/reactotron-custom-commands";
import { Screen } from "../../../../components/Screen";
import { useUserData } from "../../../../contexts/user/UserContext.provider";
import { Typography } from "../../../../components/Typography";
import Button from "../../../../components/Button";
import { XStack, YStack } from "tamagui";
import { ListView } from "../../../../components/ListView";
import TimeSelect from "../../../../components/TimeSelect";
import CardFooter from "../../../../components/CardFooter";
import PollingStationInfoDefault from "../../../../components/PollingStationInfoDefault";
import Card from "../../../../components/Card";
import FormCard from "../../../../components/FormCard";
import {
  pollingStationsKeys,
  upsertPollingStationGeneralInformationMutation,
  useElectionRoundAllForms,
  useFormSubmissions,
  usePollingStationInformation,
  usePollingStationInformationForm,
} from "../../../../services/queries.service";
import { ApiFormAnswer } from "../../../../services/interfaces/answer.type";
import { useQueryClient } from "@tanstack/react-query";
import SelectPollingStation from "../../../../components/SelectPollingStation";
import NoVisitsExist from "../../../../components/NoVisitsExist";
import NoElectionRounds from "../../../../components/NoElectionRounds";
import PollingStationInfo from "../../../../components/PollingStationInfo";
ReactotronCommands.default();

export type FormItemStatus = "not started" | "in progress" | "completed";

export type FormListItem = {
  id: string;
  name: string;
  options: string;
  numberOfQuestions: number;
  numberOfCompletedQuestions: number;
  status: FormItemStatus;
};

const FormList = () => {
  const { activeElectionRound, selectedPollingStation } = useUserData();

  const { data: allForms } = useElectionRoundAllForms(activeElectionRound?.id);
  console.log(allForms?.forms);

  const { data: formSubmissions } = useFormSubmissions(
    activeElectionRound?.id,
    selectedPollingStation?.pollingStationId,
  );
  console.log("formSubmissions", formSubmissions);

  const formList: FormListItem[] =
    allForms?.forms.map((form) => {
      return {
        id: form.id,
        name: `${form.code} - ${form.name.RO}`,
        numberOfCompletedQuestions: 0,
        numberOfQuestions: form.questions.length,
        options: `Available in ${Object.keys(form.name).join(", ")}`,
        status: "not started",
      };
    }) || [];

  return (
    <YStack gap="$xxs">
      <Typography>Flashlist</Typography>
      {/* TODO: the heigh should be number of forms * their height */}
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
  const queryClient = useQueryClient();

  const {
    isLoading,
    enoughDataForOffline,
    electionRounds,
    visits,
    selectedPollingStation,
    activeElectionRound,
  } = useUserData();

  const { data } = usePollingStationInformation(
    activeElectionRound?.id,
    selectedPollingStation?.pollingStationId,
  );

  const { data: informationFormQuestions } = usePollingStationInformationForm(
    activeElectionRound?.id,
  );

  const { mutate } = upsertPollingStationGeneralInformationMutation();

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

    if (selectedPollingStation && activeElectionRound) {
      mutate(
        {
          electionRoundId: activeElectionRound?.id,
          pollingStationId: selectedPollingStation?.pollingStationId as string,
          arrivalTime: data?.arrivalTime,
          departureTime: data?.departureTime,
          answers: data?.answers,
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

  if (isLoading) {
    return <Typography>Loading...</Typography>;
  }

  if (!enoughDataForOffline) {
    return <Typography>Not enough data for offline, need to retry...</Typography>;
  }

  if (!electionRounds?.length) {
    return <NoElectionRounds />;
  }

  if (visits.length === 0) {
    return <NoVisitsExist />;
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
            <Card flex={0.5} paddingVertical="$xs">
              <TimeSelect
                type="arrival"
                time={data?.arrivalTime ? new Date(data?.arrivalTime) : undefined}
                setTime={(data: Date) => updateGeneralData({ arrivalTime: data?.toISOString() })}
              />
            </Card>
            <Card flex={0.5} paddingVertical="$xs">
              <TimeSelect
                type="departure"
                time={data?.departureTime ? new Date(data?.departureTime) : undefined}
                setTime={(data: Date) => updateGeneralData({ departureTime: data?.toISOString() })}
              />
            </Card>
          </XStack>
          <Card gap="$md" onPress={router.push.bind(null, "/polling-station-questionnaire")}>
            {data?.answers?.length ? (
              <PollingStationInfoDefault
                onPress={router.push.bind(null, "/polling-station-questionnaire")}
              />
            ) : (
              <PollingStationInfo
                nrOfAnswers={data?.answers?.length}
                nrOfQuestions={informationFormQuestions?.questions?.length}
              />
            )}
            <CardFooter text="Polling station information"></CardFooter>
          </Card>
          <Card>
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
