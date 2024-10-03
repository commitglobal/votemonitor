import { Slot } from "expo-router";
import React from "react";
import { Text } from "react-native";

const ObserverLayout = () => {
  console.log("ObserverLayout");
  return (
    <>
      <Text>Observer Layout</Text>
      <Slot />
    </>
  );
};

export default ObserverLayout;
