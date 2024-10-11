import { Screen } from "../../../../../components/Screen";
import Header from "../../../../../components/Header";
import { Icon } from "../../../../../components/Icon";
import { useNavigation } from "expo-router";
import { DrawerActions } from "@react-navigation/native";
import { useTranslation } from "react-i18next";
import { useUserData } from "../../../../../contexts/user/UserContext.provider";
import { useGuides } from "../../../../../services/queries/guides.query";
import ResourcesGuidesList from "../../../../../components/ResourcesList";
import { Typography } from "../../../../../components/Typography";

const Guides = () => {
  const { t } = useTranslation("guides");
  const navigation = useNavigation();
  const { activeElectionRound } = useUserData();

  const {
    data: guides,
    isLoading: isLoadingGuides,
    refetch: refetchGuides,
    isRefetching: isRefetchingGuides,
  } = useGuides(activeElectionRound?.id);

  return (
    <Screen
      preset="fixed"
      contentContainerStyle={{
        flexGrow: 1,
      }}
    >
      <Header
        title={t("title")}
        titleColor="white"
        barStyle="light-content"
        leftIcon={<Icon icon="menuAlt2" color="white" />}
        onLeftPress={() => navigation.dispatch(DrawerActions.openDrawer)}
      />
      <ResourcesGuidesList
        isLoading={isLoadingGuides}
        isRefetching={isRefetchingGuides}
        resources={guides || []}
        refetch={refetchGuides}
        header={
          guides && guides?.length > 0 ? (
            <Typography preset="body1" fontWeight="700" marginBottom="$xs">
              {t("list.heading")}
            </Typography>
          ) : undefined
        }
      />
    </Screen>
  );
};

export default Guides;
