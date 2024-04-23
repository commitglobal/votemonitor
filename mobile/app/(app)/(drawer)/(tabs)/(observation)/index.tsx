import React, {
  ComponentType,
  JSXElementConstructor,
  ReactElement,
  useMemo,
  useState,
} from "react";
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
import { FormStatus, mapFormStateStatus } from "../../../../../services/form.parser";
import {
  PollingStationInformationAPIPayload,
  PollingStationInformationAPIResponse,
  upsertPollingStationGeneralInformation,
} from "../../../../../services/definitions.api";
import { useTranslation } from "react-i18next";
import RadioFormInput from "../../../../../components/FormInputs/RadioFormInput";
import { Controller, FieldError, FieldErrorsImpl, Merge, useForm } from "react-hook-form";
import LoadingScreen from "../../../../../components/LoadingScreen";
import NotEnoughData from "../../../../../components/NotEnoughData";
import GenericErrorScreen from "../../../../../components/GenericErrorScreen";

export type FormListItem = {
  id: string;
  name: string;
  options: string;
  numberOfQuestions: number;
  numberOfCompletedQuestions: number;
  languages: string[];
  status: FormStatus;
};

const FormList = ({
  ListHeaderComponent,
}: {
  ListHeaderComponent:
    | ComponentType<any>
    | ReactElement<any, string | JSXElementConstructor<any>>
    | null
    | undefined;
}) => {
  const { activeElectionRound, selectedPollingStation } = useUserData();
  const [selectedForm, setSelectedForm] = useState<FormListItem | null>(null);
  const { t } = useTranslation("form_overview");

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
        languages: form.languages,
      };
    }) || [];

  const {
    control,
    handleSubmit,
    formState: { errors },
  } = useForm({});

  const onConfirmFormLanguage = (formItem: FormListItem, language: string) => {
    // navigate to the language
    router.push(`/form-details/${formItem?.id}?language=${language}`);
    setSelectedForm(null);
  };

  const openForm = (formItem: FormListItem) => {
    if (!formItem?.languages?.length) {
      // TODO: Display error toast
      console.log("No language exists");
    }

    if (formItem?.languages?.length === 1) {
      onConfirmFormLanguage(formItem, formItem.languages[0]);
    } else {
      setSelectedForm(formItem);
    }
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
      {/* height = number of forms * formCard max height + ListHeaderComponent height  */}
      <YStack height={formList.length * 140 + 400}>
        <ListView<FormListItem>
          data={formList}
          ListHeaderComponent={ListHeaderComponent}
          showsVerticalScrollIndicator={false}
          bounces={false}
          renderItem={({ item, index }) => {
            return (
              <>
                <FormCard
                  key={index}
                  form={item}
                  onPress={openForm.bind(null, item)}
                  marginBottom="$xxs"
                />
              </>
            );
          }}
          estimatedItemSize={100}
        />
        {selectedForm && (
          <Controller
            key={selectedForm.id}
            name={selectedForm.name}
            control={control}
            rules={{
              required: { value: true, message: t("language_modal.error") },
            }}
            render={({ field: { onChange, value } }) => (
              <Dialog
                open={!!selectedForm}
                header={<Typography preset="heading">{t("language_modal.header")}</Typography>}
                content={
                  <DialogContent
                    languages={selectedForm.languages}
                    error={errors[selectedForm.name]}
                    value={value}
                    onChange={onChange}
                  />
                }
                footer={
                  <XStack gap="$md">
                    <Button preset="chromeless" onPress={setSelectedForm.bind(null, null)}>
                      Cancel
                    </Button>
                    <Button
                      onPress={handleSubmit(() => onConfirmFormLanguage(selectedForm, value))}
                      flex={1}
                    >
                      Save
                    </Button>
                  </XStack>
                }
              />
            )}
          />
        )}
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
  const navigation = useNavigation();

  const {
    isLoading,
    notEnoughDataForOffline,
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
    console.log(error);
    return <GenericErrorScreen />;
  }

  if (isLoading) {
    return <LoadingScreen />;
  } else {
    if (electionRounds && !electionRounds.length) {
      return <NoElectionRounds />;
    }

    if (visits && !visits.length) {
      return <NoVisitsExist />;
    }

    if (notEnoughDataForOffline) {
      return <NotEnoughData />;
    }
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

      <YStack paddingHorizontal="$md">
        <FormList
          ListHeaderComponent={
            <YStack>
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

              <Card
                gap="$md"
                onPress={router.push.bind(null, "/polling-station-questionnaire")}
                marginTop="$xxs"
              >
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

              <Typography preset="body1" fontWeight="700" marginTop="$lg" marginBottom="$xxs">
                Forms
              </Typography>
            </YStack>
          }
        />
      </YStack>
    </Screen>
  );
};

const DialogContent = ({
  languages,
  error,
  value,
  onChange,
}: {
  languages: string[];
  error: FieldError | Merge<FieldError, FieldErrorsImpl<any>> | undefined;
  value: string;
  onChange: (...event: any[]) => void;
}) => {
  const { t } = useTranslation("form_overview");

  const languageMapping: { [key: string]: string } = {
    RO: "Romanian",
    EN: "English",
  };

  const transformedLanguages = languages.map((language) => ({
    id: language,
    value: language,
    // TODO: decide if we add the name to the label as well
    label: languageMapping[language],
  }));

  return (
    <YStack>
      <Typography preset="body1" marginBottom="$lg">
        {t("language_modal.helper")}
      </Typography>
      <RadioFormInput options={transformedLanguages} value={value} onValueChange={onChange} />
      {error && (
        <Typography marginTop="$sm" style={{ color: "red" }}>
          {`${error.message}`}
        </Typography>
      )}
    </YStack>
  );
};

export default Index;
