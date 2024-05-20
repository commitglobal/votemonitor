import React, { useContext, useMemo } from "react";
import { useSafeAreaInsets } from "react-native-safe-area-context";
import { YStack } from "tamagui";
import { Icon } from "./Icon";
import { Typography } from "./Typography";
import Select from "./Select";
import Button from "./Button";
import { useTranslation } from "react-i18next";
import { Controller, useForm } from "react-hook-form";
import * as Localization from "expo-localization";
import { Language, LanguageContext } from "../contexts/language/LanguageContext.provider";

const ChooseOnboardingLanguage = ({
  setLanguageSelectionApplied,
}: {
  setLanguageSelectionApplied: React.Dispatch<React.SetStateAction<boolean>>;
}) => {
  const insets = useSafeAreaInsets();
  const { t, i18n } = useTranslation(["languages", "login"]);
  const systemLocale = Localization.getLocales()[0];
  const { changeLanguage } = useContext(LanguageContext);

  const { control, handleSubmit } = useForm({
    defaultValues: {
      selectedLanguage:
        systemLocale?.languageCode && i18n.languages.includes(systemLocale.languageCode)
          ? (systemLocale.languageCode as Language)
          : ("en" as Language),
    },
  });

  const onSubmit = ({ selectedLanguage }: { selectedLanguage: Language }) => {
    changeLanguage(selectedLanguage);
    setLanguageSelectionApplied(true);
  };

  const mapLanguagesToSelectOptions = (languages: readonly string[]) => {
    const mappedLanguages = languages.map((language, index) => {
      return {
        id: index,
        value: language,
        label: t(language) || language,
      };
    });
    return mappedLanguages;
  };

  const mappedLanguages = useMemo(
    () => mapLanguagesToSelectOptions(i18n.languages),
    [i18n.languages],
  );

  return (
    <YStack
      key="1"
      collapsable={false}
      height="100%"
      backgroundColor="$purple6"
      padding="$xl"
      paddingBottom={insets.bottom + 32}
      justifyContent="space-between"
      alignItems="center"
    >
      <YStack gap="$xl" alignItems="center" marginTop={100}>
        <Icon icon="onboardingLanguage" />

        <Typography preset="heading" fontWeight="500" textAlign="center" color="white">
          {t("onboarding.choose_language.title", { ns: "login" })}
        </Typography>
        <Controller
          control={control}
          render={({ field: { onChange, value } }) => (
            <Select options={mappedLanguages} value={value} onValueChange={onChange} />
          )}
          name="selectedLanguage"
        />

        <Typography fontSize={18} lineHeight={24} textAlign="center" color="white" opacity={0.7}>
          {t("onboarding.choose_language.description", { ns: "login" })}
        </Typography>
      </YStack>
      <Button
        backgroundColor="$yellow6"
        paddingHorizontal="$xxxl"
        textStyle={{ color: "#5F288D", fontSize: 16, fontWeight: "500" }}
        justifyContent="center"
        alignItems="center"
        onPress={handleSubmit(onSubmit)}
      >
        {t("onboarding.choose_language.action_button", { ns: "login" })}
      </Button>
    </YStack>
  );
};

export default ChooseOnboardingLanguage;
