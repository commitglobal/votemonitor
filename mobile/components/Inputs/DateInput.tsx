import React, { useState } from "react";
import { Sheet, XStack, XStackProps } from "tamagui";
import RNDateTimePicker, { DateTimePickerEvent } from "@react-native-community/datetimepicker";
import { Keyboard, Platform } from "react-native";
import { Typography } from "../Typography";
import { Icon } from "../Icon";
import Button from "../Button";

export interface DateInputProps extends XStackProps {
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
  ...rest
}) => {
  const [open, setOpen] = useState(false);
  // on ios we use a temporary date, as the onChange function gets triggered every time the user picks a new date
  // therefore, we will update the FINAL date state (that comes from the outside), only onDonePress
  const [tempDate, setTempDate] = useState(value || new Date());

  const handleSheetOpen = () => {
    Keyboard.dismiss();
    setOpen(true);
  };

  const onDateChange = (event: DateTimePickerEvent, selectedDate: Date | undefined) => {
    if (Platform.OS === "ios") {
      selectedDate && setTempDate(selectedDate);
    } else {
      // press OK - set the time
      if (event.type === "set") {
        onClose();
        onChange(selectedDate);
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
      {...rest}
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
            <XStack gap="$sm" justifyContent="flex-end" width="100%">
              <Button onPress={onDonePress}>Done</Button>
            </XStack>
            <XStack flex={1} justifyContent="center" alignItems="center">
              <RNDateTimePicker
                value={tempDate}
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
