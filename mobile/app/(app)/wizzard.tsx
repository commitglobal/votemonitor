import { View } from "react-native";
import { router } from "expo-router";
import { Button } from "tamagui";
import { Typography } from "../../components/Typography";

const Wizzard = () => {
  return (
    <View
      style={{
        flex: 1,
        justifyContent: "center",
        alignItems: "center",
        gap: 20,
      }}
    >
      <Typography>This is the wizzard</Typography>
    </View>
  );
};

export default Wizzard;
