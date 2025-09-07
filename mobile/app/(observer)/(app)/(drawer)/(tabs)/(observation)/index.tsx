import { DrawerActions } from "@react-navigation/native";
import { router, useNavigation } from "expo-router";
import React, { useState } from "react";
import { useTranslation } from "react-i18next";
import { YStack } from "tamagui";
import SingleSubmissionFormList from "../../../../../../components/SingleSubmissionFormList";
import MultiSubmissionFormList from "../../../../../../components/MultiSubmissionFormList";
import Header from "../../../../../../components/Header";
import { Icon } from "../../../../../../components/Icon";
import NoElectionRounds from "../../../../../../components/NoElectionRounds";
import NoVisitsExist from "../../../../../../components/NoVisitsExist";
import OptionsSheet from "../../../../../../components/OptionsSheet";
import { PollingStationGeneral } from "../../../../../../components/PollingStationGeneral";
import { Screen } from "../../../../../../components/Screen";
import SelectPollingStation from "../../../../../../components/SelectPollingStation";
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
            paddingVertical="$xxs"
            paddingHorizontal="$sm"
            onPress={() => {
              setOpenContextualMenu(false);
              router.push("/manage-polling-stations");
            }}
          >
            <Typography preset="body1" color="$gray7" lineHeight={24}>
              {t("options_menu.manage_my_polling_stations")}
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
