import { useLocalSearchParams, useRouter } from "expo-router";
import { useTranslation } from "react-i18next";
import { useSafeAreaInsets } from "react-native-safe-area-context";
import Header from "../../../../components/Header";
import { Icon } from "../../../../components/Icon";
import { Screen } from "../../../../components/Screen";
import { Typography } from "../../../../components/Typography";
import { useElectionRoundsQuery } from "../../../../services/queries.service";
import { useElectionRoundStatistics } from "../../../../services/queries/er-statistics.query";

type SearchParamsType = {
  electionRoundId: string;
};

const ElectionRoundStatistics = () => {
  const { electionRoundId } = useLocalSearchParams<SearchParamsType>();
  const { data: electionRound, isLoading: isLoadingElectionRound } = useElectionRoundsQuery(
    (elections) => elections.find((e) => e.id === electionRoundId),
  );
  const { t } = useTranslation(["er-statistics", "common"]);
  const router = useRouter();
  const insets = useSafeAreaInsets();

  if (!electionRoundId) {
    return <Typography>Incorrect page params</Typography>;
  }

  const {
    data: erStatistics,
    isLoading: isLoadingERStatistics,
    error: eRStatisticsError,
    refetch: refetchERStatistics,
    isRefetching: isRefetchingERStatistics,
  } = useElectionRoundStatistics(electionRoundId);

  if (isLoadingElectionRound || isLoadingERStatistics) {
    return <Typography>{t("loading", { ns: "common" })}</Typography>;
  }

  return (
    <Screen
      preset="fixed"
      contentContainerStyle={{ flexGrow: 1, paddingBottom: 16 + insets.bottom }}
      style={{ backgroundColor: "white" }}
    >
      <Header
        title={"Your activity during election" + electionRoundId}
        leftIcon={<Icon icon="chevronLeft" color="white" />}
        onLeftPress={() => router.back()}
      />
      {erStatistics && electionRound && <Typography>hello!</Typography>}
    </Screen>
  );
};

export default ElectionRoundStatistics;
