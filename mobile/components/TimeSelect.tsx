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
import { useTranslation } from "react-i18next";

interface TimeSelectProps {
  type: "arrival" | "departure";
  time: Date | undefined;
  setTime: any;
  arrivalTime?: Date | undefined;
  departureTime?: Date | undefined;
}

const TimeSelect: React.FC<TimeSelectProps> = memo(
  ({ type, time, setTime, arrivalTime, departureTime }) => {
    const { t, i18n } = useTranslation("observation");
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
        if (event.type === "set") {
          // if we're trying to set a departure time before having set the arrival time -> close picker and display error toast
          if (type === "departure" && !arrivalTime) {
            onClose();
            return Toast.show({
              type: "error",
              text2: t("polling_stations_information.error.arrival_first"),
              visibilityTime: 5000,
            });
          }
          // after setting the date, close date picker and open time picker
          onClose();
          DateTimePickerAndroid.open({
            mode: "time",
            value: time || new Date(),
            onChange: (event, eventTime) => {
              if (event.type === "set" && selectedTime && eventTime) {
                // we need to set the date of the time picker, as it uses the current date as default
                eventTime.setDate(selectedTime.getDate());
                // setting departure time and we have an arrival time set
                if (type === "departure" && arrivalTime) {
                  // setting time for the same day -> don't allow an earlier departure time
                  if (eventTime.getDate() === selectedTime.getDate() && eventTime < arrivalTime) {
                    onClose();
                    return Toast.show({
                      type: "error",
                      text2: t("polling_stations_information.error.later_departure"),
                      visibilityTime: 5000,
                    });
                  }
                } else if (type === "arrival" && departureTime) {
                  // if departureTime is set and we're setting the time for the same day -> don't allow a later arrival time
                  if (eventTime.getDate() === selectedTime.getDate() && eventTime > departureTime) {
                    onClose();
                    return Toast.show({
                      type: "error",
                      text2: t("polling_stations_information.error.earlier_arrival"),
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
          text2: t("polling_stations_information.time_select.error.arrival_first"),
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
                {t("polling_stations_information.time_select.not_defined")}
              </Typography>
            )}
          </Stack>

          <CardFooter
            text={
              type === "arrival"
                ? t("polling_stations_information.time_select.arrival_time")
                : t("polling_stations_information.time_select.departure_time")
            }
            marginTop="auto"
          ></CardFooter>
        </YStack>

        {Platform.OS === "ios" ? (
          <Sheet
            modal
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
                <Button onPress={onDonePress}>
                  {t("polling_stations_information.time_select.save")}
                </Button>
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
                  locale={i18n.language}
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
            maximumDate: type === "arrival" && departureTime ? departureTime : undefined,
          })
        )}
      </>
    );
  },
);

export default TimeSelect;
