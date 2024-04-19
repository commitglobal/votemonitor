import { Screen } from "../components/Screen";
import { Icon } from "../components/Icon";
import { View } from "tamagui";

const SplashScreen = () => {
  return (
    // <Screen
    //   preset="auto"
    //   contentContainerStyle={{
    //     flexGrow: 1,
    //     backgroundColor: "#5F288D",
    //     alignItems: "center",
    //     justifyContent: "center",
    //   }}
    // >
    //   <Icon icon="loginLogo" />
    // </Screen>
    <View
      style={{
        backgroundColor: "#5F288D",
        alignItems: "center",
        justifyContent: "center",
        flex: 1,
      }}
    >
      <Icon icon="loginLogo" />
    </View>
  );
};

export default SplashScreen;
