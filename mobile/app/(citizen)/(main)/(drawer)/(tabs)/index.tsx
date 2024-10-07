import React, { useState } from "react";
import { ICitizenReportingForm } from "../../../../../services/api/citizen/get-citizen-reporting-forms";
import { useTranslation } from "react-i18next";
import { useGetCitizenReportingForms } from "../../../../../services/queries/citizen.query";
import { Screen } from "../../../../../components/Screen";
import { ScrollView, Spinner, XStack, YStack } from "tamagui";
import { IssueCard } from "../../../../../components/IssueCard";
import { Dialog } from "../../../../../components/Dialog";
import Button from "../../../../../components/Button";
import { Typography } from "../../../../../components/Typography";
import { Icon } from "../../../../../components/Icon";
import Header from "../../../../../components/Header";
import { DrawerActions, useNavigation } from "@react-navigation/native";

// todo: remove mocks
const mockCitizenReportingForms: ICitizenReportingForm[] = [
  {
    id: "1",
    name: "Problems Regarding Public Resources",
    description: "Report issues related to misuse or mismanagement of public resources.",
  },
  {
    id: "2",
    name: "Problems Regarding Voting",
    description: "Report any irregularities or issues encountered during the voting process.",
  },
  {
    id: "3",
    name: "Problems with Personal Data Protection",
    description: "Report concerns about the handling or protection of personal data.",
  },
  {
    id: "4",
    name: "Other Issues",
    description: "Report any other election-related issues not covered by the above categories.",
  },
  {
    id: "5",
    name: "Problems with Personal Data Protection",
  },
];

export default function CitizenReportIssue() {
  const { t } = useTranslation("citizen_report_issue");
  const navigation = useNavigation();
  const [isOpenInfoModal, setIsOpenInfoModal] = useState(false);
  //   todo: remove this id
  const {
    data: citizenReportingForms,
    isLoading: isLoadingCitizenReportingForms,
    isError: isErrorCitizenReportingForms,
    refetch: refetchCitizenReportingForms,
    isRefetching: isRefetchingCitizenReportingForms,
  } = useGetCitizenReportingForms("5bf2d5aa-5457-4f36-82f8-1a65962cece7");

  const handleOpenInfoModal = () => {
    setIsOpenInfoModal(true);
  };

  const handleCloseInfoModal = () => {
    setIsOpenInfoModal(false);
  };

  const renderHeader = () => {
    return (
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
  };

  // loading state
  if (isLoadingCitizenReportingForms) {
    return (
      <Screen
        preset="fixed"
        contentContainerStyle={{
          flex: 1,
        }}
      >
        {renderHeader()}
        <YStack flex={1} paddingVertical="$lg" justifyContent="center" alignItems="center">
          <Spinner color="$purple5" />
        </YStack>
      </Screen>
    );
  }

  // error state
  if (isErrorCitizenReportingForms) {
    return (
      <Screen
        preset="fixed"
        contentContainerStyle={{
          flex: 1,
        }}
      >
        {renderHeader()}
        <YStack flex={1} paddingVertical="$lg" paddingHorizontal="$lg" gap="$lg">
          <XStack gap="$md">
            {isRefetchingCitizenReportingForms ? (
              <Spinner color="$purple5" size="large" />
            ) : (
              <Icon icon="warning" color="$purple5" size={36} />
            )}
            <Typography flex={1}>{t("error_message")}</Typography>
          </XStack>

          <Button
            onPress={() => refetchCitizenReportingForms()}
            disabled={isRefetchingCitizenReportingForms}
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
      {renderHeader()}

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
              {/* //todo: remove mocks */}
              {mockCitizenReportingForms.map((form) => (
                <IssueCard
                  key={form.id}
                  form={form}
                  onClick={() => {
                    console.log("todo");
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
