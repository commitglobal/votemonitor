import { ScrollView, YStack } from "tamagui";
import { Screen } from "./Screen";
import { Icon } from "./Icon";
import { Typography } from "./Typography";
import Header from "./Header";
import { useNavigation } from "expo-router";
import { DrawerActions } from "@react-navigation/native";
import { useTranslation } from "react-i18next";
import { RefreshControl } from "react-native";
import { useElectionRoundsQuery } from "../services/queries.service";
import { useNetInfoContext } from "../contexts/net-info-banner/NetInfoContext";

const NoElectionRounds = () => {
  const navigation = useNavigation();
  const { t } = useTranslation("observation");
  const { isOnline } = useNetInfoContext();
  const { isRefetching: isRefetchingRounds, refetch: refetchRounds } = useElectionRoundsQuery();

  return (
    <Screen preset="fixed" contentContainerStyle={{ flexGrow: 1 }}>
      <Header
        barStyle="light-content"
        leftIcon={<Icon icon="menuAlt2" color="white" />}
        onLeftPress={() => navigation.dispatch(DrawerActions.openDrawer)}
        title={t("title")}
      />
      <ScrollView
        contentContainerStyle={{
          flexGrow: 1,
          alignItems: "center",
          justifyContent: "center",
        }}
        showsVerticalScrollIndicator={false}
        backgroundColor="white"
        bounces={isOnline}
        refreshControl={
          <RefreshControl refreshing={isRefetchingRounds} onRefresh={refetchRounds} />
        }
      >
        <YStack width={312} alignItems="center">
          <Icon icon="peopleAddingVote" marginBottom="$md" />
          <Typography preset="subheading" textAlign="center" marginBottom="$xxxs">
            {t("no_election_round.heading")}
          </Typography>
          <Typography preset="body1" textAlign="center" color="$gray5">
            {t("no_election_round.paragraph")}
          </Typography>
        </YStack>
      </ScrollView>
    </Screen>
  );
};

export default NoElectionRounds;
