import { ViewStyle } from "react-native";
import { Screen } from "../../../components/Screen";
import Header from "../../../components/Header";
import { Icon } from "../../../components/Icon";
import { router } from "expo-router";

const ChangePassowrd = () => {
  return (
    <Screen
      preset="auto"
      ScrollViewProps={{
        bounces: false,
      }}
      contentContainerStyle={$containerStyle}
    >
      <Header
        title={"Change Password"}
        titleColor="white"
        barStyle="light-content"
        leftIcon={<Icon icon="chevronLeft" color="white" />}
        onLeftPress={() => router.back()}
      />
    </Screen>
  );
};

const $containerStyle: ViewStyle = {
  flexGrow: 1,
};

export default ChangePassowrd;
