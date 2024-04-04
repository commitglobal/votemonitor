import { View, Text } from "react-native";
import { useAuth } from "../../../../hooks/useAuth";
import OfflinePersistComponentExample from "../../../../components/OfflinePersistComponentExample";
import { StatusBar } from "react-native";
import { Button, Card } from "tamagui";
import { router } from "expo-router";
import CardFooter from "../../../../components/CardFooter";
import { Typography } from "../../../../components/Typography";

const Index = () => {
  const { signOut } = useAuth();

  return (
    <View style={{ gap: 20 }}>
      <StatusBar barStyle="light-content" />
      <OfflinePersistComponentExample></OfflinePersistComponentExample>
      <Button onPress={() => router.push("/polling-station-wizzard/1")}>
        Go To Polling station wizzard
      </Button>
      <Button onPress={() => router.push("/form-questionnaire/1")}>
        Go Form wizzard
      </Button>
      <Button onPress={() => router.push("/polling-station-questionnaire")}>
        Go To Polling Station Qustionnaire
      </Button>
      <Card padding="$md">
        <Typography>Card component</Typography>
        <CardFooter
          text="Card footer"
          action={() => console.log("card footer")}
          marginTop="$sm"
        />
      </Card>
      <Text onPress={signOut}>Logout</Text>
    </View>
  );
};

export default Index;
