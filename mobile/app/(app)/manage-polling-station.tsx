import { Screen } from "../../components/Screen";
import Header from "../../components/Header";
import { Icon } from "../../components/Icon";
import { YStack } from "tamagui";
import { Typography } from "../../components/Typography";
import { router } from "expo-router";
import { useUserData } from "../../contexts/user/UserContext.provider";
import { useTranslation } from "react-i18next";
import PollingStationCard from "../../components/PollingStationCard";
import { useState } from "react";
import { PollingStationVisitVM } from "../../common/models/polling-station.model";

const ManagePollingStation = () => {
  const { t } = useTranslation("manage_polling_stations");
  const { visits } = useUserData();
  const [selectedPS, setSelectedPS] = useState<PollingStationVisitVM | null>(null);

  if (visits === undefined || visits.length === 0) {
    return <Typography>No visits</Typography>;
  }

  console.log("🛑visits", visits);

  return (
    <Screen
      preset="scroll"
      ScrollViewProps={{
        showsVerticalScrollIndicator: false,
        stickyHeaderIndices: [0],
        bounces: false,
      }}
      backgroundColor="white"
    >
      <Header
        title={t("header.title")}
        titleColor="white"
        barStyle="light-content"
        leftIcon={<Icon icon="chevronLeft" color="white" />}
        onLeftPress={() => router.back()}
      />

      {/* //todo: flashlist here */}
      <YStack gap="$lg" paddingTop="$lg" paddingHorizontal="$md" backgroundColor={"red"}>
        {visits.map((visit) => (
          <PollingStationCard
            visit={visit}
            key={visit.pollingStationId}
            onPress={() => setSelectedPS(visit)}
          />
        ))}
      </YStack>
    </Screen>
  );
};

export default ManagePollingStation;
