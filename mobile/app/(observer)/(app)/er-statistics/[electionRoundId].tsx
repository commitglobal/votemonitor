import { useLocalSearchParams, useRouter } from "expo-router";
import { useTranslation } from "react-i18next";
import { RefreshControl } from "react-native";
import { ScrollView, YStack } from "tamagui";
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
  const { t } = useTranslation(["er_statistics", "common"]);
  const router = useRouter();

  if (!electionRoundId) {
    return <Typography>Incorrect page params</Typography>;
  }

  const {
    data: erStatistics,
    isLoading: isLoadingERStatistics,
    error: erStatisticsError,
    refetch: refetchERStatistics,
    isRefetching: isRefetchingERStatistics,
  } = useElectionRoundStatistics(electionRoundId);

  if (isLoadingElectionRound || isLoadingERStatistics) {
    return <Typography>{t("loading", { ns: "common" })}</Typography>;
  }

  if (erStatisticsError) {
    return (
      <Screen preset="fixed" contentContainerStyle={{ flexGrow: 1 }}>
        <Header
          title={t("header")}
          leftIcon={<Icon icon="chevronLeft" color="white" />}
          onLeftPress={() => router.back()}
        />
        <YStack flex={1}>
          <ScrollView
            showsVerticalScrollIndicator={false}
            contentContainerStyle={{ flex: 1, alignItems: "center", flexGrow: 1 }}
            paddingVertical="$xxl"
            refreshControl={
              <RefreshControl
                refreshing={isRefetchingERStatistics}
                onRefresh={refetchERStatistics}
              />
            }
          >
            <Typography>{t("error")}</Typography>
          </ScrollView>
        </YStack>
      </Screen>
    );
  }

  return (
    <Screen
      preset="fixed"
      contentContainerStyle={{
        flexGrow: 1,
      }}
      backgroundColor="white"
    >
      <Header
        title={t("header")}
        leftIcon={<Icon icon="chevronLeft" color="white" />}
        onLeftPress={() => router.back()}
      />

      <YStack flex={1}>
        <ScrollView
          contentContainerStyle={{
            paddingVertical: 32,
            gap: 32,
            flexGrow: 1,
            paddingHorizontal: 16,
          }}
          showsVerticalScrollIndicator={false}
          refreshControl={
            <RefreshControl refreshing={isRefetchingERStatistics} onRefresh={refetchERStatistics} />
          }
        >
          <YStack gap={16}>
            <Typography preset="heading" fontWeight="700">
              {t("title", {
                title: electionRound?.title || "",
                englishTitle: electionRound?.englishTitle || "",
              })}
            </Typography>
            <Typography preset="body1">
              {t("forms_submitted", { value: erStatistics?.numberOfFormsSubmitted || 0 })}
            </Typography>
            <Typography preset="body1">
              {t("questions_answered", { value: erStatistics?.numberOfQuestionsAnswered || 0 })}
            </Typography>
            <Typography preset="body1">
              {t("quick_reports", { value: erStatistics?.numberOfQuickReports || 0 })}
            </Typography>
            <Typography preset="body1">
              {t("notes_taken", { value: erStatistics?.numberOfNotes || 0 })}
            </Typography>
            <Typography preset="body1">
              {t("attachments_sent", { value: erStatistics?.numberOfAttachments || 0 })}
            </Typography>
            <Typography preset="body1">
              {t("visited_ps", { value: erStatistics?.numberOfPollingStationsVisited || 0 })}
            </Typography>
          </YStack>
        </ScrollView>
      </YStack>
    </Screen>
  );
};

export default ElectionRoundStatistics;
