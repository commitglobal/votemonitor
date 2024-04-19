import React, { useMemo, useState } from "react";
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
  useElectionRoundAllForms,
  useFormSubmissions,
  usePollingStationInformation,
  usePollingStationInformationForm,
} from "../../../../../services/queries.service";
import { ApiFormAnswer } from "../../../../../services/interfaces/answer.type";
import { useMutation, useQueryClient } from "@tanstack/react-query";
import SelectPollingStation from "../../../../../components/SelectPollingStation";
import NoVisitsExist from "../../../../../components/NoVisitsExist";
import NoElectionRounds from "../../../../../components/NoElectionRounds";
import PollingStationInfo from "../../../../../components/PollingStationInfo";
import { Dialog } from "../../../../../components/Dialog";
import Button from "../../../../../components/Button";
import Header from "../../../../../components/Header";
import { Icon } from "../../../../../components/Icon";
import { DrawerActions } from "@react-navigation/native";
import {
  PollingStationInformationAPIPayload,
  PollingStationInformationAPIResponse,
  upsertPollingStationGeneralInformation,
} from "../../../../../services/definitions.api";
import LottieView from "lottie-react-native";

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
  console.log("formSubmissions", formSubmissions?.submissions);

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

  const onConfirmFormLanguage = (language: string) => {
    console.log("language", language);

    // navigate to the language
    router.push(`/form-details/${selectedFormId}?language=${language}`);

    setSelectedFormId(null);
  };

  if (isLoadingAnswers || isLoadingForms) {
    return (
      <LottieView
        source={require("../../../../../assets/animations/loader.json")}
        autoPlay
        loop
        style={{ height: 50 }}
      />
    );
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
        footer={<Button onPress={onConfirmFormLanguage.bind(null, "EN")}>Confirm selection</Button>}
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

  const pollingStationInformationQK = useMemo(
    () =>
      pollingStationsKeys.pollingStationInformation(
        activeElectionRound?.id,
        selectedPollingStation?.pollingStationId,
      ),
    [activeElectionRound, selectedPollingStation],
  );

  // TODO: this is almost duplicate of PollingStationQuestionnaire, merge them
  const { mutate } = useMutation({
    mutationKey: [pollingStationsKeys.mutatePollingStationGeneralData()],
    mutationFn: async (payload: PollingStationInformationAPIPayload) => {
      return upsertPollingStationGeneralInformation(payload);
    },
    onMutate: async (payload: PollingStationInformationAPIPayload) => {
      // Cancel any outgoing refetches
      // (so they don't overwrite our optimistic update)
      await queryClient.cancelQueries({ queryKey: pollingStationInformationQK });

      // Snapshot the previous value
      const previousData = queryClient.getQueryData<PollingStationInformationAPIResponse>(
        pollingStationInformationQK,
      );

      // Optimistically update to the new value
      queryClient.setQueryData<PollingStationInformationAPIResponse>(pollingStationInformationQK, {
        ...(previousData || {
          id: "-1",
          pollingStationId: payload?.pollingStationId,
          answers: [],
        }),
        arrivalTime: payload?.arrivalTime || previousData?.arrivalTime || "",
        departureTime: payload?.departureTime || previousData?.departureTime || "",
      });

      // Return a context object with the snapshotted value
      return { previousData };
    },
    onError: (err, newData, context) => {
      console.log(err);
      queryClient.setQueryData(pollingStationInformationQK, context?.previousData);
    },
    onSettled: () => {
      // TODO: we want to keep the mutation in pending until the refetch is done?
      return queryClient.invalidateQueries({ queryKey: pollingStationInformationQK });
    },
  });

  const updateArrivalDepartureTime = (
    payload: Partial<Pick<PollingStationInformationVM, "arrivalTime" | "departureTime">>,
  ) => {
    if (selectedPollingStation?.pollingStationId && activeElectionRound?.id) {
      mutate({
        electionRoundId: activeElectionRound?.id,
        pollingStationId: selectedPollingStation?.pollingStationId,
        arrivalTime: data?.arrivalTime || null,
        departureTime: data?.departureTime || null,
        ...payload,
      });
    } else {
      console.error("Missing election round and polling station");
    }
  };

  if (error) {
    return <Typography>Error while loading data {JSON.stringify(error)}</Typography>;
  }

  if (isLoading) {
    return (
      <LottieView
        source={require("../../../../../assets/animations/loader.json")}
        autoPlay
        loop
        style={{ height: 50 }}
      />
    );
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
                setTime={(data: Date) =>
                  updateArrivalDepartureTime({ arrivalTime: data?.toISOString() })
                }
              />
            </Card>
            <Card flex={0.5} paddingVertical="$xs">
              <TimeSelect
                type="departure"
                time={data?.departureTime ? new Date(data?.departureTime) : undefined}
                setTime={(data: Date) =>
                  updateArrivalDepartureTime({ departureTime: data?.toISOString() })
                }
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
