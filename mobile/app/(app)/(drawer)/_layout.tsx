import { GestureHandlerRootView } from "react-native-gesture-handler";
import { Drawer } from "expo-router/drawer";
import { getHeaderTitle } from "@react-navigation/elements";
import { View, Text, XStack, useTheme } from "tamagui";
import { useSafeAreaInsets } from "react-native-safe-area-context";
import React from "react";
import Eye from "../../../assets/icons/Eye.svg";
import {
  StyleProp,
  TouchableOpacity,
  TouchableOpacityProps,
  ViewStyle,
} from "react-native";
import ObservationSVG from "../../../assets/icons/observation.svg";
import { DrawerActions } from "@react-navigation/native";
import { Typography } from "../../../components/Typography";

export default function MainLayout() {
  const theme = useTheme();
  return (
    <GestureHandlerRootView style={{ flex: 1 }}>
      <Drawer
        screenOptions={{
          drawerType: "front",
          headerStyle: {
            backgroundColor: theme?.purple5?.val,
          },
          header: ({ navigation, route, options }) => {
            const title = getHeaderTitle(options, route.name);
            return (
              <Header
                navigation={navigation}
                title={title}
                titleColor="white"
                style={options.headerStyle}
                leftIcon={<Eye fill="white" />}
                leftIconColor="white"
                onLeftPress={() =>
                  navigation.dispatch(DrawerActions.openDrawer)
                }
                rightIcon={<ObservationSVG fill="white" />}
              />
            );
          },
        }}
      >
        <Drawer.Screen name="(tabs)" />
      </Drawer>
    </GestureHandlerRootView>
  );
}

interface HeaderProps {
  title: string;
  titleColor?: string;
  style?: StyleProp<ViewStyle>;
  // leftIcon
  leftIconColor?: string;
  rightIconColor?: string;
  // rightIcon
  onLeftPress?: TouchableOpacityProps["onPress"];
  onRightPress?: TouchableOpacityProps["onPress"];
}

const Header = ({
  navigation,
  title,
  titleColor,
  style,
  leftIcon,
  leftIconColor,
  onLeftPress,
  rightIcon,
  rightIconColor,
  onRightPress,
}: HeaderProps) => {
  const insets = useSafeAreaInsets();
  const theme = useTheme();

  return (
    <XStack
      style={{
        ...style,
        height: 50 + insets.top,
        paddingTop: insets.top,
      }}
      justifyContent="space-between"
      alignItems="center"
    >
      {/* left header */}

      <TouchableOpacity
        onPress={onLeftPress ? onLeftPress : () => void 0}
        style={{
          flex: 1,
          backgroundColor: "green",
        }}
      >
        {leftIcon ? leftIcon : null}
      </TouchableOpacity>

      <Typography
        preset="body2"
        style={{
          color: { titleColor },
          flex: 6,
          textAlign: "center",
        }}
      >
        {title}
      </Typography>

      {/* right header */}
      <TouchableOpacity
        onPress={onRightPress ? onRightPress : () => void 0}
        style={{
          // for larger tapping space
          flex: 1,
          backgroundColor: "green",
          flexDirection: "row",
          justifyContent: "flex-end",
        }}
      >
        {rightIcon ? rightIcon : null}
      </TouchableOpacity>
    </XStack>
  );
};
