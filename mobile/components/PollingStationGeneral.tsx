import { router } from "expo-router";
import { useState } from "react";
import { Controller, useForm } from "react-hook-form";
import { useTranslation } from "react-i18next";
import { XStack, YStack } from "tamagui";
import { getFormLanguagePreference, setFormLanguagePreference } from "../common/language.preferences";
import {
  PollingStationInformationAPIResponse,
  PollingStationInformationFormAPIResponse,
} from "../services/definitions.api";
import Button from "./Button";
import Card from "./Card";
import CardFooter from "./CardFooter";
import { Dialog } from "./Dialog";
import PollingStationInfo from "./PollingStationInfo";
import PollingStationInfoDefault from "./PollingStationInfoDefault";
import { PSITime } from "./PSITime";
import SelectFormLanguageDialogContent from "./SelectFormLanguageDialogContent";
import { Typography } from "./Typography";

interface PollingStationGeneralProps {
  psiData: PollingStationInformationAPIResponse | null | undefined;
  psiFormQuestions: PollingStationInformationFormAPIResponse;
}

export const PollingStationGeneral: React.FC<PollingStationGeneralProps> = ({
  psiData: psi,
  psiFormQuestions,
}) => {
  const { t } = useTranslation("observation");
  const [displayLanguageDialog, setDisplayLanguageDialog] = useState<boolean>(false);

  const onConfirmFormLanguage = (language: string) => {
    setDisplayLanguageDialog(false);
    setFormLanguagePreference({ formId: psiFormQuestions.id, language });

    router.push(
      `/polling-station-questionnaire?language=${language}`,
    );
  };

  const openForm = async () => {
    if (!psiFormQuestions?.languages?.length) {
      console.log("No language exists");
    }

    const preferedLanguage = await getFormLanguagePreference({ formId: psiFormQuestions.id });

    if (preferedLanguage && psiFormQuestions.languages.includes(preferedLanguage)) {
      onConfirmFormLanguage( preferedLanguage);
    } else if (psiFormQuestions?.languages?.length === 1) {
      onConfirmFormLanguage(psiFormQuestions.languages[0]);
    } else {
      setDisplayLanguageDialog(true);
    }
  };

  const {
    control,
    handleSubmit,
    formState: { errors },
  } = useForm({});


  return (
    <YStack gap="$xxs">
      <Typography preset="body2" fontWeight="700" color="$gray7">
        {t("polling_stations_information.heading")}
      </Typography>

      <PSITime psiData={psi} />

      {/* only display the PSI card for polling stations that have a configured PSI form */}
      {psiFormQuestions && psiFormQuestions.questions && psiFormQuestions.questions.length > 0 && (
        <Card gap="$md" onPress={openForm}>
          {!psi?.answers?.length && !psi?.isCompleted ? (
            <PollingStationInfoDefault
              onPress={openForm}
            />
          ) : (
            <PollingStationInfo
              nrOfAnswers={psi?.answers?.length}
              nrOfQuestions={psiFormQuestions?.questions?.length}
              isMarkedAsCompleted={psi?.isCompleted}
            />
          )}
          <CardFooter
            text={t("polling_stations_information.polling_station_form.form_details_button_label")}
          ></CardFooter>
        </Card>
      )}

      {displayLanguageDialog && (
        <Controller
          key={psiFormQuestions.id}
          name={'psi'}
          control={control}
          rules={{
            required: { value: true, message: t("forms.select_language_modal.error") },
          }}
          render={({ field: { onChange, value } }) => (
            <Dialog
              open={displayLanguageDialog}
              header={
                <Typography preset="heading">{t("forms.select_language_modal.header")}</Typography>
              }
              content={
                <SelectFormLanguageDialogContent
                  languages={psiFormQuestions.languages}
                  error={errors.psi}
                  value={value}
                  onChange={onChange}
                />
              }
              footer={
                <XStack gap="$md">
                  <Button preset="chromeless" onPress={setDisplayLanguageDialog.bind(null, false)}>
                    {t("cancel", { ns: "common" })}
                  </Button>
                  <Button
                    onPress={handleSubmit(() => onConfirmFormLanguage(value))}
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
