import React, { useState } from "react";
import { Sheet, XStack } from "tamagui";
import RNDateTimePicker, { DateTimePickerEvent } from "@react-native-community/datetimepicker";
import { Keyboard, Platform } from "react-native";
import { Typography } from "../Typography";
import { Icon } from "../Icon";

export interface DateInputProps {
  value: Date;
  onChange: (...event: any[]) => void;
  minimumDate?: Date;
  maximumDate?: Date;
  placeholder?: string;
}

export const DateInput: React.FC<DateInputProps> = ({
  value,
  onChange,
  minimumDate,
  maximumDate,
  placeholder,
}) => {
  const [open, setOpen] = useState(false);

  const handleSheetOpen = () => {
    Keyboard.dismiss();
    setOpen(true);
  };

  const onDateChange = (event: DateTimePickerEvent, selectedDate: Date | undefined) => {
    // send selected date to the form
    onChange(selectedDate);

    // on android, close the modal
    if (Platform.OS === "android") {
      setOpen(false);
    }
  };

  return (
    <XStack
      onPress={handleSheetOpen}
      backgroundColor="white"
      justifyContent="space-between"
      alignItems="center"
      paddingHorizontal={14}
      paddingVertical="$xs"
      borderWidth={1}
      borderColor="$gray3"
      borderRadius={8}
      gap="$xs"
    >
      <Typography preset="body1" color="$gray5" numberOfLines={1} width="90%">
        {value ? value.toLocaleDateString("en-GB") : placeholder}
      </Typography>
      <Icon icon="calendar" color="transparent" />

      {/* open bottom sheet on ios with date picker */}
      {Platform.OS === "ios" ? (
        <Sheet modal native open={open} onOpenChange={setOpen} zIndex={100_000} snapPoints={[45]}>
          <Sheet.Overlay />
          <Sheet.Frame padding="$md">
            <XStack gap="$sm" justifyContent="space-between" width="100%"></XStack>
            <XStack flex={1} justifyContent="center" alignItems="center">
              <RNDateTimePicker
                value={value || new Date()}
                display="spinner"
                onChange={onDateChange}
                minimumDate={minimumDate}
                maximumDate={maximumDate}
              />
            </XStack>
          </Sheet.Frame>
        </Sheet>
      ) : (
        // open date picker modal on android
        open && (
          <RNDateTimePicker
            value={value || new Date()}
            onChange={onDateChange}
            minimumDate={minimumDate}
            maximumDate={maximumDate}
          />
        )
      )}
    </XStack>
  );
};
