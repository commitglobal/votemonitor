import React from "react";
import { Icon } from "../../../../../components/Icon";
import { Screen } from "../../../../../components/Screen";
import Header from "../../../../../components/Header";
import { Text } from "react-native";
import { useNavigation, useRouter } from "expo-router";
import { DrawerActions } from "@react-navigation/native";
import Button from "../../../../../components/Button";

export default function Report() {
  const navigation = useNavigation();
  const router = useRouter();

  return (
    <Screen preset="fixed" contentContainerStyle={{ flexGrow: 1 }}>
      <Header
        title="Report"
        titleColor="white"
        barStyle="light-content"
        leftIcon={<Icon icon="menuAlt2" color="white" />}
        onLeftPress={() => navigation.dispatch(DrawerActions.openDrawer)}
        rightIcon={<Icon icon="dotsVertical" color="white" />}
        onRightPress={() => {}}
      />

      <Text>Report Index </Text>
      <Button onPress={() => router.push("/questionnaire/1")}>Go to Questionnaire</Button>
    </Screen>
  );
}
