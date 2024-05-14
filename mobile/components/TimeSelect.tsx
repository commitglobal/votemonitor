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
import Toast from "react-native-toast-message";

interface TimeSelectProps {
  type: "arrival" | "departure";
  time: Date | undefined;
  setTime: any;
  arrivalTime?: Date | undefined;
  departureTime?: Date | undefined;
}

enum CardFooterDisplay {
  ARRIVAL = "Arrival",
  DEPARTURE = "Departure",
}

const TimeSelect: React.FC<TimeSelectProps> = memo(
  ({ type, time, setTime, arrivalTime, departureTime }) => {
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
          // if we're trying to set a departure time before having set the arrival time -> close picker and display error toast
          if (type === "departure" && !arrivalTime) {
            onClose();
            return Toast.show({
              type: "error",
              text2: "Please select an arrival time first. ",
              visibilityTime: 5000,
            });
          }
          onClose();
          DateTimePickerAndroid.open({
            mode: "time",
            value: time || new Date(),
            onChange: (event, eventTime) => {
              if (event.type === "set" && selectedTime && eventTime) {
                // if arrivalTime is set -> don't allow an earlier departure time
                if (type === "departure" && arrivalTime) {
                  if (eventTime < arrivalTime) {
                    onClose();
                    return Toast.show({
                      type: "error",
                      text2:
                        "Please select a departure time that is at a later time than the arrival.",
                      visibilityTime: 5000,
                    });
                  }
                } else if (type === "arrival" && departureTime) {
                  // if departureTime is set -> don't allow a later arrival time
                  if (eventTime > departureTime) {
                    onClose();
                    return Toast.show({
                      type: "error",
                      text2:
                        "Please select an arrival time that is at an earlier time than the departure.",
                      visibilityTime: 5000,
                    });
                  }
                }
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
      // if we're trying to set a departure time before having set the arrival time -> error toast
      if (type === "departure" && !arrivalTime) {
        Toast.show({
          type: "error",
          text2: "Please select an arrival time first.",
          visibilityTime: 5000,
        });
        return onClose();
      }
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
          flex={1}
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
            marginTop="auto"
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
                  // if setting departure time and arrival time has already been set -> don't allow an earlier time
                  minimumDate={type === "departure" && arrivalTime ? arrivalTime : undefined}
                  // if setting arrival time and departure time has already been set -> don't allow a later time
                  maximumDate={type === "arrival" && departureTime ? departureTime : undefined}
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
            // if setting departure date and arrival date has already been set -> don't allow an earlier date
            minimumDate: type === "departure" && arrivalTime ? arrivalTime : undefined,
          })
        )}
      </>
    );
  },
);

export default TimeSelect;
