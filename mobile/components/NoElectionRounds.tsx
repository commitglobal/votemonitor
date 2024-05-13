import { Stack, YStack } from "tamagui";
import { Screen } from "./Screen";
import { Icon } from "./Icon";
import { Typography } from "./Typography";
import Header from "./Header";
import { useNavigation } from "expo-router";
import { DrawerActions } from "@react-navigation/native";

const NoElectionRounds = () => {
  const navigation = useNavigation();

  return (
    <Screen preset="fixed" contentContainerStyle={{ flexGrow: 1 }}>
      <Header
        barStyle="light-content"
        leftIcon={<Icon icon="menuAlt2" color="white" />}
        onLeftPress={() => navigation.dispatch(DrawerActions.openDrawer)}
      />
      <Stack backgroundColor="white" alignItems="center" justifyContent="center" flex={1}>
        <YStack width={312} alignItems="center">
          <Icon icon="peopleAddingVote" marginBottom="$md" />
          <Typography preset="subheading" textAlign="center" marginBottom="$xxxs">
            No election event to observe yet
          </Typography>
          <Typography preset="body1" textAlign="center" color="$gray5">
            You will be able to use the app once you will be assigned to an election event by your
            organization
          </Typography>
        </YStack>
      </Stack>
    </Screen>
  );
};

export default NoElectionRounds;
