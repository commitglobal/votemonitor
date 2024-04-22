import React from "react";
import { Typography } from "../../../../components/Typography";
import { YStack } from "tamagui";
import { Icon } from "../../../../components/Icon";
import { Screen } from "../../../../components/Screen";
import { useNavigation } from "expo-router";
import Header from "../../../../components/Header";
import { DrawerActions } from "@react-navigation/native";

const QuickReport = () => {
  const navigation = useNavigation();

  return (
    <Screen
      preset="auto"
      ScrollViewProps={{
        bounces: false,
      }}
      contentContainerStyle={{
        flexGrow: 1,
      }}
    >
      <Header
        title={"Quick Report"}
        titleColor="white"
        barStyle="light-content"
        leftIcon={<Icon icon="menuAlt2" color="white" />}
        onLeftPress={() => navigation.dispatch(DrawerActions.openDrawer)}
        rightIcon={<Icon icon="dotsVertical" color="white" />}
        onRightPress={() => {
          console.log("Right icon pressed");
        }}
      />

      <YStack flex={1} alignItems="center" justifyContent="center" gap="$md">
        <Icon icon="loadingScreenDevice" size={190} />

        <YStack gap="$xs" paddingHorizontal="$lg">
          <Typography preset="subheading" textAlign="center">
            Quick Reports
          </Typography>
          <Typography preset="body1" textAlign="center" color="$gray12">
            Here you'll be able to submit quick reports. Stay tuned! 👀
          </Typography>
        </YStack>
      </YStack>
    </Screen>
  );
};

export default QuickReport;
