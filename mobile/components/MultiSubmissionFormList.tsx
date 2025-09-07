import { useQueryClient } from "@tanstack/react-query";
import * as Crypto from "expo-crypto";
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
import { mapFormToMultiSubmissionFormListItem as mapFormToMultiSubmissionFormListItem } from "../services/form.parser";
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
import MultiSubmissionFormCard from "./MultiSubmissionFormCard";
import FormListEmptyComponent from "./FormListEmptyComponent";
import FormListErrorScreen from "./FormListError";
import { ListView } from "./ListView";
import { Typography } from "./Typography";
import SelectFormLanguageDialogContent from "./SelectFormLanguageDialogContent";

const ESTIMATED_ITEM_SIZE = 100;

export type MultiSubmissionFormListItem = {
  id: string;
  name: string;
  options: string;
  languages: string[];
  numberOfSubmissions: number;
};

type ListHeaderComponentType =
  | ComponentType<any>
  | ReactElement<any, string | JSXElementConstructor<any>>
  | null
  | undefined;

interface IMultiSubmissionFormListProps {
  ListHeaderComponent: ListHeaderComponentType;
}

const MultiSubmissionFormList = ({ ListHeaderComponent }: IMultiSubmissionFormListProps) => {
  const { t } = useTranslation(["observation", "common"]);
  const { isOnline } = useNetInfoContext();
  const { width } = useWindowDimensions();

  const { activeElectionRound, selectedPollingStation } = useUserData();
  const queryClient = useQueryClient();

  const [isRefreshing, setIsRefreshing] = useState(false);
  const [selectedForm, setSelectedForm] = useState<MultiSubmissionFormListItem | null>(null);

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

  const formList: MultiSubmissionFormListItem[] = useMemo(() => {
    return mapFormToMultiSubmissionFormListItem(allForms?.forms, formSubmissions);
  }, [allForms, formSubmissions]);

  const {
    control,
    handleSubmit,
    formState: { errors },
  } = useForm({});

  const onConfirmFormLanguage = (formItem: MultiSubmissionFormListItem, language: string) => {
    setFormLanguagePreference({ formId: formItem.id, language });

    if (formItem.numberOfSubmissions === 0) {
      const newSubmissionId = Crypto.randomUUID();
      setSelectedForm(null);
      router.push(
        `/form-submission-details/${newSubmissionId}?formId=${formItem.id}&language=${language}`,
      );
    } else {
      router.push(`/multi-submission-form-details/${formItem.id}?language=${language}`);
      setSelectedForm(null);
    }
  };

  const openForm = async (formItem: MultiSubmissionFormListItem) => {
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
      <ListView<MultiSubmissionFormListItem>
        data={formList}
        ListHeaderComponent={ListHeaderComponent}
        contentContainerStyle={{ paddingVertical: 16 }}
        ListEmptyComponent={<FormListEmptyComponent />}
        showsVerticalScrollIndicator={false}
        bounces={isOnline}
        renderItem={({ item, index }) => {
          return (
            <MultiSubmissionFormCard
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

export default MultiSubmissionFormList;
