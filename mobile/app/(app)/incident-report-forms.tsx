import { router } from "expo-router";
import React from "react";
import { useTranslation } from "react-i18next";
import { YStack } from "tamagui";
import Header from "../../components/Header";
import { Icon } from "../../components/Icon";
import IncidentReportingFormList from "../../components/IncidentReportingFormList";
import NoElectionRounds from "../../components/NoElectionRounds";
import { Screen } from "../../components/Screen";
import ObservationSkeleton from "../../components/SkeletonLoaders/ObservationSkeleton";
import { Typography } from "../../components/Typography";
import { useUserData } from "../../contexts/user/UserContext.provider";

const IncidentReportForms = () => {
  const { t } = useTranslation("incident_report");

  const { isLoading, activeElectionRound } = useUserData();

  if (!isLoading && !activeElectionRound) {
    return <NoElectionRounds />;
  }

  return (
    <Screen preset="fixed" contentContainerStyle={{ flexGrow: 1 }}>
      <YStack marginBottom={20}>
        <Header
          title={t("title")}
          titleColor="white"
          barStyle="light-content"
          leftIcon={<Icon icon="chevronLeft" color="white" />}
          onLeftPress={() => router.back()}
        />
      </YStack>

      <YStack paddingHorizontal="$md" flex={1}>
        {isLoading && <ObservationSkeleton />}
        {activeElectionRound && (
          <IncidentReportingFormList
            ListHeaderComponent={
              <YStack>
                <Typography preset="body1" fontWeight="700" marginTop="$lg" marginBottom="$xxs">
                  {t("forms.heading")}
                </Typography>
              </YStack>
            }
          />
        )}
      </YStack>
    </Screen>
  );
};

export default IncidentReportForms;
