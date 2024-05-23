import { Spinner, YStack } from "tamagui";
import { Screen } from "../../../../components/Screen";
import Header from "../../../../components/Header";
import { Typography } from "../../../../components/Typography";
import { Icon } from "../../../../components/Icon";
import { useNavigation } from "expo-router";
import { DrawerActions } from "@react-navigation/native";
import NoNotificationsReceived from "../../../../components/NoNotificationsReceived";
import { ListView } from "../../../../components/ListView";
import { useWindowDimensions } from "react-native";
import {
  NotificationsKeys,
  useNotifications,
} from "../../../../services/queries/notifications.query";
import { useUserData } from "../../../../contexts/user/UserContext.provider";
import { useMemo, useState } from "react";
import { useTranslation } from "react-i18next";
import NotificationListItem from "../../../../components/NotificationListItem";
import OptionsSheet from "../../../../components/OptionsSheet";
import { useAppState } from "../../../../hooks/useAppState";
import { useQueryClient } from "@tanstack/react-query";

const Inbox = () => {
  const queryClient = useQueryClient();
  const { t, i18n } = useTranslation("inbox");
  const navigation = useNavigation();

  const { activeElectionRound } = useUserData();
  const { data, isLoading } = useNotifications(activeElectionRound?.id);
  const notifications = data?.notifications;
  const ngoName = data?.ngoName;

  const [sliceNumber, setSliceNumber] = useState(10);
  const [openContextualMenu, setOpenContextualMenu] = useState(false);
  const loadMore = () => {
    setSliceNumber((sliceNum) => sliceNum + 10);
  };

  const displayedNotifications = useMemo(
    () => notifications?.slice(0, sliceNumber),
    [notifications, sliceNumber, i18n.language],
  );

  useAppState((activating: boolean) => {
    if (activating) {
      queryClient.invalidateQueries({
        queryKey: NotificationsKeys.notifications(activeElectionRound?.id),
      });
    }
  });

  if (!isLoading && (!notifications || !notifications.length)) {
    return <NoNotificationsReceived />;
  }

  return (
    <Screen preset="fixed" contentContainerStyle={{ flexGrow: 1 }} >
      <Header
        title={t("title")}
        titleColor="white"
        barStyle="light-content"
        leftIcon={<Icon icon="menuAlt2" color="white" />}
        onLeftPress={() => navigation.dispatch(DrawerActions.openDrawer)}
        rightIcon={<Icon icon="dotsVertical" color="white" />}
        onRightPress={() => setOpenContextualMenu(true)}
      />

      {isLoading ? (
        <YStack flex={1} justifyContent="center" alignItems="center" >
          <Spinner size="large" color="$purple5" />
        </YStack>
      ) : (
        <>
          <YStack backgroundColor="$yellow6" paddingVertical="$xxs" paddingHorizontal="$md">
            <Typography textAlign="center" color="$purple5" fontWeight="500">
              {`${t("banner", { ngoName: ngoName || t("your_organization") })}`}
            </Typography>
          </YStack>
          <YStack padding="$md" style={{ flex: 1 }}>
            <ListView<any>
              data={displayedNotifications}
              showsVerticalScrollIndicator={false}
              keyExtractor={(item) => item.id}
              bounces={false}
              renderItem={({ item }) => {
                return <NotificationListItem notification={item} />;
              }}
              estimatedItemSize={500}
              onEndReached={loadMore}
              onEndReachedThreshold={0.5}
            />
          </YStack>
        </>
      )}
      <OptionsSheet open={openContextualMenu} setOpen={setOpenContextualMenu}>
        <YStack
          paddingVertical="$xxs"
          paddingHorizontal="$sm"
          onPress={() => {
            setOpenContextualMenu(false);
            // todo: router.push to manage my polling stations
            // return router.push("change-password");
          }}
        >
          <Typography preset="body1" color="$gray7" lineHeight={24}>
            {t("menu.manage_polling_stations")}
          </Typography>
        </YStack>
      </OptionsSheet>
    </Screen>
  );
};

export default Inbox;
