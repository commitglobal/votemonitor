import { useQueryClient } from "@tanstack/react-query";
import { router } from "expo-router";
import { ComponentType, JSXElementConstructor, ReactElement, useMemo, useState } from "react";
import { Controller, useForm } from "react-hook-form";
import { useTranslation } from "react-i18next";
import { Spinner, useWindowDimensions, XStack, YStack } from "tamagui";
import {
  getFormLanguagePreference,
  setFormLanguagePreference,
} from "../common/language.preferences";
import { useNetInfoContext } from "../contexts/net-info-banner/NetInfoContext";
import { useUserData } from "../contexts/user/UserContext.provider";
import { mapFormToFormListItem, SubmissionStatus } from "../services/form.parser";
import {
  electionRoundsKeys,
  pollingStationsKeys,
  usePollingStationInformation,
  usePollingStationInformationForm,
} from "../services/queries.service";
import { useFormSubmissions } from "../services/queries/form-submissions.query";
import { useElectionRoundAllForms } from "../services/queries/forms.query";
import Button from "./Button";
import { Dialog } from "./Dialog";
import SingleSubmissionFormCard from "./SingleSubmissionFormCard";
import FormListEmptyComponent from "./FormListEmptyComponent";
import FormListErrorScreen from "./FormListError";
import { ListView } from "./ListView";
import SelectFormLanguageDialogContent from "./SelectFormLanguageDialogContent";
import { Typography } from "./Typography";

const ESTIMATED_ITEM_SIZE = 100;

export type FormListItem = {
  id: string;
  submissionId: string;
  name: string;
  options: string;
  numberOfQuestions: number;
  numberOfCompletedQuestions: number;
  languages: string[];
  status: SubmissionStatus;
};

type ListHeaderComponentType =
  | ComponentType<any>
  | ReactElement<any, string | JSXElementConstructor<any>>
  | null
  | undefined;

interface ISingleSubmissionFormListProps {
  ListHeaderComponent: ListHeaderComponentType;
}

const SingleSubmissionFormList = ({ ListHeaderComponent }: ISingleSubmissionFormListProps) => {
  const { t } = useTranslation(["observation", "common"]);
  const { isOnline } = useNetInfoContext();
  const { width } = useWindowDimensions();

  const { activeElectionRound, selectedPollingStation } = useUserData();
  const queryClient = useQueryClient();

  const [isRefreshing, setIsRefreshing] = useState(false);
  const [selectedForm, setSelectedForm] = useState<FormListItem | null>(null);

  const {
    data: allForms,
    isLoading: isLoadingForms,
    error: formsError,
    refetch: refetchForms,
  } = useElectionRoundAllForms(activeElectionRound?.id);

  const {
    data: formSubmissions,
    isLoading: isLoadingAnswers,
    error: answersError,
    refetch: refetchFormSubmissions,
  } = useFormSubmissions(activeElectionRound?.id, selectedPollingStation?.pollingStationId);

  const { refetch: refetchPSIData } = usePollingStationInformation(
    activeElectionRound?.id,
    selectedPollingStation?.pollingStationId,
  );

  const { refetch: refetchPSIFormQuestions } = usePollingStationInformationForm(
    activeElectionRound?.id,
  );

  const handleRefetch = () => {
    setIsRefreshing(true);
    Promise.all([
      refetchPSIData(),
      refetchPSIFormQuestions(),
      refetchForms(),
      refetchFormSubmissions(),
    ]).then(() => {
      setIsRefreshing(false);
    });
  };

  const formList: FormListItem[] = useMemo(() => {
    return mapFormToFormListItem(allForms?.forms, formSubmissions);
  }, [allForms, formSubmissions]);

  const {
    control,
    handleSubmit,
    formState: { errors },
  } = useForm({});

  const onConfirmFormLanguage = (formItem: FormListItem, language: string) => {
    setFormLanguagePreference({ formId: formItem.id, language });

    router.push(
      `/single-submission-form-details/${formItem?.id}?language=${language}&submissionId=${formItem.submissionId}`,
    ); // TODO @birloiflorian we can pass formTitle
    setSelectedForm(null);
  };

  const openForm = async (formItem: FormListItem) => {
    if (!formItem?.languages?.length) {
      // TODO: Display error toast
      console.log("No language exists");
    }

    const preferedLanguage = await getFormLanguagePreference({ formId: formItem.id });

    if (preferedLanguage && formItem.languages.includes(preferedLanguage)) {
      onConfirmFormLanguage(formItem, preferedLanguage);
    } else if (formItem?.languages?.length === 1) {
      onConfirmFormLanguage(formItem, formItem.languages[0]);
    } else {
      setSelectedForm(formItem);
    }
  };

  if (isLoadingAnswers || isLoadingForms) {
    return (
      <YStack justifyContent="center" alignItems="center" flex={1}>
        <Spinner size="large" color="$purple5" />
      </YStack>
    );
  }

  if (formsError || answersError) {
    return (
      <FormListErrorScreen
        onPress={() => {
          queryClient.invalidateQueries({
            queryKey: electionRoundsKeys.forms(activeElectionRound?.id),
          });
          queryClient.invalidateQueries({
            queryKey: pollingStationsKeys.allFormSubmissions(
              activeElectionRound?.id,
              selectedPollingStation?.pollingStationId,
            ),
          });
        }}
      />
    );
  }

  return (
    <YStack flex={1}>
      <ListView<FormListItem>
        data={formList}
        ListHeaderComponent={ListHeaderComponent}
        contentContainerStyle={{ paddingVertical: 16 }}
        ListEmptyComponent={<FormListEmptyComponent />}
        showsVerticalScrollIndicator={false}
        bounces={isOnline}
        renderItem={({ item, index }) => {
          return (
            <SingleSubmissionFormCard
              key={index}
              form={item}
              onPress={openForm.bind(null, item)}
              marginBottom="$xxs"
            />
          );
        }}
        estimatedItemSize={ESTIMATED_ITEM_SIZE}
        estimatedListSize={{ height: ESTIMATED_ITEM_SIZE * 5, width: width - 32 }}
        refreshing={isRefreshing}
        onRefresh={handleRefetch}
      />
      {selectedForm && (
        <Controller
          key={selectedForm.id}
          name={selectedForm.name}
          control={control}
          rules={{
            required: { value: true, message: t("forms.select_language_modal.error") },
          }}
          render={({ field: { onChange, value } }) => (
            <Dialog
              open={!!selectedForm}
              header={
                <Typography preset="heading">{t("forms.select_language_modal.header")}</Typography>
              }
              content={
                <SelectFormLanguageDialogContent
                  languages={selectedForm.languages}
                  error={errors[selectedForm.name]}
                  value={value}
                  onChange={onChange}
                />
              }
              footer={
                <XStack gap="$md">
                  <Button preset="chromeless" onPress={setSelectedForm.bind(null, null)}>
                    {t("cancel", { ns: "common" })}
                  </Button>
                  <Button
                    onPress={handleSubmit(() => onConfirmFormLanguage(selectedForm, value))}
                    flex={1}
                  >
                    {t("save", { ns: "common" })}
                  </Button>
                </XStack>
              }
            />
          )}
        />
      )}
    </YStack>
  );
};

export default SingleSubmissionFormList;
