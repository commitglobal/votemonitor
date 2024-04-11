import React, { useState } from "react";
import { Typography } from "./Typography";
import { YStack, XStack, Sheet } from "tamagui";
import Button from "../components/Button";
import { Platform } from "react-native";
import RNDateTimePicker, { DateTimePickerEvent } from "@react-native-community/datetimepicker";
import CardFooter from "../components/CardFooter";

interface TimeSelectProps {
  type: "arrival" | "departure";
  time: Date;
  setTime: React.Dispatch<React.SetStateAction<Date>>;
}

enum CardFooterDisplay {
  ARRIVAL = "Arrival",
  DEPARTURE = "Departure",
}

const TimeSelect: React.FC<TimeSelectProps> = ({ type, time, setTime }) => {
  const [open, setOpen] = useState(false);

  const onChange = (event: DateTimePickerEvent, selectedTime: Date | undefined) => {
    selectedTime && setTime(selectedTime);
    if (Platform.OS === "android") {
      setOpen(false);
    }
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
      <YStack onPress={() => setOpen(true)}>
        <Typography preset="heading" fontWeight="500" paddingVertical="$sm" marginBottom="$xxs">
          {time.toLocaleTimeString([], {
            hour: "2-digit",
            minute: "2-digit",
          })}
        </Typography>

        <CardFooter>
          <Typography fontWeight="500" color="$gray5">
            {type === "arrival" ? CardFooterDisplay.ARRIVAL : CardFooterDisplay.DEPARTURE} time
          </Typography>
        </CardFooter>
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
              <Button preset="outlined" onPress={onResetTime}>
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
        open && <RNDateTimePicker mode="time" value={time} is24Hour={true} onChange={onChange} />
      )}
    </>
  );
};

export default TimeSelect;
