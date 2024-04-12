import { Redirect, Stack } from "expo-router";
import { useAuth } from "../../hooks/useAuth";
import { PortalProvider } from "tamagui";
import Header from "../../components/Header";
import { Icon } from "../../components/Icon";
import { getHeaderTitle } from "@react-navigation/elements";
import { StyleProp, ViewStyle } from "react-native";
import { useTheme } from "tamagui";
import { useTranslation } from "react-i18next";

const AppLayout = () => {
  const { isAuthenticated } = useAuth();
  const theme = useTheme();
  const { t } = useTranslation("polling_station_information");

  if (!isAuthenticated) {
    // On web, static rendering will stop here as the user is not authenticated
    // in the headless Node process that the pages are rendered in.
    return <Redirect href="/login" />;
  }

  return (
    <PortalProvider>
      <Stack>
        <Stack.Screen name="(drawer)" options={{ headerShown: false }} />
        <Stack.Screen
          name="polling-station-wizzard"
          options={{
            header: ({ navigation, route, options }) => {
              const title = getHeaderTitle(options, route.name);
              return (
                <Header
                  title={t("header.title")}
                  titleColor="white"
                  backgroundColor={theme.purple5?.val}
                  barStyle="light-content"
                  style={options.headerStyle as StyleProp<ViewStyle>}
                  leftIcon={<Icon icon="chevronLeft" color="white" />}
                  onLeftPress={() => navigation.goBack()}
                  rightIcon={<Icon icon="dotsVertical" color="white" />}
                  onRightPress={() => {
                    console.log("on right action press");
                  }}
                />
              );
            },
          }}
        />
        <Stack.Screen name="form-questionnaire" />
        <Stack.Screen name="polling-station-questionnaire" />
      </Stack>
    </PortalProvider>
  );
};

export default AppLayout;
