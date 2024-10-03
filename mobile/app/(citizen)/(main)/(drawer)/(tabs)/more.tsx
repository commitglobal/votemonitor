import React from "react";
import { Icon } from "../../../../../components/Icon";
import { Screen } from "../../../../../components/Screen";
import Header from "../../../../../components/Header";
import { Text } from "react-native";
import { useNavigation, useRouter } from "expo-router";
import { DrawerActions } from "@react-navigation/native";
import Button from "../../../../../components/Button";
import { useAppMode } from "../../../../../contexts/app-mode/AppModeContext.provider";

export default function More() {
  const navigation = useNavigation();
  const { appMode, setAppMode } = useAppMode();
  const router = useRouter();
  console.log("render Mode", appMode);

  return (
    <Screen preset="fixed" contentContainerStyle={{ flexGrow: 1 }}>
      <Header
        title="More"
        titleColor="white"
        barStyle="light-content"
        leftIcon={<Icon icon="menuAlt2" color="white" />}
        onLeftPress={() => navigation.dispatch(DrawerActions.openDrawer)}
        rightIcon={<Icon icon="dotsVertical" color="white" />}
        onRightPress={() => {}}
      />

      <Text>More Index </Text>
      <Button
        onPress={() => {
          setAppMode("onboarding");
          router.push("/");
        }}
      >
        Go to onboarding
      </Button>
    </Screen>
  );
}
