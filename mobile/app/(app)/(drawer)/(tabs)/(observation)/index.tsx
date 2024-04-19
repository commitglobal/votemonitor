import React, { useState } from "react";
import { Dimensions } from "react-native";
import { router, useNavigation } from "expo-router";
import { Screen } from "../../../../../components/Screen";
import { useUserData } from "../../../../../contexts/user/UserContext.provider";
import { Typography } from "../../../../../components/Typography";
import { XStack, YStack } from "tamagui";
import { ListView } from "../../../../../components/ListView";
import TimeSelect from "../../../../../components/TimeSelect";
import CardFooter from "../../../../../components/CardFooter";
import PollingStationInfoDefault from "../../../../../components/PollingStationInfoDefault";
import Card from "../../../../../components/Card";
import FormCard from "../../../../../components/FormCard";
import {
  pollingStationsKeys,
  upsertPollingStationGeneralInformationMutation,
  useElectionRoundAllForms,
  useFormSubmissions,
  usePollingStationInformation,
  usePollingStationInformationForm,
} from "../../../../../services/queries.service";
import { ApiFormAnswer } from "../../../../../services/interfaces/answer.type";
import { useQueryClient } from "@tanstack/react-query";
import SelectPollingStation from "../../../../../components/SelectPollingStation";
import NoVisitsExist from "../../../../../components/NoVisitsExist";
import NoElectionRounds from "../../../../../components/NoElectionRounds";
import PollingStationInfo from "../../../../../components/PollingStationInfo";
import { Dialog } from "../../../../../components/Dialog";
import Button from "../../../../../components/Button";
import Header from "../../../../../components/Header";
import { Icon } from "../../../../../components/Icon";
import { DrawerActions } from "@react-navigation/native";
import { FormStatus, mapFormStateStatus } from "../../../../../services/form.parser";

export type FormListItem = {
  id: string;
  name: string;
  options: string;
  numberOfQuestions: number;
  numberOfCompletedQuestions: number;
  status: FormStatus;
};

const FormList = () => {
  const { activeElectionRound, selectedPollingStation } = useUserData();
  const [selectedFormId, setSelectedFormId] = useState<string | null>(null);

  const {
    data: allForms,
    isLoading: isLoadingForms,
    error: formsError,
  } = useElectionRoundAllForms(activeElectionRound?.id);

  const {
    data: formSubmissions,
    isLoading: isLoadingAnswers,
    error: answersError,
  } = useFormSubmissions(activeElectionRound?.id, selectedPollingStation?.pollingStationId);

  const formList: FormListItem[] =
    allForms?.forms.map((form) => {
      const numberOfAnswers =
        formSubmissions?.submissions.find((sub) => sub.formId === form.id)?.answers.length || 0;
      return {
        id: form.id,
        name: `${form.code} - ${form.name.RO}`,
        numberOfCompletedQuestions: numberOfAnswers,
        numberOfQuestions: form.questions.length,
        options: `Available in ${Object.keys(form.name).join(", ")}`,
        status: mapFormStateStatus(numberOfAnswers, form.questions.length),
      };
    }) || [];

  const onConfirmFormLanguage = (language: string) => {
    // navigate to the language
    router.push(`/form-details/${selectedFormId}?language=${language}`);
    setSelectedFormId(null);
  };

  if (isLoadingAnswers || isLoadingForms) {
    return <Typography>Loading...</Typography>;
  }

  if (allForms?.forms.length === 0) {
    return <Typography>No data to display</Typography>;
  }

  if (formsError || answersError) {
    return <Typography>Error while showing form data</Typography>;
  }

  return (
    <YStack gap="$xxs">
      <Typography>Forms</Typography>
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
                onPress={setSelectedFormId.bind(null, item.id)}
                marginBottom="$xxs"
              />
            );
          }}
          estimatedItemSize={100}
        />
      </YStack>
      <Dialog
        open={!!selectedFormId}
        header={<Typography>Choose language</Typography>}
        content={<Typography>Select language</Typography>}
        footer={<Button onPress={onConfirmFormLanguage.bind(null, "RO")}>Confirm selection</Button>}
      />
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
  const navigation = useNavigation();

  const {
    isLoading,
    enoughDataForOffline,
    electionRounds,
    visits,
    selectedPollingStation,
    activeElectionRound,
    error,
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

  if (error) {
    return <Typography>Error while loading data {JSON.stringify(error)}</Typography>;
  }

  if (isLoading) {
    return <Typography>Loading...</Typography>;
  }

  if (!enoughDataForOffline) {
    return (
      <Typography>Not enough data for offline, need to invalidate queries and retry...</Typography>
    );
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
      ScrollViewProps={{
        showsVerticalScrollIndicator: false,
        stickyHeaderIndices: [0],
        bounces: false,
      }}
    >
      <YStack marginBottom={20}>
        <Header
          title={"Observation"}
          titleColor="white"
          barStyle="light-content"
          leftIcon={<Icon icon="menuAlt2" color="white" />}
          onLeftPress={() => navigation.dispatch(DrawerActions.openDrawer)}
        />
        <SelectPollingStation />
      </YStack>
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
            {!data?.answers?.length ? (
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
        </YStack>
        <FormList />
      </YStack>
    </Screen>
  );
};

export default Index;
