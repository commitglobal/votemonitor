import { useQueryClient } from "@tanstack/react-query";
import { router } from "expo-router";
import React, {
  ComponentType,
  JSXElementConstructor,
  ReactElement,
  useMemo,
  useState,
} from "react";
import { Controller, FieldError, FieldErrorsImpl, Merge, useForm } from "react-hook-form";
import { useTranslation } from "react-i18next";
import { RefreshControl } from "react-native";
import { Spinner, useWindowDimensions, XStack, YStack } from "tamagui";
import {
  getFormLanguagePreference,
  setFormLanguagePreference,
} from "../common/language.preferences";
import { useUserData } from "../contexts/user/UserContext.provider";
import { FormType } from "../services/definitions.api";
import { mapFormToIncidentReportFormListItem } from "../services/form.parser";
import { electionRoundsKeys } from "../services/queries.service";
import { useElectionRoundAllForms } from "../services/queries/forms.query";
import { incidentReportKeys, useIncidentReports } from "../services/queries/incident-reports.query";
import Button from "./Button";
import { Dialog } from "./Dialog";
import FormCard from "./FormCard";
import RadioFormInput from "./FormInputs/RadioFormInput";
import { FormListItem } from "./FormList";
import IncidentReportingFormListEmptyComponent from "./IncidentReportingFormListEmptyComponent";
import IncidentReportingFormListErrorScreen from "./IncidentReportingFormListError";
import { ListView } from "./ListView";
import { Typography } from "./Typography";

const ESTIMATED_ITEM_SIZE = 100;

type ListHeaderComponentType =
  | ComponentType<any>
  | ReactElement<any, string | JSXElementConstructor<any>>
  | null
  | undefined;

interface IIncidentReportingFormListProps {
  ListHeaderComponent: ListHeaderComponentType;
}
const IncidentReportingFormList = ({ ListHeaderComponent }: IIncidentReportingFormListProps) => {
  const { activeElectionRound } = useUserData();
  const [selectedForm, setSelectedForm] = useState<FormListItem | null>(null);
  const { t } = useTranslation(["incident_report", "common"]);

  const { width } = useWindowDimensions();

  const queryClient = useQueryClient();

  const {
    data: allForms,
    isLoading: isLoadingForms,
    error: formsError,
    refetch: refetchForms,
    isRefetching: isRefetchingForms,
  } = useElectionRoundAllForms(activeElectionRound?.id, (data) =>
    data.forms.filter((f) => f.formType === FormType.IncidentReporting),
  );


  const {
    data: incidentReports,
    isLoading: isLoadingAnswers,
    error: answersError,
    refetch: refetchIncidentReports,
    isRefetching: isRefetchingIncidentReports,
  } = useIncidentReports(activeElectionRound?.id);

  const isRefetching = useMemo(() => {
    return (
      isRefetchingForms ||
      isRefetchingIncidentReports
    );
  }, [
    isRefetchingForms ,
    isRefetchingIncidentReports
  ]);
  const handleRefetch = () => {
    refetchForms();
    refetchIncidentReports();
  };

  const formList: FormListItem[] = useMemo(() => {
    return mapFormToIncidentReportFormListItem(allForms,incidentReports);
  }, [allForms, incidentReports]);

  const {
    control,
    handleSubmit,
    formState: { errors },
  } = useForm({});

  const onConfirmFormLanguage = (formItem: FormListItem, language: string) => {
    setFormLanguagePreference({ formId: formItem.id, language });

    router.push(`/incident-report-form/${formItem?.id}?language=${language}?`);
    setSelectedForm(null);
  };

  const openForm = async (formItem: FormListItem) => {
    if (!formItem?.languages?.length) {
      // TODO: Display error toast
      console.log("No language exists");
    }

    const preferredLanguage = await getFormLanguagePreference({ formId: formItem.id });

    if (preferredLanguage && formItem.languages.includes(preferredLanguage)) {
      onConfirmFormLanguage(formItem, preferredLanguage);
    } else if (formItem?.languages?.length === 1) {
      onConfirmFormLanguage(formItem, formItem.languages[0]);
    } else {
      setSelectedForm(formItem);
    }
  };

  if (isLoadingForms || isLoadingAnswers) {
    return (
      <YStack justifyContent="center" alignItems="center" flex={1}>
        <Spinner size="large" color="$purple5" />
      </YStack>
    );
  }

  if (formsError || answersError) {
    return (
      <IncidentReportingFormListErrorScreen
        onPress={() => {
          queryClient.invalidateQueries({
            queryKey: electionRoundsKeys.forms(activeElectionRound?.id),
          });
          queryClient.invalidateQueries({
            queryKey: incidentReportKeys.allFormSubmissions(activeElectionRound?.id),
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
        ListEmptyComponent={<IncidentReportingFormListEmptyComponent />}
        showsVerticalScrollIndicator={false}
        bounces={true}
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
        estimatedItemSize={ESTIMATED_ITEM_SIZE}
        estimatedListSize={{ height: ESTIMATED_ITEM_SIZE * 5, width: width - 32 }}
        refreshControl={<RefreshControl refreshing={isRefetching} onRefresh={handleRefetch} />}
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
  const { t } = useTranslation(["observation", "languages"]);

  const languageMapping: { [key: string]: string } = {
    RO: t("ro", { ns: "languages" }),
    EN: t("en", { ns: "languages" }),
    PL: t("pl", { ns: "languages" }),
    BG: t("bg", { ns: "languages" }),
  };

  const transformedLanguages = languages.map((language) => ({
    id: language,
    value: language,
    // TODO: decide if we add the name to the label as well
    label: languageMapping[language] || language,
  }));

  return (
    <YStack>
      <Typography preset="body1" marginBottom="$lg">
        {t("forms.select_language_modal.helper")}
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

export default IncidentReportingFormList;
