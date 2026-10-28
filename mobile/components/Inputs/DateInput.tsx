import React, { useMemo, useState } from "react";
import { Sheet, XStack, XStackProps } from "tamagui";
import RNDateTimePicker, {
  DateTimePickerAndroid,
  DateTimePickerEvent,
} from "@react-native-community/datetimepicker";
import { Animated, Keyboard, Platform } from "react-native";
import { Typography } from "../Typography";
import { Icon } from "../Icon";
import Button from "../Button";
import { useLocalSearchParams } from "expo-router";
import { useTranslation } from "react-i18next";
import useAnimatedKeyboardPadding from "../../hooks/useAnimatedKeyboardPadding";

export type DateInputMode = "date" | "time" | "datetime";

export interface DateInputProps extends XStackProps {
  value: Date | null;
  // what is picked: a calendar day, a time of day, or both (the default)
  mode?: DateInputMode;
  onChange: (...event: any[]) => void;
  minimumDate?: Date | null;
  maximumDate?: Date | null;
  placeholder?: string;
  error?: string;
}

const formatDay = (value: Date) =>
  value.toLocaleDateString(["en-GB"], { month: "2-digit", day: "2-digit", year: "numeric" });
const formatTime = (value: Date) =>
  value.toLocaleTimeString([], { hour: "2-digit", minute: "2-digit" });

const formatValue = (value: Date, mode: DateInputMode) => {
  if (mode === "date") return formatDay(value);
  if (mode === "time") return formatTime(value);
  return `${formatDay(value)} - ${formatTime(value)}`;
};

export const DateInput: React.FC<DateInputProps> = ({
  value,
  onChange,
  minimumDate,
  maximumDate,
  placeholder,
  disabled,
  error,
  mode = "datetime",
  ...rest
}) => {
  const localParams = useLocalSearchParams();
  const [open, setOpen] = useState(false);
  // on ios we use a temporary date, as the onChange function gets triggered every time the user picks a new date
  // therefore, we will update the FINAL date state (that comes from the outside), only onDonePress
  const [tempDate, setTempDate] = useState(value || new Date());
  const { t } = useTranslation("common");

  const handleSheetOpen = () => {
    Keyboard.dismiss();
    setOpen(true);
  };

  const onDateChange = (event: DateTimePickerEvent, selectedDate: Date | undefined) => {
    if (Platform.OS === "ios") {
      selectedDate && setTempDate(selectedDate);
    } else {
      if (event.type === "set") {
        onClose();
        // android picks one part per dialog: a date or time question is done after its own dialog
        if (mode !== "datetime") {
          selectedDate && onChange(selectedDate);
          return;
        }
        DateTimePickerAndroid.open({
          mode: "time",
          value: value || new Date(),
          onChange: (event, eventTime) => {
            if (eventTime && selectedDate) {
              selectedDate.setHours(eventTime.getHours());
              selectedDate.setMinutes(eventTime.getMinutes());
              onChange(selectedDate);
            }
          },
        });
      } else if (event.type === "dismissed") {
        // press Cancel - close modal
        onClose();
      }
    }
  };

  const onDonePress = () => {
    // if the onChange was not already triggered and the value wasn't updated, we set the date with the current one
    onChange(tempDate);
    onClose();
  };

  const onClose = () => {
    setOpen(false);
  };

  const paddingBottom = useAnimatedKeyboardPadding(16);
  const AnimatedSheetFrame = useMemo(() => Animated.createAnimatedComponent(Sheet.Frame), []);

  return (
    <XStack
      onPress={disabled ? undefined : handleSheetOpen}
      backgroundColor={disabled ? "$gray1" : "white"}
      justifyContent="space-between"
      alignItems="center"
      paddingHorizontal={14}
      paddingVertical="$xs"
      borderWidth={1}
      borderColor={error ? "$red5" : "$gray3"}
      borderRadius={8}
      gap="$xs"
      {...rest}
    >
      <Typography
        preset="body1"
        color={disabled ? "$gray4" : "$gray5"}
        numberOfLines={1}
        width="90%"
      >
        {value ? formatValue(value, mode) : placeholder}
      </Typography>
      <Icon icon="calendar" color={disabled ? "$gray3" : "$gray4"} />
      {/* open bottom sheet on ios with date picker */}
      {Platform.OS === "ios" && open ? (
        <Sheet modal open onOpenChange={setOpen} zIndex={100_000} snapPointsMode="fit">
          <Sheet.Overlay />
          <AnimatedSheetFrame padding="$md" paddingBottom={paddingBottom}>
            <XStack gap="$sm" justifyContent="flex-end" width="100%">
              <Button onPress={onDonePress}>{t("done")}</Button>
            </XStack>
            <XStack flex={1} justifyContent="center" alignItems="center" marginVertical="$sm">
              <RNDateTimePicker
                mode={mode}
                display="spinner"
                value={tempDate}
                onChange={onDateChange}
                minimumDate={minimumDate || undefined}
                maximumDate={maximumDate || undefined}
                locale={localParams.language as string}
              />
            </XStack>
          </AnimatedSheetFrame>
        </Sheet>
      ) : (
        // open date picker modal on android
        open &&
        DateTimePickerAndroid.open({
          mode: mode === "time" ? "time" : "date",
          value: value || new Date(),
          onChange: onDateChange,
          is24Hour: true,
          minimumDate: minimumDate || undefined,
          maximumDate: maximumDate || undefined,
        })
      )}
    </XStack>
  );
};
