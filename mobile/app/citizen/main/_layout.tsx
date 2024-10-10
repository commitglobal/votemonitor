import React from "react";
import { Redirect, Stack } from "expo-router";
import { useCitizenUserData } from "../../../contexts/citizen-user/CitizenUserContext.provider";

export default function MainLayout() {
  const { selectedElectionRound, isLoading } = useCitizenUserData();

  if (!selectedElectionRound && !isLoading) {
    return <Redirect href="/citizen/select-election-rounds" />;
  }

  return (
    <Stack>
      <Stack.Screen name="(drawer)" options={{ headerShown: false }} />
      <Stack.Screen name="select-location" options={{ headerShown: false }} />
      <Stack.Screen name="form" options={{ headerShown: false }} />
    </Stack>
  );
}
