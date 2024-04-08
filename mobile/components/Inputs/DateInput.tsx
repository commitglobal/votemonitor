import React, { useState } from "react";
import { XStack } from "tamagui";
import RNDateTimePicker from "@react-native-community/datetimepicker";

export const DateInput = () => {
  const [date, setDate] = useState(new Date());
  const [open, setOpen] = useState(false);

  const onChange = (event, selectedDate) => {
    const currentDate = selectedDate;
    setDate(currentDate);
  };

  return (
    <XStack backgroundColor="red" height={24} onPress={() => setOpen(true)}>
      {open && <RNDateTimePicker value={date} onChange={onChange} />}
    </XStack>
  );
};
