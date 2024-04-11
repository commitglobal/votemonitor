import React, { useState } from "react";
import { Typography } from "./Typography";
import { YStack, XStack, Sheet, Stack } from "tamagui";
import Button from "../components/Button";
import { Platform } from "react-native";
import RNDateTimePicker, { DateTimePickerEvent } from "@react-native-community/datetimepicker";
import CardFooter from "../components/CardFooter";

interface TimeSelectProps {
  type: "arrival" | "departure";
  time: Date | undefined;
  setTime: React.Dispatch<Date> | React.Dispatch<undefined>;
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
    //TODO: do we want to reset it to undefined(current time) or to 00:00?
    const resetTime = time ? new Date(time) : new Date();
    resetTime.setMinutes(0);
    resetTime.setHours(0);
    // setTime(undefined);
    setTime(resetTime);
  };

  const onClose = () => {
    setOpen(false);
  };

  return (
    <>
      <YStack onPress={() => setOpen(true)}>
        <Stack paddingVertical="$sm" marginBottom="$xxs">
          {time ? (
            <Typography preset="heading" fontWeight="500">
              {time &&
                time.toLocaleTimeString([], {
                  hour: "2-digit",
                  minute: "2-digit",
                })}
            </Typography>
          ) : (
            <Typography preset="heading" fontWeight="500" color="$gray5">
              Not defined
            </Typography>
          )}
        </Stack>

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
                value={time || new Date()}
                is24Hour={true}
                onChange={onChange}
              />
            </XStack>
          </Sheet.Frame>
        </Sheet>
      ) : (
        open && (
          <RNDateTimePicker
            mode="time"
            value={time || new Date()}
            is24Hour={true}
            onChange={onChange}
          />
        )
      )}
    </>
  );
};

export default TimeSelect;
