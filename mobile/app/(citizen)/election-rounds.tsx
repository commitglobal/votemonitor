import { Text } from "react-native";
import React from "react";
import Button from "../../components/Button";
import { Screen } from "../../components/Screen";
import { useCitizenUserData } from "../../contexts/citizen-user/CitizenUserContext.provider";
import { router } from "expo-router";

function CitizenElectionRoundsSelector() {
  const { setSelectedElectionRound } = useCitizenUserData();

  return (
    <Screen preset="fixed" contentContainerStyle={{ flexGrow: 1 }}>
      <Text style={{ marginBottom: 125 }}>Election Rounds</Text>
      <Button
        onPress={() => {
          setSelectedElectionRound(true);
          router.push("(main)");
        }}
      >
        Continue
      </Button>
    </Screen>
  );
}

export default CitizenElectionRoundsSelector;