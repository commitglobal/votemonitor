import React, { useState } from "react";
import { XStack } from "tamagui";
import RNDateTimePicker from "@react-native-community/datetimepicker";

export const DateInput = () => {
  const [date, setDate] = useState(new Date());

  const onChange = (event, selectedDate) => {
    const currentDate = selectedDate;
    setDate(currentDate);
  };

  return (
    <XStack>
      <RNDateTimePicker value={date} onChange={onChange} />
    </XStack>
  );
};
