import React from "react";
import { Stack } from "expo-router";

export default function MainLayout() {
  return (
    <Stack>
      <Stack.Screen name="(drawer)" options={{ headerShown: false }} />
      <Stack.Screen name="select-location" options={{ headerShown: false }} />
      <Stack.Screen name="form" options={{ headerShown: false }} />
    </Stack>
  );
}
