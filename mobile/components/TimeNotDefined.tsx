import React from "react";
import { XStack } from "tamagui";
import { Typography } from "./Typography";
import { Icon } from "./Icon";
import { useTranslation } from "react-i18next";

export const TimeNotDefined = ({ nrOfBreaks }: { nrOfBreaks: number }) => {
  const { t } = useTranslation("observation");

  return (
    <XStack justifyContent="space-between" alignItems="flex-end">
      <Typography preset="heading" fontWeight="500" flex={0.6}>
        {t("polling_stations_information.observation_time.not_defined")}
      </Typography>

      {/* breaks */}
      <XStack alignItems="flex-start" flex={0.4} justifyContent="flex-end" gap="$xxxs">
        <Icon icon="coffeeBreak" color="$purple5" size={24} />
        <XStack justifyContent="flex-end" maxWidth="80%">
          <Typography fontWeight="500" color="$purple5" maxWidth="100%">
            {t("polling_stations_information.observation_time.no_of_breaks", { value: nrOfBreaks })}
          </Typography>
        </XStack>
      </XStack>
    </XStack>
  );
};
