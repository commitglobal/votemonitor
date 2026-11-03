import React from "react";
import { router } from "expo-router";
import { useTranslation } from "react-i18next";
import { XStack, YStack } from "tamagui";
import { DATE_FORMATS, getDateFormatLabel, formatDate, formatTime } from "../common/utils/time";
import { useDateTimeFormat } from "../contexts/date-time-format/DateTimeFormatContext.provider";
import Card from "./Card";
import Header from "./Header";
import { Icon } from "./Icon";
import { Screen } from "./Screen";
import { Typography } from "./Typography";

interface SettingsOptionProps {
  label: string;
  example: string;
  selected: boolean;
  onPress: () => void;
}

const SettingsOption = ({ label, example, selected, onPress }: SettingsOptionProps) => (
  <Card
    onPress={onPress}
    style={{ minHeight: 64, justifyContent: "center" }}
    borderWidth={1}
    borderColor={selected ? "$purple5" : "transparent"}
  >
    <XStack alignItems="center" justifyContent="space-between" gap="$xxs">
      <YStack gap="$xxxs" flex={1}>
        <Typography preset="body2">{label}</Typography>
        <Typography color="$gray8">{example}</Typography>
      </YStack>
      {selected && <Icon size={24} icon="check" color="$purple5" />}
    </XStack>
  </Card>
);

const DateTimeSettingsScreen = () => {
  const { t } = useTranslation("settings");
  const { is24Hour, setIs24Hour, dateFormat, setDateFormat } = useDateTimeFormat();

  // a fixed afternoon shows the difference between the 12 and 24-hour clocks
  const now = new Date();
  const afternoon = new Date(now.getFullYear(), now.getMonth(), now.getDate(), 13, 30);

  return (
    <Screen
      preset="auto"
      backgroundColor="white"
      ScrollViewProps={{
        stickyHeaderIndices: [0],
        bounces: false,
        showsVerticalScrollIndicator: false,
      }}
    >
      <Header
        title={t("title")}
        titleColor="white"
        barStyle="light-content"
        leftIcon={<Icon icon="chevronLeft" color="white" />}
        onLeftPress={() => router.back()}
      />
      <YStack paddingHorizontal="$md" paddingVertical="$xl" gap="$xl">
        <YStack gap="$md">
          <Typography preset="subheading" fontWeight="500">
            {t("time_format.title")}
          </Typography>
          {[true, false].map((option) => (
            <SettingsOption
              key={String(option)}
              label={option ? t("time_format.24h") : t("time_format.12h")}
              example={formatTime(afternoon, option)}
              selected={is24Hour === option}
              onPress={() => setIs24Hour(option)}
            />
          ))}
        </YStack>

        <YStack gap="$md">
          <Typography preset="subheading" fontWeight="500">
            {t("date_format.title")}
          </Typography>
          {DATE_FORMATS.map((option) => (
            <SettingsOption
              key={option}
              label={getDateFormatLabel(option)}
              example={formatDate(now, option)}
              selected={dateFormat === option}
              onPress={() => setDateFormat(option)}
            />
          ))}
        </YStack>
      </YStack>
    </Screen>
  );
};

export default DateTimeSettingsScreen;
