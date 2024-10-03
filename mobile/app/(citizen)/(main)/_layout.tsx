import { Redirect } from "expo-router";
import React from "react";

export default function MainLayout() {
  console.log("MainLayout");

  const selectedElectionRound = false;

  if (!selectedElectionRound) {
    return <Redirect href="/election-rounds" />;
  }

  return <></>;
}
