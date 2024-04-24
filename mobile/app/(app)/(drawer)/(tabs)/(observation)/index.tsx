import React, { ComponentType, JSXElementConstructor, ReactElement, useState } from "react";
import { router, useNavigation } from "expo-router";
import { Screen } from "../../../../../components/Screen";
import { useUserData } from "../../../../../contexts/user/UserContext.provider";
import { Typography } from "../../../../../components/Typography";
import { XStack, YStack } from "tamagui";
import { ListView } from "../../../../../components/ListView";
import FormCard from "../../../../../components/FormCard";
import {
  useElectionRoundAllForms,
  useFormSubmissions,
  usePollingStationInformation,
  usePollingStationInformationForm,
} from "../../../../../services/queries.service";
import SelectPollingStation from "../../../../../components/SelectPollingStation";
import { Dialog } from "../../../../../components/Dialog";
import Button from "../../../../../components/Button";
import Header from "../../../../../components/Header";
import { Icon } from "../../../../../components/Icon";
import { DrawerActions } from "@react-navigation/native";
import { FormStatus, mapFormStateStatus } from "../../../../../services/form.parser";
import { useTranslation } from "react-i18next";
import RadioFormInput from "../../../../../components/FormInputs/RadioFormInput";
import { Controller, FieldError, FieldErrorsImpl, Merge, useForm } from "react-hook-form";
import { Dimensions } from "react-native";
import { useSafeAreaInsets } from "react-native-safe-area-context";
import NoVisitsExist from "../../../../../components/NoVisitsExist";
import { PollingStationGeneral } from "../../../../../components/PollingStationGeneral";

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
  const insets = useSafeAreaInsets();

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
      {/* height = screen height - tabs height (50 + insets.bottom) - header (70 + insets.top) - select polling station component (66) - margin (20) */}
      <YStack
        height={Dimensions.get("screen").height - 50 - insets.bottom - 60 - insets.top - 66 - 20}
      >
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

const Index = () => {
  const navigation = useNavigation();

  const { isLoading, visits, selectedPollingStation, activeElectionRound } = useUserData();

  const { data: psiData } = usePollingStationInformation(
    activeElectionRound?.id,
    selectedPollingStation?.pollingStationId,
  );

  const { data: psiFormQuestions } = usePollingStationInformationForm(activeElectionRound?.id);

  if (!isLoading && visits && !visits.length) {
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

      <YStack paddingHorizontal="$md">
        <FormList
          ListHeaderComponent={
            <YStack>
              {activeElectionRound &&
                selectedPollingStation?.pollingStationId &&
                psiData !== undefined &&
                psiFormQuestions && (
                  <PollingStationGeneral
                    electionRoundId={activeElectionRound.id}
                    pollingStationId={selectedPollingStation.pollingStationId}
                    psiData={psiData}
                    psiFormQuestions={psiFormQuestions}
                  />
                )}
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
