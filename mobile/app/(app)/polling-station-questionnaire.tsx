import { View } from "react-native";
import { Typography } from "../../components/Typography";

const PollingStationQuestionnaire = () => {
  return (
    <View
      style={{
        flex: 1,
        justifyContent: "center",
        alignItems: "center",
        gap: 20,
      }}
    >
      <Typography>This is the polling station questionnaire</Typography>
    </View>
  );
};

export default PollingStationQuestionnaire;
