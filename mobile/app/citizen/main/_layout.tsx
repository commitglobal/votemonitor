import React from "react";
import { Stack } from "expo-router";

export default function MainLayout() {
  console.log("🔵 (citizen) > main > _layout");

  return (
    <Stack>
      <Stack.Screen name="(drawer)" options={{ headerShown: false }} />
      <Stack.Screen name="questionnaire/[questionId]" options={{ headerShown: false }} />
    </Stack>
  );
}
