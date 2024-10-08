import React from "react";
import { Text } from "react-native";
import { useNavigation, useRouter } from "expo-router";
import { DrawerActions } from "@react-navigation/native";
import { Icon } from "../../../../../components/Icon";
import { useAppMode, AppMode } from "../../../../../contexts/app-mode/AppModeContext.provider";
import { Screen } from "../../../../../components/Screen";
import Button from "../../../../../components/Button";
import Header from "../../../../../components/Header";

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
          setAppMode(AppMode.ONBOARDING);
          router.push("/");
        }}
      >
        Go to onboarding
      </Button>
    </Screen>
  );
}
