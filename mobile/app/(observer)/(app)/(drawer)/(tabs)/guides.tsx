import { Screen } from "../../../../../components/Screen";
import Header from "../../../../../components/Header";
import { Icon } from "../../../../../components/Icon";
import { useNavigation, useRouter } from "expo-router";
import { DrawerActions } from "@react-navigation/native";
import { useTranslation } from "react-i18next";
import { useUserData } from "../../../../../contexts/user/UserContext.provider";
import { useGuides } from "../../../../../services/queries/guides.query";
import ResourcesGuidesList from "../../../../../components/ResourcesList";
import { YStack } from "tamagui";
import SearchInput from "../../../../../components/SearchInput";
import { useState, useMemo } from "react";
import { filterResources } from "../../../../../helpers/resources";
import { Guide } from "../../../../../services/api/get-guides.api";
import InfoModal from "../../../../../components/InfoModal";

const Guides = () => {
  const { t } = useTranslation("guides");
  const router = useRouter();
  const navigation = useNavigation();

  const [search, setSearch] = useState("");
  const [isOpenInfoModal, setIsOpenInfoModal] = useState(false);

  const { activeElectionRound } = useUserData();
  const {
    data: guides,
    isLoading: isLoadingGuides,
    refetch: refetchGuides,
    isRefetching: isRefetchingGuides,
  } = useGuides(activeElectionRound?.id);

  const filteredGuides = useMemo(() => {
    return filterResources(guides || [], search);
  }, [guides, search]);

  const handleSearch = (text: string) => {
    setSearch(text);
  };

  const handleResourcePress = (resource: Guide) => {
    router.push(`/guide/${resource.id}`);
  };

  const handleOpenInfoModal = () => {
    setIsOpenInfoModal(true);
  };

  const handleCloseInfoModal = () => {
    setIsOpenInfoModal(false);
  };

  return (
    <Screen
      preset="fixed"
      contentContainerStyle={{
        flexGrow: 1,
      }}
    >
      <YStack>
        <Header
          title={t("title")}
          titleColor="white"
          barStyle="light-content"
          leftIcon={<Icon icon="menuAlt2" color="white" />}
          onLeftPress={() => navigation.dispatch(DrawerActions.openDrawer)}
          rightIcon={<Icon icon="infoCircle" color="white" />}
          onRightPress={handleOpenInfoModal}
        />
        <YStack backgroundColor="$purple5" padding="$md">
          <SearchInput onSearch={handleSearch} />
        </YStack>
      </YStack>
      <ResourcesGuidesList
        isLoading={isLoadingGuides}
        isRefetching={isRefetchingGuides}
        resources={filteredGuides || []}
        refetch={refetchGuides}
        emptyContainerMarginTop="30%"
        onResourcePress={handleResourcePress}
      />

      {isOpenInfoModal && (
        <InfoModal paragraphs={[t("info_modal")]} handleCloseInfoModal={handleCloseInfoModal} />
      )}
    </Screen>
  );
};

export default Guides;
