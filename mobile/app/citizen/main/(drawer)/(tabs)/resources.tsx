import React, { useState, useMemo } from "react";
import { Icon } from "../../../../../components/Icon";
import { Screen } from "../../../../../components/Screen";
import Header from "../../../../../components/Header";
import { useNavigation, useRouter } from "expo-router";
import { DrawerActions } from "@react-navigation/native";
import { useTranslation } from "react-i18next";
import { useCitizenUserData } from "../../../../../contexts/citizen-user/CitizenUserContext.provider";
import { useCitizenGuides } from "../../../../../services/queries/guides.query";
import ResourcesGuidesList from "../../../../../components/ResourcesList";
import InfoModal from "../../../../../components/InfoModal";
import { YStack } from "tamagui";
import SearchInput from "../../../../../components/SearchInput";
import { Guide, guideType } from "../../../../../services/api/get-guides.api";
import { filterResources } from "../../../../../helpers/resources";

export default function Resources() {
  const navigation = useNavigation();
  const router = useRouter();
  const { t } = useTranslation(["resources", "common"]);
  const [isOpenInfoModal, setIsOpenInfoModal] = useState<boolean>(false);
  const [searchTerm, setSearchTerm] = useState<string>("");

  const { selectedElectionRound } = useCitizenUserData();

  const { data: resources, isLoading, refetch } = useCitizenGuides(selectedElectionRound);

  const filteredResources = useMemo(() => {
    return filterResources(resources || [], searchTerm);
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

  const handleResourcePress = (resource: Guide) => {
    if (resource.guideType === guideType.TEXT) {
      router.push(`/citizen/main/resource/${resource.id}`);
    }
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
        key={`resources-${selectedElectionRound}`}
        isLoading={isLoading}
        resources={filteredResources || []}
        refetch={refetch}
        translationKey="resources"
        emptyContainerMarginTop="30%"
        onResourcePress={handleResourcePress}
      />
      {isOpenInfoModal && (
        <InfoModal paragraphs={[t("info")]} handleCloseInfoModal={handleCloseInfoModal} />
      )}
    </Screen>
  );
}
