import React, { ReactPropTypes, useState } from "react";
import GeneralCard from "../GeneralCard";
import { H1, H2, H3, H4, Text, XStack, Sheet, Button } from "tamagui";
import { ChevronRight } from "@tamagui/lucide-icons";
import DateTimePicker from "@react-native-community/datetimepicker";
import RNDateTimePicker, {
  DateTimePickerAndroid,
} from "@react-native-community/datetimepicker";
import { Platform } from "react-native";

const TimeCard = (props: ReactPropTypes) => {
  const [open, setOpen] = useState(false);
  const [time, setTime] = useState(new Date());

  // const onChange = (event, selectedTime) => {
  //   const currentTime = selectedTime;
  //   setTime(currentTime);
  // };

  // const showTimepicker = () => {
  //   showMode("time");
  // };

  // const showMode = (currentTime) => {
  //   DateTimePickerAndroid.open({
  //     value: time,
  //     onChange,
  //     mode: currentTime,
  //     is24Hour: true,
  //   });
  // };

  return (
    <GeneralCard {...props}>
      <H3 marginBottom="$2" color="#A1A1AA">
        Not defined
      </H3>

      <XStack
        alignItems="center"
        justifyContent="space-between"
        onPress={() => setOpen(true)}
      >
        <Text>Arrival time</Text>
        <ChevronRight color="#7833B3" />
        {/* {open && (
          <RNDateTimePicker
            value={time}
            mode="time"
            is24Hour={true}
            display="spinner"
          />
        )} */}
      </XStack>

      <Sheet
        modal
        native
        open={open}
        onOpenChange={setOpen}
        zIndex={100_000}
        snapPoints={[35]}
      >
        <Sheet.Overlay />
        <Sheet.Frame justifyContent="center" alignItems="center">
          <RNDateTimePicker
            value={time}
            mode="time"
            is24Hour={true}
            display="spinner"
          />
          {/* <Button onPress={showTimepicker}>"Show time picker!"</Button> */}
        </Sheet.Frame>
      </Sheet>
    </GeneralCard>
  );
};

export default TimeCard;
