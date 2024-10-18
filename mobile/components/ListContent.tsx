import { Spinner, YStack } from "tamagui";

import { Typography } from "./Typography";
import { Icon } from "./Icon";

import { useTranslation } from "react-i18next";

interface EmptyContentProps {
  translationKey: string;
  illustrationIconKey?: string;
  emptyContainerMarginTop?: string;
}

export const EmptyContent = ({
  translationKey,
  illustrationIconKey,
  emptyContainerMarginTop = "40%",
}: EmptyContentProps) => {
  const { t } = useTranslation(translationKey);
  return (
    <YStack
      alignItems="center"
      justifyContent="center"
      gap="$md"
      marginTop={emptyContainerMarginTop}
    >
      {illustrationIconKey && <Icon icon={illustrationIconKey} size={190} />}

      <YStack gap="$xs" paddingHorizontal="$lg">
        <Typography preset="subheading" textAlign="center">
          {t("empty.heading")}
        </Typography>
        <Typography preset="body1" textAlign="center" color="$gray12">
          {t("empty.paragraph")}
        </Typography>
      </YStack>
    </YStack>
  );
};

export const LoadingContent = () => {
  return (
    <YStack flex={1} justifyContent="center" alignItems="center">
      <Spinner size="large" color="$purple5" />
    </YStack>
  );
};
