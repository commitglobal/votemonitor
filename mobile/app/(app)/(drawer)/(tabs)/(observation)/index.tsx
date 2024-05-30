import React, { useState } from "react";
import { useNavigation, router } from "expo-router";
import { Screen } from "../../../../../components/Screen";
import { useUserData } from "../../../../../contexts/user/UserContext.provider";
import { Typography } from "../../../../../components/Typography";
import { YStack } from "tamagui";
import {
  usePollingStationInformation,
  usePollingStationInformationForm,
} from "../../../../../services/queries.service";
import SelectPollingStation from "../../../../../components/SelectPollingStation";
import Header from "../../../../../components/Header";
import { Icon } from "../../../../../components/Icon";
import { DrawerActions } from "@react-navigation/native";
import NoVisitsExist from "../../../../../components/NoVisitsExist";
import { PollingStationGeneral } from "../../../../../components/PollingStationGeneral";
import FormList from "../../../../../components/FormList";
import OptionsSheet from "../../../../../components/OptionsSheet";
import { useTranslation } from "react-i18next";
import NoElectionRounds from "../../../../../components/NoElectionRounds";

const Index = () => {
  const { t } = useTranslation("observation");
  const navigation = useNavigation();
  const [openContextualMenu, setOpenContextualMenu] = useState(false);

  const { isLoading, visits, selectedPollingStation, activeElectionRound, electionRounds } =
    useUserData();

  const { data: psiData } = usePollingStationInformation(
    activeElectionRound?.id,
    selectedPollingStation?.pollingStationId,
  );

  const { data: psiFormQuestions } = usePollingStationInformationForm(activeElectionRound?.id);

  if (!isLoading && electionRounds && !electionRounds.length) {
    return <NoElectionRounds />;
  }

  if (!isLoading && visits && !visits.length) {
    return <NoVisitsExist />;
  }

  return (
    <Screen
      preset="scroll"
      ScrollViewProps={{
        showsVerticalScrollIndicator: false,
        stickyHeaderIndices: [0],
        bounces: false,
      }}
    >
      <YStack marginBottom={20}>
        <Header
          title={activeElectionRound?.title}
          titleColor="white"
          barStyle="light-content"
          leftIcon={<Icon icon="menuAlt2" color="white" />}
          onLeftPress={() => navigation.dispatch(DrawerActions.openDrawer)}
          rightIcon={<Icon icon="dotsVertical" color="white" />}
          onRightPress={() => setOpenContextualMenu(true)}
        />
        <SelectPollingStation />
      </YStack>

      <YStack paddingHorizontal="$md">
        <FormList
          ListHeaderComponent={
            <YStack>
              {activeElectionRound &&
                selectedPollingStation?.pollingStationId &&
                psiFormQuestions && (
                  <PollingStationGeneral
                    electionRoundId={activeElectionRound.id}
                    pollingStationId={selectedPollingStation.pollingStationId}
                    psiData={psiData}
                    psiFormQuestions={psiFormQuestions}
                  />
                )}
              <Typography preset="body1" fontWeight="700" marginTop="$lg" marginBottom="$xxs">
                {t("forms.heading")}
              </Typography>
            </YStack>
          }
        />
      </YStack>
      <OptionsSheet open={openContextualMenu} setOpen={setOpenContextualMenu}>
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
    </Screen>
  );
};

export default Index;
