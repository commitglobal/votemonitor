import Header from "../../../../../components/Header";
import { Screen } from "../../../../../components/Screen";
import { Icon } from "../../../../../components/Icon";
import { DrawerActions, useNavigation } from "@react-navigation/native";
import { Typography } from "../../../../../components/Typography";
import { useTranslation } from "react-i18next";
import { useState } from "react";
import InfoModal from "../../../../../components/InfoModal";

const Updates = () => {
  const { t } = useTranslation("updates");
  const [isOpenInfoModal, setIsOpenInfoModal] = useState<boolean>(false);
  const navigation = useNavigation();

  const handleOpenInfoModal = () => {
    setIsOpenInfoModal(true);
  };

  const handleCloseInfoModal = () => {
    setIsOpenInfoModal(false);
  };

  return (
    <Screen preset="fixed">
      <Header
        title={t("title")}
        titleColor="white"
        barStyle="light-content"
        leftIcon={<Icon icon="menuAlt2" color="white" />}
        onLeftPress={() => navigation.dispatch(DrawerActions.openDrawer)}
        rightIcon={<Icon icon="infoCircle" color="white" width={24} height={24} />}
        onRightPress={handleOpenInfoModal}
      />
      <Typography>Updates</Typography>
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
