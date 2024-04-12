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
  setTime: any;
}

enum CardFooterDisplay {
  ARRIVAL = "Arrival",
  DEPARTURE = "Departure",
}

const TimeSelect: React.FC<TimeSelectProps> = ({ type, time, setTime }) => {
  const [open, setOpen] = useState(false);

  // on ios we use a temporary time, as the onChange function gets triggered every time the user picks a new time
  // therefore, we will update the FINAL time state (that comes from the outside), only onDonePress
  const [tempTime, setTempTime] = useState(new Date());

  const onChange = (event: DateTimePickerEvent, selectedTime: Date | undefined) => {
    if (Platform.OS === "ios") {
      selectedTime ? setTempTime(selectedTime) : setTempTime(tempTime);
    } else {
      // press OK - set the time
      if (event.type === "set") {
        onClose();
        setTime(selectedTime);
      } else if (event.type === "dismissed") {
        // press Cancel - close modal
        onClose();
      }
    }
  };

  const onDonePress = () => {
    setTime(tempTime);
    onClose();
  };

  const onResetTime = () => {
    // setting time to undefined
    setTime();
    // resetting temporary time
    setTempTime(new Date());
    // close the picker
    onClose();
  };

  const onClose = () => {
    setOpen(false);
  };

  return (
    <>
      <YStack
        onPress={() => {
          setOpen(true);
        }}
      >
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

        <CardFooter
          text={`${type === "arrival" ? CardFooterDisplay.ARRIVAL : CardFooterDisplay.DEPARTURE} time`}
        ></CardFooter>
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
              <Button onPress={onDonePress}>Done</Button>
            </XStack>
            <XStack flex={1} justifyContent="center" alignItems="center">
              <RNDateTimePicker
                mode="time"
                display="spinner"
                value={tempTime || new Date()}
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
