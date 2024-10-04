import { Redirect, Stack } from "expo-router";
import React from "react";
import { useCitizenUserData } from "../../../contexts/citizen-user/CitizenUserContext.provider";

export default function MainLayout() {
  console.log("MainLayout");

  const { selectedElectionRound } = useCitizenUserData();

  console.log("👀 selectedElectionRound", selectedElectionRound);

  if (!selectedElectionRound) {
    return <Redirect href="/election-rounds" />;
  }

  return (
    <Stack>
      <Stack.Screen name="(drawer)" options={{ headerShown: false }} />
      <Stack.Screen name="questionnaire/[questionId]" options={{ headerShown: false }} />
    </Stack>
  );
}
