import React from "react";
import { YStack } from "tamagui";
import Button from "./Button";
import { Typography } from "./Typography";
import { useTranslation } from "react-i18next";

const PollingStationInfoDefault = ({ onPress }: { onPress: () => void }) => {
  const { t } = useTranslation("observations_polling_station");
  return (
    <YStack
      backgroundColor="$gray1"
      borderRadius={3}
      paddingVertical="$md"
      paddingHorizontal={35}
      alignItems="center"
      gap={8}
    >
      <Typography textAlign="center" fontWeight="500" color="$gray5">
        {t("polling_station_card.title")}
      </Typography>
      <Button preset="outlined" backgroundColor="white" onPress={onPress}>
        {t("polling_station_card.actions.answer_questions")}
      </Button>
    </YStack>
  );
};

export default PollingStationInfoDefault;
