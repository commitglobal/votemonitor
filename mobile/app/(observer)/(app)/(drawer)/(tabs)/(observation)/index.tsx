import { DrawerActions } from "@react-navigation/native";
import * as Clipboard from "expo-clipboard";
import { router, useNavigation } from "expo-router";
import React, { useState } from "react";
import { useTranslation } from "react-i18next";
import Toast from "react-native-toast-message";
import { YStack } from "tamagui";
import Header from "../../../../../../components/Header";
import { Icon } from "../../../../../../components/Icon";
import MultiSubmissionFormList from "../../../../../../components/MultiSubmissionFormList";
import NoElectionRounds from "../../../../../../components/NoElectionRounds";
import NoVisitsExist from "../../../../../../components/NoVisitsExist";
import OptionsSheet from "../../../../../../components/OptionsSheet";
import { PollingStationGeneral } from "../../../../../../components/PollingStationGeneral";
import { Screen } from "../../../../../../components/Screen";
import SelectPollingStation from "../../../../../../components/SelectPollingStation";
import SingleSubmissionFormList from "../../../../../../components/SingleSubmissionFormList";
import ObservationSkeleton from "../../../../../../components/SkeletonLoaders/ObservationSkeleton";
import { Typography } from "../../../../../../components/Typography";
import { useUserData } from "../../../../../../contexts/user/UserContext.provider";
import {
  usePollingStationInformation,
  usePollingStationInformationForm,
} from "../../../../../../services/queries.service";

const Index = () => {
  const { t } = useTranslation("observation");
  const navigation = useNavigation();
  const [openContextualMenu, setOpenContextualMenu] = useState(false);

  const { isLoading, visits, selectedPollingStation, activeElectionRound } = useUserData();

  const { data: psiData, isLoading: isLoadingPsiData } = usePollingStationInformation(
    activeElectionRound?.id,
    selectedPollingStation?.pollingStationId,
  );

  const { data: psiFormQuestions, isLoading: isLoadingPsiFormQuestions } =
    usePollingStationInformationForm(activeElectionRound?.id);

  const handleCopyPollingStationInfo = async () => {
    if (!selectedPollingStation) {
      return;
    }

    // Find the matching visit to get address and level information
    const matchingVisit = visits?.find(
      (visit) => visit.pollingStationId === selectedPollingStation.pollingStationId,
    );

    const infoText = [
      matchingVisit?.level1,
      matchingVisit?.level2,
      matchingVisit?.level3,
      matchingVisit?.level4,
      matchingVisit?.level5,
      matchingVisit?.number,
      matchingVisit?.address,
    ]
      .filter(Boolean)
      .join(" / ");

    try {
      await Clipboard.setStringAsync(infoText);
      Toast.show({
        type: "success",
        text2: t("options_menu.copy_success_toast"),
      });
      setOpenContextualMenu(false);
    } catch (error) {
      console.error("Failed to copy polling station information:", error);
      Toast.show({
        type: "error",
        text2: t("options_menu.copy_error_toast"),
      });
    }
  };

  if (!isLoading && !activeElectionRound) {
    return <NoElectionRounds />;
  }

  if (!isLoading && visits && !visits.length) {
    return <NoVisitsExist activeElectionRound={activeElectionRound} />;
  }

  return (
    <Screen preset="fixed" contentContainerStyle={{ flexGrow: 1 }}>
      <YStack>
        <Header
          title={t("title")}
          titleColor="white"
          barStyle="light-content"
          leftIcon={<Icon icon="menuAlt2" color="white" />}
          onLeftPress={() => navigation.dispatch(DrawerActions.openDrawer)}
          rightIcon={<Icon icon="dotsVertical" color="white" />}
          onRightPress={() => setOpenContextualMenu(true)}
        />
        <SelectPollingStation />
      </YStack>

      {openContextualMenu && (
        <OptionsSheet open setOpen={setOpenContextualMenu}>
          <YStack
            paddingHorizontal="$sm"
            gap="$xxs"
          >
            <Typography
              preset="body1"
              color="$gray7"
              paddingVertical="$xs"
              lineHeight={24}
              onPress={() => {
                setOpenContextualMenu(false);
                router.push("/manage-polling-stations");
              }}>
              {t("options_menu.manage_my_polling_stations")}
            </Typography>
            <Typography
              preset="body1"
              color="$gray7"
              paddingVertical="$xs"
              lineHeight={24}
              onPress={handleCopyPollingStationInfo}>
              {t("options_menu.copy_polling_station_information")}
            </Typography>
          </YStack>
        </OptionsSheet>
      )}

      <YStack paddingHorizontal="$md" flex={1}>
        {(isLoading || isLoadingPsiData || isLoadingPsiFormQuestions) && <ObservationSkeleton />}
        {activeElectionRound &&
          selectedPollingStation?.pollingStationId &&
          psiFormQuestions &&
          !activeElectionRound.allowMultipleFormSubmission && (
            <SingleSubmissionFormList
              ListHeaderComponent={
                <YStack>
                  <PollingStationGeneral psiData={psiData} psiFormQuestions={psiFormQuestions} />
                  <Typography preset="body1" fontWeight="700" marginTop="$lg" marginBottom="$xxs">
                    {t("forms.heading")}
                  </Typography>
                </YStack>
              }
            />
          )}
        {activeElectionRound &&
          selectedPollingStation?.pollingStationId &&
          psiFormQuestions &&
          activeElectionRound.allowMultipleFormSubmission && (
            <MultiSubmissionFormList
              ListHeaderComponent={
                <YStack>
                  <PollingStationGeneral psiData={psiData} psiFormQuestions={psiFormQuestions} />
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

export default Index;
