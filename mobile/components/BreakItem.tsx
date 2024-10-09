import React from "react";
import { XStack, YStack } from "tamagui";
import { Typography } from "./Typography";
import { Icon } from "./Icon";
import DateFormInput from "./FormInputs/DateFormInput";
import { useTranslation } from "react-i18next";
import { Control, Controller, UseFormWatch } from "react-hook-form";

export const BreakItem = ({
  control,
  index,
  isPending,
  setBreakToDelete,
  watch,
}: {
  control: Control<
    {
      arrivalTime: Date | undefined;
      departureTime: Date | undefined;
      breaks: {
        start: Date | undefined;
        end: Date | undefined;
        duration?: number;
      }[];
    },
    any
  >;
  index: number;
  isPending: boolean;
  onDelete: (index: number) => void;
  watch: UseFormWatch<{
    arrivalTime: Date | undefined;
    departureTime: Date | undefined;
    breaks: {
      start: Date | undefined;
      end: Date | undefined;
      duration?: number;
    }[];
  }>;
  setBreakToDelete: (value: number) => void;
}) => {
  const { t } = useTranslation("observation");

  return (
    <YStack>
      {/* title and trash icon */}
      <XStack justifyContent="space-between" alignItems="center">
        {/* //todo: add break number */}
        <Typography preset="body2">
          {t("polling_stations_information.observation_time.break", { value: index + 1 })}
        </Typography>
        <Icon
          icon="bin"
          color={isPending ? "$gray3" : "black"}
          size={24}
          padding="$md"
          onPress={!isPending ? () => setBreakToDelete(index) : undefined}
          pressStyle={{ opacity: 0.5 }}
        />
      </XStack>
      {/* start and end time */}
      <YStack gap="$lg">
        <Controller
          name={`breaks.${index}.start`}
          control={control}
          render={({ field: { value, onChange } }) => (
            <DateFormInput
              title={t("polling_stations_information.observation_time.start_time")}
              placeholder={t("polling_stations_information.observation_time.select_start_time")}
              onChange={onChange}
              value={value}
              disabled={isPending}
            />
          )}
        />
        <Controller
          name={`breaks.${index}.end`}
          control={control}
          render={({ field: { value, onChange } }) => (
            <DateFormInput
              title={t("polling_stations_information.observation_time.end_time")}
              placeholder={t("polling_stations_information.observation_time.select_end_time")}
              onChange={onChange}
              value={value}
              disabled={!watch(`breaks.${index}.start`) || isPending}
            />
          )}
        />
      </YStack>
    </YStack>
  );
};
