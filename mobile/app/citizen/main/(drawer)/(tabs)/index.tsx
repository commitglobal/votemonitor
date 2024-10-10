import React, { useState } from "react";
import { useTranslation } from "react-i18next";
import {
  useGetCitizenLocations,
  useGetCitizenReportingForms,
} from "../../../../../services/queries/citizen.query";
import { Screen } from "../../../../../components/Screen";
import { ScrollView, Spinner, XStack, YStack } from "tamagui";
import { IssueCard } from "../../../../../components/IssueCard";
import { Dialog } from "../../../../../components/Dialog";
import Button from "../../../../../components/Button";
import { Typography } from "../../../../../components/Typography";
import { Icon } from "../../../../../components/Icon";
import Header from "../../../../../components/Header";
import { DrawerActions, useNavigation } from "@react-navigation/native";
import { useCitizenUserData } from "../../../../../contexts/citizen-user/CitizenUserContext.provider";
import { useRouter } from "expo-router";

const LoadingScreen = ({ children }: { children: React.ReactNode }) => (
  <Screen
    preset="fixed"
    contentContainerStyle={{
      flex: 1,
    }}
  >
    {children}
    <YStack flex={1} paddingVertical="$lg" justifyContent="center" alignItems="center">
      <Spinner color="$purple5" />
    </YStack>
  </Screen>
);

export default function CitizenReportIssue() {
  const { t } = useTranslation("citizen_report_issue");
  const navigation = useNavigation();
  const router = useRouter();

  const [isOpenInfoModal, setIsOpenInfoModal] = useState(false);
  const { selectedElectionRound } = useCitizenUserData();

  const PageHeader = () => (
    <Header
      title={t("header_title")}
      leftIcon={<Icon icon="menuAlt2" color="white" />}
      onLeftPress={() => navigation.dispatch(DrawerActions.openDrawer)}
      rightIcon={<Icon icon="infoCircle" color="white" />}
      onRightPress={handleOpenInfoModal}
      barStyle="light-content"
      backgroundColor="$purple5"
    ></Header>
  );

  if (!selectedElectionRound) {
    return (
      <LoadingScreen>
        <PageHeader />
      </LoadingScreen>
    );
  }

  const {
    data: citizenReportingForms,
    isLoading: isLoadingCitizenReportingForms,
    isError: isErrorCitizenReportingForms,
    refetch: refetchCitizenReportingForms,
    isRefetching: isRefetchingCitizenReportingForms,
  } = useGetCitizenReportingForms(selectedElectionRound);

  // console.log("👀 citizenReportingForms", JSON.stringify(citizenReportingForms, null, 2));

  const {
    isLoading: isLoadingCitizenLocations,
    isError: isErrorCitizenLocations,
    refetch: refetchCitizenLocations,
    isRefetching: isRefetchingCitizenLocations,
  } = useGetCitizenLocations(selectedElectionRound);

  const handleOpenInfoModal = () => {
    setIsOpenInfoModal(true);
  };

  const handleCloseInfoModal = () => {
    setIsOpenInfoModal(false);
  };

  // loading state
  if (isLoadingCitizenReportingForms || isLoadingCitizenLocations || !selectedElectionRound) {
    return (
      <LoadingScreen>
        <PageHeader />
      </LoadingScreen>
    );
  }

  // error state
  if (isErrorCitizenReportingForms || isErrorCitizenLocations) {
    return (
      <Screen
        preset="fixed"
        contentContainerStyle={{
          flex: 1,
        }}
      >
        <PageHeader />
        <YStack flex={1} paddingVertical="$lg" paddingHorizontal="$lg" gap="$lg">
          <XStack gap="$md">
            {isRefetchingCitizenReportingForms || isRefetchingCitizenLocations ? (
              <Spinner color="$purple5" size="large" />
            ) : (
              <Icon icon="warning" color="$purple5" size={36} />
            )}
            <Typography flex={1}>{t("error_message")}</Typography>
          </XStack>

          <Button
            onPress={() => {
              refetchCitizenReportingForms();
              refetchCitizenLocations();
            }}
            disabled={isRefetchingCitizenReportingForms || isRefetchingCitizenLocations}
            marginTop="$lg"
          >
            {t("retry")}
          </Button>
        </YStack>
      </Screen>
    );
  }

  return (
    <Screen
      preset="fixed"
      contentContainerStyle={{
        flex: 1,
      }}
    >
      <PageHeader />

      <YStack flex={1} paddingVertical="$lg">
        <ScrollView
          showsVerticalScrollIndicator={false}
          contentContainerStyle={{
            flexGrow: 1,
            flexDirection: "column",
            gap: 16,
            paddingHorizontal: 24,
          }}
        >
          <Typography>{t("description")}</Typography>

          {/* empty state */}
          {!citizenReportingForms || citizenReportingForms.forms.length === 0 ? (
            <XStack gap="$md" alignItems="center">
              <Icon icon="form" />
              <Typography color="$gray5" flex={1}>
                {t("empty_forms")}
              </Typography>
            </XStack>
          ) : (
            <>
              {citizenReportingForms.forms.map((form) => (
                <IssueCard
                  key={form.id}
                  form={form}
                  onClick={() => {
                    router.push(
                      `/citizen/main/select-location?formId=${form.id}&questionId=${form.questions[0].id}`,
                    );
                  }}
                />
              ))}
            </>
          )}
        </ScrollView>
      </YStack>

      {isOpenInfoModal && (
        <Dialog
          open
          content={
            <YStack maxHeight="85%" gap="$md">
              <ScrollView
                contentContainerStyle={{ gap: 16, flexGrow: 1 }}
                showsVerticalScrollIndicator={false}
                bounces={false}
              >
                <Typography color="$gray6">{t("info_modal.p1")}</Typography>

                <Typography color="$gray6">{t("info_modal.p2")}</Typography>
              </ScrollView>
            </YStack>
          }
          footer={
            <XStack justifyContent="center">
              <Button preset="chromeless" onPress={handleCloseInfoModal}>
                {t("info_modal.ok")}
              </Button>
            </XStack>
          }
        />
      )}
    </Screen>
  );
}
