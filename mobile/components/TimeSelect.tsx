import React, { memo, useEffect, useState } from "react";
import { Typography } from "./Typography";
import { YStack, XStack, Sheet, Stack } from "tamagui";
import Button from "../components/Button";
import { Platform } from "react-native";
import RNDateTimePicker, {
  DateTimePickerAndroid,
  DateTimePickerEvent,
} from "@react-native-community/datetimepicker";

import CardFooter from "../components/CardFooter";

interface TimeSelectProps {
  type: "arrival" | "departure";
  time: Date | undefined;
  setTime: any;
  arrivalTime?: Date | undefined;
}

enum CardFooterDisplay {
  ARRIVAL = "Arrival",
  DEPARTURE = "Departure",
}

const TimeSelect: React.FC<TimeSelectProps> = memo(({ type, time, setTime, arrivalTime }) => {
  const [open, setOpen] = useState(false);

  // on ios we use a temporary time, as the onChange function gets triggered every time the user picks a new time
  // therefore, we will update the FINAL time state (that comes from the outside), only onDonePress
  const [tempTime, setTempTime] = useState(time || new Date());

  useEffect(() => {
    setTempTime(time ?? new Date());
  }, [time]);

  const onChange = (event: DateTimePickerEvent, selectedTime: Date | undefined) => {
    if (Platform.OS === "ios") {
      selectedTime ? setTempTime(selectedTime) : setTempTime(tempTime);
    } else {
      // press OK - set the time
      if (event.type === "set") {
        onClose();
        DateTimePickerAndroid.open({
          mode: "time",
          minimumDate: type === "departure" && arrivalTime ? arrivalTime : undefined,
          value: time || new Date(),
          onChange: (event, eventTime) => {
            if (event.type === "set" && selectedTime && eventTime) {
              selectedTime.setHours(eventTime?.getHours());
              selectedTime.setMinutes(eventTime?.getMinutes());
              setTime(selectedTime);
            }
          },
          is24Hour: true,
        });
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

  // const onResetTime = () => {
  //   // setting time to undefined
  //   setTime();
  //   // resetting temporary time
  //   setTempTime(new Date());
  //   // close the picker
  //   onClose();
  // };

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
            <React.Fragment>
              <Typography preset="body2" fontWeight="500" paddingBottom="$xxs">
                {time &&
                  time.toLocaleDateString(["en-GB"], {
                    month: "2-digit",
                    day: "2-digit",
                    year: "numeric",
                  })}
              </Typography>
              <Typography preset="heading" fontWeight="500">
                {time &&
                  time.toLocaleTimeString([], {
                    hour: "2-digit",
                    minute: "2-digit",
                  })}
              </Typography>
            </React.Fragment>
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
            <XStack gap="$sm" justifyContent="flex-end" width="100%">
              {/* <Button preset="outlined" onPress={onResetTime}>
                Reset
              </Button> */}
              <Button onPress={onDonePress}>Done</Button>
            </XStack>
            <XStack flex={1} justifyContent="center" alignItems="center">
              <RNDateTimePicker
                mode="datetime"
                display="spinner"
                value={tempTime}
                onChange={onChange}
                minimumDate={type === "departure" && arrivalTime ? arrivalTime : undefined}
              />
            </XStack>
          </Sheet.Frame>
        </Sheet>
      ) : (
        open &&
        // using imperative API for android
        DateTimePickerAndroid.open({
          mode: "date",
          value: time || new Date(),
          onChange,
          is24Hour: true,
          minimumDate: type === "departure" && arrivalTime ? arrivalTime : undefined,
        })
      )}
    </>
  );
});

export default TimeSelect;
