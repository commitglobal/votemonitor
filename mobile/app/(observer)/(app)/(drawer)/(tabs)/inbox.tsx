import { Screen } from "../../../../../components/Screen";
import Header from "../../../../../components/Header";
import { Typography } from "../../../../../components/Typography";
import { Icon } from "../../../../../components/Icon";
import { useFocusEffect, useNavigation } from "expo-router";
import { DrawerActions } from "@react-navigation/native";
import {
  NotificationsKeys,
  useNotifications,
} from "../../../../../services/queries/notifications.query";
import { useUserData } from "../../../../../contexts/user/UserContext.provider";
import { useCallback, useMemo, useState } from "react";
import { useTranslation } from "react-i18next";
import { useAppState } from "../../../../../hooks/useAppState";
import { useQueryClient } from "@tanstack/react-query";
import { useReadNotifications } from "../../../../../services/mutations/notifications/read-notifications.mutation";
import NewsList from "../../../../../components/NewsList";
import { YStack } from "tamagui";
import InfoModal from "../../../../../components/InfoModal";

const InboxList = () => {
  const queryClient = useQueryClient();
  const { activeElectionRound } = useUserData();
  const { data, isLoading, refetch } = useNotifications(activeElectionRound?.id);

  const notifications = useMemo(() => data?.notifications || [], [data]);

  const unreadNotificationIds = useMemo(
    () =>
      notifications
        .filter((notification) => !notification.isRead)
        .map((notification) => notification.id),
    [notifications],
  );

  const { mutate: readNotifications } = useReadNotifications();

  // read the unread notifications (if any) when the screen is focused
  useFocusEffect(
    useCallback(() => {
      if (
        unreadNotificationIds?.length &&
        activeElectionRound?.id &&
        unreadNotificationIds.length
      ) {
        readNotifications(
          {
            electionRoundId: activeElectionRound.id,
            notificationIds: unreadNotificationIds,
          },
          {
            onSuccess: () => {
              queryClient.invalidateQueries({
                queryKey: NotificationsKeys.notifications(activeElectionRound?.id),
              });
            },
          },
        );
      }
    }, [unreadNotificationIds, activeElectionRound, readNotifications, queryClient]),
  );

  useAppState((activating: boolean) => {
    if (activating) {
      queryClient.invalidateQueries({
        queryKey: NotificationsKeys.notifications(activeElectionRound?.id),
      });
    }
  });

  return (
    <NewsList isLoading={isLoading} news={notifications} refetch={refetch} translationKey="inbox" />
  );
};

const Inbox = () => {
  // translations
  const { t } = useTranslation("inbox");
  // navigation
  const navigation = useNavigation();
  // component state
  const [isOpenInfoModal, setIsOpenInfoModal] = useState(false);
  // server state
  const { activeElectionRound } = useUserData();
  const { data } = useNotifications(activeElectionRound?.id);

  const ngoName = useMemo(() => data?.ngoName, [data]);

  const handleOpenInfoModal = () => {
    setIsOpenInfoModal(true);
  };

  const handleCloseInfoModal = () => {
    setIsOpenInfoModal(false);
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
        {ngoName && (
          <YStack backgroundColor="$yellow6" paddingVertical="$xxs" paddingHorizontal="$md">
            <Typography textAlign="center" color="$purple5" fontWeight="500">
              {`${t("banner", { ngoName: ngoName || t("your_organization") })}`}
            </Typography>
          </YStack>
        )}
      </YStack>
      <InboxList />
      {isOpenInfoModal && (
        <InfoModal
          paragraphs={[t("info_modal.p1"), t("info_modal.p2")]}
          handleCloseInfoModal={handleCloseInfoModal}
        />
      )}
    </Screen>
  );
};

export default Inbox;