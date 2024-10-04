import { router } from "expo-router";
import React from "react";
import { useTranslation } from "react-i18next";
import { XStack, YStack } from "tamagui";
import Card from "../../components/Card";
import Header from "../../components/Header";
import { Icon } from "../../components/Icon";
import { Screen } from "../../components/Screen";
import { Typography } from "../../components/Typography";

interface IssueReportTypeCardProps {
  label: string;
  helper?: string;
  onClick?: () => void;
}

const IssueReportTypeCard = ({ label, helper, onClick }: IssueReportTypeCardProps) => (
  <Card onPress={onClick} style={{ minHeight: 64, justifyContent: "center" }}>
    <XStack alignItems="center" justifyContent="space-between" gap="$xxxs">
      <XStack alignItems="center" gap="$xxs" maxWidth="80%">
        <YStack alignContent="center" gap="$xxxs">
          <Typography preset="body2">{label} </Typography>
          {helper && <Typography color="$gray8">{helper}</Typography>}
        </YStack>
      </XStack>

      <Icon size={32} icon="chevronRight" color="$purple7" />
    </XStack>
  </Card>
);

const NewReportSelector = () => {
  const { t } = useTranslation("reports_type_selector");

  return (
    <Screen
      preset="auto"
      backgroundColor="white"
      ScrollViewProps={{
        stickyHeaderIndices: [0],
        bounces: false,
        showsVerticalScrollIndicator: false,
      }}
    >
      <Header
        title={t("title")}
        titleColor="white"
        barStyle="light-content"
        leftIcon={<Icon icon="chevronLeft" color="white" />}
        onLeftPress={() => router.back()}
      />
      <YStack paddingHorizontal="$md" paddingVertical="$xl" gap="$md">
        <IssueReportTypeCard
          label={t("quick-report")}
          helper={t("quick-report-helper")}
          onClick={() => router.push("/add-quick-report")}
        ></IssueReportTypeCard>

        <IssueReportTypeCard
          label={t("incident-report")}
          helper={t("incident-report-helper")}
          onClick={() => router.push("/incident-report-forms")}
        ></IssueReportTypeCard>
      </YStack>
    </Screen>
  );
};

export default NewReportSelector;
