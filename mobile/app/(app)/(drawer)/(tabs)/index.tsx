import { View, Text, StatusBar } from "react-native";
import { useAuth } from "../../../../hooks/useAuth";
import OfflinePersistComponentExample from "../../../../components/OfflinePersistComponentExample";
import { Button, RadioGroup } from "tamagui";
import { router } from "expo-router";
import Card from "../../../../components/Card";
import Input from "../../../../components/Inputs/Input";
import CheckboxInput from "../../../../components/Inputs/CheckboxInput";
import RadioInput from "../../../../components/Inputs/RadioInput";
import { Typography } from "../../../../components/Typography";
import { useState } from "react";

const Index = () => {
  const { signOut } = useAuth();
  const [selectedRadioValue, setSelectedRadioValue] = useState("rural");

  return (
    <View style={{ gap: 20 }}>
      <Card>
        <Input />
        <CheckboxInput label="hello" marginTop={20} id="1" />
        <CheckboxInput label="hello2" marginTop={10} id="2" />

        <Typography preset="subheading" marginVertical="$md">
          Radio buttons
        </Typography>
        <RadioGroup
          gap="$sm"
          defaultValue={selectedRadioValue}
          onValueChange={(value) => setSelectedRadioValue(value)}
        >
          <RadioInput id="10" value="rural" label="Rural" selectedValue={selectedRadioValue} />
          <RadioInput id="20" value="urban" label="Urban" selectedValue={selectedRadioValue} />
          <RadioInput
            id="30"
            value="not-known"
            label="Not known"
            selectedValue={selectedRadioValue}
          />
        </RadioGroup>
      </Card>

      <StatusBar barStyle="light-content" />
      <OfflinePersistComponentExample></OfflinePersistComponentExample>
      <Button onPress={() => router.push("/polling-station-wizzard/1")}>
        Go To Polling station wizzard
      </Button>
      <Button onPress={() => router.push("/form-questionnaire/1")}>Go Form wizzard</Button>
      <Button onPress={() => router.push("/polling-station-questionnaire")}>
        Go To Polling Station Qustionnaire
      </Button>
      <Text onPress={signOut}>Logout</Text>
    </View>
  );
};

export default Index;
