import { Screen } from "../../components/Screen";
import Header from "../../components/Header";
import { Icon } from "../../components/Icon";
import { YStack } from "tamagui";
import { Typography } from "../../components/Typography";
import { router } from "expo-router";
import { useUserData } from "../../contexts/user/UserContext.provider";
import { useTranslation } from "react-i18next";
import PollingStationCard from "../../components/PollingStationCard";

const ManagePollingStation = () => {
  const { t } = useTranslation("manage_polling_stations");
  const { visits } = useUserData();
  if (visits === undefined || visits.length === 0) {
    return <Typography>No visits</Typography>;
  }

  console.log("visits", visits.length);

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

      <YStack gap="$lg" paddingTop="$lg" paddingHorizontal="$md">
        {visits.map((visit) => (
          <PollingStationCard visit={visit} key={visit.pollingStationId} />
        ))}
      </YStack>
    </Screen>
  );
};

export default ManagePollingStation;
