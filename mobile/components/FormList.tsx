import { ComponentType, JSXElementConstructor, ReactElement, useMemo, useState } from "react";
import { FormStatus, mapFormToFormListItem } from "../services/form.parser";
import { useUserData } from "../contexts/user/UserContext.provider";
import { useTranslation } from "react-i18next";
import { Controller, FieldError, FieldErrorsImpl, Merge, useForm } from "react-hook-form";
import {
  getFormLanguagePreference,
  setFormLanguagePreference,
} from "../common/language.preferences";
import { router } from "expo-router";
import { Typography } from "./Typography";
import { Spinner, useWindowDimensions, XStack, YStack } from "tamagui";
import { ListView } from "./ListView";
import FormCard from "./FormCard";
import { Dialog } from "./Dialog";
import RadioFormInput from "./FormInputs/RadioFormInput";
import Button from "./Button";
import { useFormSubmissions } from "../services/queries/form-submissions.query";
import { useElectionRoundAllForms } from "../services/queries/forms.query";
import FormListErrorScreen from "./FormListError";
import { useQueryClient } from "@tanstack/react-query";
import { electionRoundsKeys, pollingStationsKeys } from "../services/queries.service";
import { useSafeAreaInsets } from "react-native-safe-area-context";

const ESTIMATED_ITEM_SIZE = 100;

export type FormListItem = {
  id: string;
  name: string;
  options: string;
  numberOfQuestions: number;
  numberOfCompletedQuestions: number;
  languages: string[];
  status: FormStatus;
};

type ListHeaderComponentType =
  | ComponentType<any>
  | ReactElement<any, string | JSXElementConstructor<any>>
  | null
  | undefined;

const FormList = ({ ListHeaderComponent }: { ListHeaderComponent: ListHeaderComponentType }) => {
  const { activeElectionRound, selectedPollingStation } = useUserData();
  const [selectedForm, setSelectedForm] = useState<FormListItem | null>(null);
  const { t } = useTranslation(["observation", "common"]);
  const insets = useSafeAreaInsets();
  const { height, width } = useWindowDimensions();
  const scrollHeight = useMemo(() => height - insets.top - insets.bottom - 100 - 60, []);

  const queryClient = useQueryClient();

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

    router.push(`/form-details/${formItem?.id}?language=${language}`); // TODO @birloiflorian we can pass formTitle
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
      <YStack justifyContent="center" alignItems="center" minHeight={scrollHeight - 50}>
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
            queryKey: pollingStationsKeys.formSubmissions(
              activeElectionRound?.id,
              selectedPollingStation?.pollingStationId,
            ),
          });
        }}
      />
    );
  }

  return (
    <YStack gap="$xxs">
      {/* height = number of forms * formCard max height + ListHeaderComponent height  */}
      <YStack minHeight={scrollHeight}>
        <ListView<FormListItem>
          data={formList}
          ListHeaderComponent={ListHeaderComponent}
          ListEmptyComponent={<Typography>{t("forms.list.empty")}</Typography>}
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
          estimatedItemSize={ESTIMATED_ITEM_SIZE}
          estimatedListSize={{ height: ESTIMATED_ITEM_SIZE * 5, width: width - 32 }}
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
                  <Typography preset="heading">
                    {t("forms.select_language_modal.header")}
                  </Typography>
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

export default FormList;
