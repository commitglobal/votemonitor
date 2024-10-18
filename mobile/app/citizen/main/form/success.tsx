import { Screen } from "../../../../components/Screen";
import { YStack } from "tamagui";
import { Typography } from "../../../../components/Typography";
import Button from "../../../../components/Button";
import { router, useLocalSearchParams } from "expo-router";
import { useSafeAreaInsets } from "react-native-safe-area-context";
import { useTranslation } from "react-i18next";
import { Icon } from "../../../../components/Icon";
import { useState } from "react";
import { EmailCopySheet } from "../../../../components/EmailCopySheet";
import { StatusBar } from "react-native";

const CitizenFormSuccess = () => {
  const insets = useSafeAreaInsets();
  const { t } = useTranslation("citizen_form");
  const { submissionId, formId } = useLocalSearchParams<{ submissionId: string; formId: string }>();

  const [isEmailSheetOpen, setIsEmailSheetOpen] = useState(false);
  const [copySent, setCopySent] = useState(false);

  return (
    <Screen preset="fixed" backgroundColor="white" contentContainerStyle={{ flexGrow: 1 }}>
      <StatusBar barStyle="dark-content" />

      <YStack flex={1} marginBottom={insets.bottom + 16}>
        <YStack
          gap="$lg"
          flex={1}
          justifyContent="center"
          alignItems="center"
          paddingHorizontal="$xl"
        >
          <Icon icon="successCheck" size={126} color="$purple5" />
          <Typography preset="heading" fontWeight="700" textAlign="center">
            {copySent ? t("copy_sent.title") : t("success.title")}
          </Typography>
          <Typography textAlign="center">
            {copySent ? t("copy_sent.description") : t("success.description")}
          </Typography>
        </YStack>
        <YStack gap="$xs" paddingHorizontal="$md">
          <Button onPress={() => router.back()}>{t("success.back_to_main")}</Button>
          {!copySent && (
            <Button preset="outlined" onPress={() => setIsEmailSheetOpen(true)}>
              {t("success.send_copy")}
            </Button>
          )}
        </YStack>
      </YStack>

      {isEmailSheetOpen && (
        <EmailCopySheet
          submissionId={submissionId}
          formId={formId}
          setIsEmailSheetOpen={setIsEmailSheetOpen}
          setCopySent={setCopySent}
        />
      )}
    </Screen>
  );
};

export default CitizenFormSuccess;
