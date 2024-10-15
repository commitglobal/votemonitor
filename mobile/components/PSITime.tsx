import React from "react";
import { XStack, YStack } from "tamagui";
import { Typography } from "./Typography";
import CardFooter from "./CardFooter";
import { useTranslation } from "react-i18next";
import { Icon } from "./Icon";
import Card from "./Card";
import { TimeNotDefined } from "./TimeNotDefined";
import { PollingStationInformationAPIResponse } from "../services/definitions.api";
import { router } from "expo-router";

export const PSITime = ({ psiData }: { psiData?: PollingStationInformationAPIResponse | null }) => {
  const { t } = useTranslation("observation");

  const handleNavigateToObservationTime = () => {
    router.push("/observation-time");
  };

  if (!psiData || !(psiData.arrivalTime || psiData.departureTime)) {
    return (
      <Card
        padding="$md"
        backgroundColor="white"
        gap="$xxs"
        onPress={handleNavigateToObservationTime}
      >
        <TimeNotDefined nrOfBreaks={psiData?.breaks?.length || 0} />
        <CardFooter
          text={t("polling_stations_information.observation_time.arrival_and_departure")}
        />
      </Card>
    );
  }
  return (
    <Card
      padding="$md"
      backgroundColor="white"
      gap="$xxs"
      onPress={handleNavigateToObservationTime}
    >
      <XStack justifyContent="space-between" alignItems="flex-end">
        <XStack flex={0.6}>
          <XStack maxWidth="100%" alignItems="flex-end" gap="$xxs">
            {/* arrival time */}
            <YStack alignItems="center" gap="$xxxs">
              <Typography
                preset="helper"
                fontWeight="500"
                color={psiData.arrivalTime ? "$gray7" : "$gray3"}
              >
                {psiData.arrivalTime
                  ? new Date(psiData.arrivalTime).toLocaleDateString(["en-GB"], {
                      month: "2-digit",
                      day: "2-digit",
                      year: "numeric",
                    })
                  : t("polling_stations_information.observation_time.undefined_date")}
              </Typography>

              <Typography
                preset="heading"
                fontWeight="500"
                color={psiData.arrivalTime ? "$gray7" : "$gray3"}
              >
                {psiData.arrivalTime
                  ? new Date(psiData.arrivalTime).toLocaleTimeString([], {
                      hour: "2-digit",
                      minute: "2-digit",
                    })
                  : t("polling_stations_information.observation_time.undefined_time")}
              </Typography>
            </YStack>

            <YStack>
              <Typography preset="heading" fontWeight="500">
                -
              </Typography>
            </YStack>

            {/* departure time */}
            <YStack alignItems="center" gap="$xxxs">
              <Typography
                preset="helper"
                fontWeight="500"
                color={psiData.departureTime ? "$gray7" : "$gray3"}
              >
                {psiData.departureTime
                  ? new Date(psiData.departureTime).toLocaleDateString(["en-GB"], {
                      month: "2-digit",
                      day: "2-digit",
                      year: "numeric",
                    })
                  : t("polling_stations_information.observation_time.undefined_date")}
              </Typography>

              <Typography
                preset="heading"
                fontWeight="500"
                color={psiData.departureTime ? "$gray7" : "$gray3"}
              >
                {psiData.departureTime
                  ? new Date(psiData.departureTime).toLocaleTimeString([], {
                      hour: "2-digit",
                      minute: "2-digit",
                    })
                  : t("polling_stations_information.observation_time.undefined_time")}
              </Typography>
            </YStack>
          </XStack>
        </XStack>

        {/* breaks */}
        <XStack alignItems="flex-start" flex={0.4} justifyContent="flex-end" gap="$xxxs">
          <Icon icon="coffeeBreak" color="$purple5" size={24} />
          <XStack justifyContent="flex-end" maxWidth="80%">
            <Typography fontWeight="500" color="$purple5" maxWidth="100%">
              {psiData?.breaks?.length === 1
                ? t("polling_stations_information.observation_time.one_break", {
                    value: psiData?.breaks?.length || 0,
                  })
                : t("polling_stations_information.observation_time.no_of_breaks", {
                    value: psiData?.breaks?.length || 0,
                  })}
            </Typography>
          </XStack>
        </XStack>
      </XStack>

      <CardFooter text={t("polling_stations_information.observation_time.arrival_and_departure")} />
    </Card>
  );
};
