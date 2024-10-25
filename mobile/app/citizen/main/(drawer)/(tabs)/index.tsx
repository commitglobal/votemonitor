import React, { useMemo, useState } from "react";
import { useTranslation } from "react-i18next";
import {
  useGetCitizenLocations,
  useGetCitizenReportingForms,
} from "../../../../../services/queries/citizen.query";
import { Screen } from "../../../../../components/Screen";
import { Typography } from "../../../../../components/Typography";
import { Icon } from "../../../../../components/Icon";
import Header from "../../../../../components/Header";
import { DrawerActions, useNavigation } from "@react-navigation/native";
import { useCitizenUserData } from "../../../../../contexts/citizen-user/CitizenUserContext.provider";

import InfoModal from "../../../../../components/InfoModal";
import { CitizenFormsList } from "../../../../../components/CitizenFormsList";
import { FormAPIModel } from "../../../../../services/definitions.api";
import { router } from "expo-router";

export default function CitizenReportIssue() {
  const { t } = useTranslation("citizen_report_issue");
  const navigation = useNavigation();

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
      <Screen
        preset="fixed"
        contentContainerStyle={{
          flex: 1,
        }}
      >
        <PageHeader />
        <Typography padding="$md">No selected election round!</Typography>
      </Screen>
    );
  }

  const {
    data: citizenReportingForms,
    isLoading: isLoadingCitizenReportingForms,
    isError: isErrorCitizenReportingForms,
    refetch: refetchCitizenReportingForms,
    isRefetching: isRefetchingCitizenReportingForms,
  } = useGetCitizenReportingForms(selectedElectionRound);

  const {
    isLoading: isLoadingCitizenLocations,
    isError: isErrorCitizenLocations,
    refetch: refetchCitizenLocations,
    isRefetching: isRefetchingCitizenLocations,
  } = useGetCitizenLocations(selectedElectionRound);

  const isLoading = useMemo(
    () => isLoadingCitizenReportingForms || isLoadingCitizenLocations,
    [isLoadingCitizenReportingForms, isLoadingCitizenLocations],
  );

  const isError = useMemo(
    () => isErrorCitizenReportingForms || isErrorCitizenLocations,
    [isErrorCitizenReportingForms, isErrorCitizenLocations],
  );

  const isRefetching = useMemo(
    () => isRefetchingCitizenReportingForms || isRefetchingCitizenLocations,
    [isRefetchingCitizenReportingForms, isRefetchingCitizenLocations],
  );

  const handleRefetch = () => {
    refetchCitizenReportingForms();
    refetchCitizenLocations();
  };

  const handleOpenInfoModal = () => {
    setIsOpenInfoModal(true);
  };

  const handleCloseInfoModal = () => {
    setIsOpenInfoModal(false);
  };

  const handleFormPress = (form: FormAPIModel) => {
    router.push(
      `/citizen/main/select-location?formId=${form.id}&questionId=${form.questions[0].id}`,
    );
  };

  return (
    <Screen
      preset="fixed"
      contentContainerStyle={{
        flex: 1,
      }}
    >
      <PageHeader />

      <CitizenFormsList
        forms={citizenReportingForms?.forms || []}
        isLoading={isLoading}
        isError={isError}
        isRefetching={isRefetching}
        refetch={handleRefetch}
        onFormPress={handleFormPress}
      />

      {isOpenInfoModal && (
        <InfoModal
          paragraphs={[
            t("info_modal.p1"),
            t("info_modal.p2"),
            t("info_modal.p3"),
            t("info_modal.p4"),
          ]}
          handleCloseInfoModal={handleCloseInfoModal}
        />
      )}
    </Screen>
  );
}
