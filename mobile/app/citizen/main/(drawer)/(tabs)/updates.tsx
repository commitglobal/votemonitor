import Header from "../../../../../components/Header";
import { Screen } from "../../../../../components/Screen";
import { Icon } from "../../../../../components/Icon";
import { DrawerActions, useNavigation } from "@react-navigation/native";
import { useTranslation } from "react-i18next";
import { useState } from "react";
import InfoModal from "../../../../../components/InfoModal";
import { useCitizenUpdates } from "../../../../../services/queries/notifications.query";
import NewsList from "../../../../../components/NewsList";
import { useCitizenUserData } from "../../../../../contexts/citizen-user/CitizenUserContext.provider";

const Updates = () => {
  const { t } = useTranslation("updates");
  const [isOpenInfoModal, setIsOpenInfoModal] = useState<boolean>(false);
  const navigation = useNavigation();
  const { selectedElectionRound } = useCitizenUserData();
  const { data, isLoading, refetch } = useCitizenUpdates(selectedElectionRound || undefined);

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
      <NewsList
        isLoading={isLoading}
        news={data?.notifications || []}
        refetch={refetch}
        translationKey="updates"
      />
      {isOpenInfoModal && (
        <InfoModal
          paragraphs={[t("info_modal.p1"), t("info_modal.p2")]}
          handleCloseInfoModal={handleCloseInfoModal}
        />
      )}
    </Screen>
  );
};

export default Updates;