import { Text } from "react-native";
import React from "react";
import Button from "../../components/Button";
import { Screen } from "../../components/Screen";
import { router } from "expo-router";

function CitizenElectionRoundsSelector() {
  return (
    <Screen preset="fixed" contentContainerStyle={{ flexGrow: 1 }}>
      <Text>More Index </Text>
      <Button
        onPress={() => {
          router.replace("(main)");
        }}
      >
        Du-te-n mm
      </Button>
    </Screen>
  );
}

export default CitizenElectionRoundsSelector;
