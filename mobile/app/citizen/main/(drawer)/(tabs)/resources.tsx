import React, { useState, useMemo } from "react";
import { Icon } from "../../../../../components/Icon";
import { Screen } from "../../../../../components/Screen";
import Header from "../../../../../components/Header";
import { useNavigation } from "expo-router";
import { DrawerActions } from "@react-navigation/native";
import { useTranslation } from "react-i18next";
// import { useCitizenUserData } from "../../../../../contexts/citizen-user/CitizenUserContext.provider";
import { useCitizenGuides } from "../../../../../services/queries/guides.query";
import ResourcesGuidesList from "../../../../../components/ResourcesList";
import InfoModal from "../../../../../components/InfoModal";
import { YStack } from "tamagui";
import SearchInput from "../../../../../components/SearchInput";
import { guideType } from "../../../../../services/api/get-guides.api";

export default function Resources() {
  const navigation = useNavigation();
  const { t } = useTranslation(["resources", "common"]);
  const [isOpenInfoModal, setIsOpenInfoModal] = useState<boolean>(false);
  const [searchTerm, setSearchTerm] = useState<string>("");

  // const { selectedElectionRound } = useCitizenUserData();

  const {
    data: resources,
    isLoading,
    refetch,
    isRefetching,
    // } = useCitizenGuides(selectedElectionRound);
  } = useCitizenGuides("43b91c74-6d05-4fd1-bd93-dfe203c83c53");

  const filteredResources = useMemo(() => {
    return resources?.filter(
      (resource) =>
        resource.title.toLowerCase().includes(searchTerm.toLowerCase()) ||
        resource.fileName?.toLowerCase().includes(searchTerm.toLowerCase()) ||
        (resource.guideType === guideType.TEXT &&
          resource.text?.toLowerCase().includes(searchTerm.toLowerCase())),
    );
  }, [resources, searchTerm]);

  const handleOpenInfoModal = () => {
    setIsOpenInfoModal(true);
  };

  const handleCloseInfoModal = () => {
    setIsOpenInfoModal(false);
  };

  const handleSearch = (value: string) => {
    setSearchTerm(value);
  };

  return (
    <Screen preset="fixed" contentContainerStyle={{ flexGrow: 1 }}>
      <YStack>
        <Header
          title={t("title")}
          titleColor="white"
          barStyle="light-content"
          leftIcon={<Icon icon="menuAlt2" color="white" />}
          onLeftPress={() => navigation.dispatch(DrawerActions.openDrawer)}
          rightIcon={<Icon icon="infoCircle" color="white" width={24} height={24} />}
          onRightPress={handleOpenInfoModal}
        />
        <YStack backgroundColor="$purple5" padding="$md">
          <SearchInput onSearch={handleSearch} />
        </YStack>
      </YStack>
      <ResourcesGuidesList
        isLoading={isLoading}
        isRefetching={isRefetching}
        resources={filteredResources || []}
        refetch={refetch}
        translationKey="resources"
        emptyContainerMarginTop="30%"
      />
      {isOpenInfoModal && (
        <InfoModal paragraphs={[t("info")]} handleCloseInfoModal={handleCloseInfoModal} />
      )}
    </Screen>
  );
}
