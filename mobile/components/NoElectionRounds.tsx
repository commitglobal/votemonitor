import { Stack, YStack } from "tamagui";
import { Screen } from "./Screen";
import { Icon } from "./Icon";
import { Typography } from "./Typography";
import Header from "./Header";
import { useNavigation } from "expo-router";
import { DrawerActions } from "@react-navigation/native";
import { useTranslation } from "react-i18next";

const NoElectionRounds = () => {
  const navigation = useNavigation();
  const { t } = useTranslation("observation");

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
            {t("no_election_round.heading")}
          </Typography>
          <Typography preset="body1" textAlign="center" color="$gray5">
            {t("no_election_round.paragraph")}
          </Typography>
        </YStack>
      </Stack>
    </Screen>
  );
};

export default NoElectionRounds;
