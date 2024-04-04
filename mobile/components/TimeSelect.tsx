import React, { useState } from "react";
import { Typography } from "./Typography";
import { YStack, XStack, Sheet, Button } from "tamagui";
import { Icon } from "./Icon";
import { Platform } from "react-native";
import RNDateTimePicker, {
  DateTimePickerEvent,
} from "@react-native-community/datetimepicker";
import DateTimePickerModal from "@react-native-community/datetimepicker";

const TimeSelect = () => {
  const [open, setOpen] = useState(false);
  const [time, setTime] = useState<Date>(new Date());

  const onChange = (
    event: DateTimePickerEvent,
    selectedTime: Date | undefined
  ) => {
    selectedTime && setTime(selectedTime);
    if (Platform.OS === "android") {
      setOpen(false);
    }
    return;
  };

  const onResetTime = () => {
    const resetTime = new Date(time);
    resetTime.setMinutes(0);
    resetTime.setHours(0);
    setTime(resetTime);
  };

  const onClose = () => {
    setOpen(false);
  };

  return (
    <>
      <YStack>
        <Typography size="xl" style={{ fontWeight: "500" }}>
          {time.toLocaleTimeString([], {
            hour: "2-digit",
            minute: "2-digit",
          })}
        </Typography>
        <XStack onPress={() => setOpen(true)}>
          <Typography>Arrival time</Typography>
          <Icon icon="chevronRight" />
        </XStack>
      </YStack>

      {Platform.OS === "ios" ? (
        <Sheet
          modal
          native
          open={open}
          onOpenChange={setOpen}
          zIndex={100_000}
          snapPoints={[45]}
          moveOnKeyboardChange={true}
        >
          <Sheet.Overlay />
          <Sheet.Frame padding="$md">
            <XStack gap="$sm" justifyContent="space-between" width="100%">
              <Button fontWeight="500" fontSize={16} onPress={onResetTime}>
                Reset
              </Button>
              <Button onPress={onClose}>Done</Button>
            </XStack>
            <XStack flex={1} justifyContent="center" alignItems="center">
              <RNDateTimePicker
                mode="time"
                display="spinner"
                value={time}
                is24Hour={true}
                onChange={onChange}
              />
            </XStack>
          </Sheet.Frame>
        </Sheet>
      ) : (
        open && (
          <DateTimePickerModal
            mode="time"
            value={time}
            is24Hour={true}
            onChange={onChange}
            onTouchCancel={() => console.log("cancelled")}
          />
        )
      )}
    </>
  );
};

export default TimeSelect;
