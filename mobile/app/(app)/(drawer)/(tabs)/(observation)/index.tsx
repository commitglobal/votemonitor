import React, { ComponentType, JSXElementConstructor, memo, ReactElement, useState } from "react";
import { router, useNavigation } from "expo-router";
import { Screen } from "../../../../../components/Screen";
import { useUserData } from "../../../../../contexts/user/UserContext.provider";
import { Typography } from "../../../../../components/Typography";
import { YStack } from "tamagui";
import { ListView } from "../../../../../components/ListView";
import FormCard from "../../../../../components/FormCard";
import {
  useElectionRoundAllForms,
  useFormSubmissions,
  usePollingStationInformation,
  usePollingStationInformationForm,
} from "../../../../../services/queries.service";
import SelectPollingStation from "../../../../../components/SelectPollingStation";
import Header from "../../../../../components/Header";
import { Icon } from "../../../../../components/Icon";
import { DrawerActions } from "@react-navigation/native";
import { FormStatus, mapFormStateStatus } from "../../../../../services/form.parser";
import NoVisitsExist from "../../../../../components/NoVisitsExist";
import { PollingStationGeneral } from "../../../../../components/PollingStationGeneral";
import {
  getFormLanguagePreference,
  setFormLanguagePreference,
} from "../../../../../common/language.preferences";
import ChangeLanguageDialog from "../../../../../components/ChangeLanguageDialog";

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

  const onConfirmFormLanguage = (formId: string, language: string) => {
    setFormLanguagePreference({ formId, language });

    router.push(`/form-details/${formId}?language=${language}`);
    setSelectedForm(null);
  };

  const openForm = async (formItem: FormListItem) => {
    if (!formItem?.languages?.length) {
      // TODO: Display error toast
      console.log("No language exists");
    }

    const preferedLanguage = await getFormLanguagePreference({ formId: formItem.id });

    if (preferedLanguage && formItem.languages.includes(preferedLanguage)) {
      onConfirmFormLanguage(formItem.id, preferedLanguage);
    } else if (formItem?.languages?.length === 1) {
      onConfirmFormLanguage(formItem.id, formItem.languages[0]);
    } else {
      setSelectedForm(formItem);
    }
  };

  const ListHeader = memo(() => ListHeaderComponent as JSX.Element);

  if (isLoadingAnswers || isLoadingForms) {
    return (
      <>
        {ListHeader}
        <Typography>Loading...</Typography>
      </>
    );
  }

  if (formsError || answersError) {
    return (
      <>
        {ListHeader}
        <Typography>Error while showing form data</Typography>
      </>
    );
  }

  return (
    <YStack gap="$xxs">
      {/* height = number of forms * formCard max height + ListHeaderComponent height  */}
      <YStack height={formList.length * 140 + 400}>
        <ListView<FormListItem>
          data={formList}
          ListHeaderComponent={ListHeader}
          ListEmptyComponent={<Typography>No data to display</Typography>}
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
          <ChangeLanguageDialog
            formId={selectedForm.id}
            languages={selectedForm.languages}
            onCancel={setSelectedForm.bind(null, null)}
            onSelectLanguage={onConfirmFormLanguage}
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

export default Index;
