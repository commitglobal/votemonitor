import React, { useState } from "react";
import { useNavigation, router } from "expo-router";
import { Screen } from "../../../../../../components/Screen";
import { useUserData } from "../../../../../../contexts/user/UserContext.provider";
import { Typography } from "../../../../../../components/Typography";
import { YStack } from "tamagui";
import SelectPollingStation from "../../../../../../components/SelectPollingStation";
import Header from "../../../../../../components/Header";
import { Icon } from "../../../../../../components/Icon";
import { DrawerActions } from "@react-navigation/native";
import NoVisitsExist from "../../../../../../components/NoVisitsExist";
import FormList from "../../../../../../components/FormList";
import OptionsSheet from "../../../../../../components/OptionsSheet";
import { useTranslation } from "react-i18next";
import NoElectionRounds from "../../../../../../components/NoElectionRounds";

const Index = () => {
  const { t } = useTranslation("observation");
  const navigation = useNavigation();
  const [openContextualMenu, setOpenContextualMenu] = useState(false);

  const {
    isLoading: isLoadingUserData,
    visits,
    selectedPollingStation,
    activeElectionRound,
  } = useUserData();

  if (!isLoadingUserData && !activeElectionRound) {
    return <NoElectionRounds />;
  }

  if (!isLoadingUserData && visits && !visits.length) {
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

      <YStack paddingHorizontal="$md" flex={1}>
        {activeElectionRound && selectedPollingStation?.pollingStationId && (
          <FormList isLoading={isLoadingUserData} />
        )}
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
    </Screen>
  );
};

export default Index;
