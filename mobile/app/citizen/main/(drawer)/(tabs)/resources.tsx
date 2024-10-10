import React, { useState } from "react";
import { Icon } from "../../../../../components/Icon";
import { Screen } from "../../../../../components/Screen";
import Header from "../../../../../components/Header";
import { useNavigation } from "expo-router";
import { DrawerActions } from "@react-navigation/native";
import { useTranslation } from "react-i18next";
import { Dialog } from "../../../../../components/Dialog";
import Button from "../../../../../components/Button";
import { Typography } from "../../../../../components/Typography";
import { XStack, YStack, ScrollView } from "tamagui";
import { useCitizenUserData } from "../../../../../contexts/citizen-user/CitizenUserContext.provider";
import { useCitizenGuides } from "../../../../../services/queries/guides.query";
import ResourcesGuidesList from "../../../../../components/ResourcesList";

const InfoDialog = ({ handleCloseInfoModal }: { handleCloseInfoModal: () => void }) => {
  const { t } = useTranslation(["resources", "common"]);
  return (
    <Dialog
      open
      content={
        <YStack maxHeight="85%" gap="$md">
          <ScrollView
            contentContainerStyle={{ flexGrow: 1 }}
            showsVerticalScrollIndicator={false}
            bounces={false}
          >
            <Typography color="$gray6">{t("info")}</Typography>
          </ScrollView>
        </YStack>
      }
      footer={
        <XStack justifyContent="center">
          <Button preset="chromeless" onPress={handleCloseInfoModal}>
            {t("ok", { ns: "common" })}
          </Button>
        </XStack>
      }
    />
  );
};

const ResourcesList = () => {
  const { selectedElectionRound } = useCitizenUserData();

  const {
    data: resources,
    isLoading,
    refetch,
    isRefetching,
  } = useCitizenGuides(selectedElectionRound);
  // } = useCitizenGuides("43b91c74-6d05-4fd1-bd93-dfe203c83c53");

  return (
    <ResourcesGuidesList
      isLoading={isLoading}
      isRefetching={isRefetching}
      resources={resources || []}
      refetch={refetch}
      translationKey="resources"
    />
  );
};

export default function Resources() {
  const navigation = useNavigation();
  const { t } = useTranslation(["resources", "common"]);
  const [isOpenInfoModal, setIsOpenInfoModal] = useState<boolean>(false);

  const handleOpenInfoModal = () => {
    setIsOpenInfoModal(true);
  };

  const handleCloseInfoModal = () => {
    setIsOpenInfoModal(false);
  };

  return (
    <Screen preset="fixed" contentContainerStyle={{ flexGrow: 1 }}>
      <Header
        title={t("title")}
        titleColor="white"
        barStyle="light-content"
        leftIcon={<Icon icon="menuAlt2" color="white" />}
        onLeftPress={() => navigation.dispatch(DrawerActions.openDrawer)}
        rightIcon={<Icon icon="infoCircle" color="white" width={24} height={24} />}
        onRightPress={handleOpenInfoModal}
      />

      <ResourcesList />
      {isOpenInfoModal && <InfoDialog handleCloseInfoModal={handleCloseInfoModal} />}
    </Screen>
  );
}
