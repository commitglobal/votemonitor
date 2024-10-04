import { Stack } from "expo-router";
import React from "react";
import CitizenUserContextProvider from "../../contexts/citizen-user/CitizenUserContext.provider";

const CitizenLayout = () => {
  console.log("CitizenLayout");

  return (
    <CitizenUserContextProvider>
      <Stack screenOptions={{ headerShown: false }}>
        <Stack.Screen name="(main)" />
      </Stack>
    </CitizenUserContextProvider>
  );
};

export default CitizenLayout;
