import React from "react";
import { router, useNavigation } from "expo-router";
import { ScrollView, YStack } from "tamagui";
import { Screen } from "./Screen";
import { Icon } from "./Icon";
import { Typography } from "./Typography";
import Button from "./Button";
import Header from "./Header";
import { DrawerActions } from "@react-navigation/native";
import { useTranslation } from "react-i18next";
import { usePollingStationsVisits } from "../services/queries.service";
import { ElectionRoundVM } from "../common/models/election-round.model";
import { RefreshControl } from "react-native";

const NoVisitsExist = ({
  activeElectionRound,
}: {
  activeElectionRound: ElectionRoundVM | undefined;
}) => {
  const navigation = useNavigation();
  const { t } = useTranslation("observation");

  const { refetch: refetchVisits, isRefetching: isRefetchingVisits } = usePollingStationsVisits(
    activeElectionRound?.id,
  );

  return (
    <Screen preset="fixed" contentContainerStyle={{ flexGrow: 1 }}>
      <Header
        title={""}
        titleColor="white"
        barStyle="light-content"
        leftIcon={<Icon icon="menuAlt2" color="white" />}
        onLeftPress={() => navigation.dispatch(DrawerActions.openDrawer)}
      />
      <ScrollView
        showsVerticalScrollIndicator={false}
        contentContainerStyle={{
          justifyContent: "center",
          alignItems: "center",
          flex: 1,
          gap: 16,
        }}
        backgroundColor="white"
        paddingHorizontal="$lg"
        refreshControl={
          <RefreshControl refreshing={isRefetchingVisits} onRefresh={refetchVisits} />
        }
      >
        <Icon icon="missingPollingStation" />
        <YStack gap="$xxxs">
          <Typography preset="subheading" textAlign="center">
            {t("no_visited_polling_stations.heading")}
          </Typography>
          <Typography preset="body1" textAlign="center" color="$gray5">
            {t("no_visited_polling_stations.paragraph")}
          </Typography>
        </YStack>
        <Button
          preset="outlined"
          backgroundColor="white"
          width="100%"
          onPress={router.push.bind(null, "/polling-station-wizzard")}
        >
          {t("no_visited_polling_stations.add")}
        </Button>
      </ScrollView>
    </Screen>
  );
};

export default NoVisitsExist;
