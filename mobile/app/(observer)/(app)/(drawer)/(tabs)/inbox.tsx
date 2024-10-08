import { ScrollView, Spinner, XStack, YStack, useWindowDimensions } from "tamagui";
import { Screen } from "../../../../../components/Screen";
import Header from "../../../../../components/Header";
import { Typography } from "../../../../../components/Typography";
import { Icon } from "../../../../../components/Icon";
import { useFocusEffect, useNavigation } from "expo-router";
import { DrawerActions } from "@react-navigation/native";
import NoNotificationsReceived from "../../../../../components/NoNotificationsReceived";
import { ListView } from "../../../../../components/ListView";
import {
  NotificationsKeys,
  useNotifications,
} from "../../../../../services/queries/notifications.query";
import { useUserData } from "../../../../../contexts/user/UserContext.provider";
import { useCallback, useMemo, useState } from "react";
import { useTranslation } from "react-i18next";
import NotificationListItem from "../../../../../components/NotificationListItem";
import { useAppState } from "../../../../../hooks/useAppState";
import { useQueryClient } from "@tanstack/react-query";
import { Notification } from "../../../../../services/api/notifications/notifications-get.api";
import { RefreshControl } from "react-native";
import { Dialog } from "../../../../../components/Dialog";
import Button from "../../../../../components/Button";
import { useReadNotifications } from "../../../../../services/mutations/notifications/read-notifications.mutation";

const ESTIMATED_ITEM_SIZE = 200;

const Inbox = () => {
  const queryClient = useQueryClient();
  const { t, i18n } = useTranslation("inbox");
  const navigation = useNavigation();
  const { width } = useWindowDimensions();

  const { activeElectionRound } = useUserData();
  const { data, isLoading, isRefetching, refetch } = useNotifications(activeElectionRound?.id);

  const notifications = useMemo(() => data?.notifications || [], [data]);
  const unreadNotificationIds = useMemo(
    () =>
      notifications
        .filter((notification) => !notification.isRead)
        .map((notification) => notification.id),
    [notifications],
  );
  const ngoName = useMemo(() => data?.ngoName, [data]);

  const { mutate: readNotifications } = useReadNotifications();

  // read the unread notifications (if any) when the screen is focused
  useFocusEffect(
    useCallback(() => {
      if (unreadNotificationIds?.length) {
        readNotifications(
          {
            electionRoundId: activeElectionRound?.id || "",
            notificationIds: unreadNotificationIds || [],
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

  const [isOpenInfoModal, setIsOpenInfoModal] = useState(false);

  const [sliceNumber, setSliceNumber] = useState(10);
  const loadMore = () => {
    setSliceNumber((sliceNum) => sliceNum + 10);
  };

  const displayedNotifications = useMemo(
    () => notifications?.slice(0, sliceNumber) || [],
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
        {!isLoading && (
          <YStack backgroundColor="$yellow6" paddingVertical="$xxs" paddingHorizontal="$md">
            <Typography textAlign="center" color="$purple5" fontWeight="500">
              {`${t("banner", { ngoName: ngoName || t("your_organization") })}`}
            </Typography>
          </YStack>
        )}
      </YStack>

      {isLoading ? (
        <YStack justifyContent="center" alignItems="center" flex={1}>
          <Spinner size="large" color="$purple5" />
        </YStack>
      ) : (
        <YStack padding="$md" flex={1}>
          <ListView<Notification>
            data={displayedNotifications}
            showsVerticalScrollIndicator={false}
            renderItem={({ item }) => {
              return <NotificationListItem notification={item} />;
            }}
            bounces={true}
            estimatedItemSize={ESTIMATED_ITEM_SIZE}
            estimatedListSize={{ height: ESTIMATED_ITEM_SIZE * 5, width: width - 32 }} // for width we need to take into account the padding also
            onEndReached={loadMore}
            onEndReachedThreshold={0.5}
            refreshControl={<RefreshControl refreshing={isRefetching} onRefresh={refetch} />}
          />
        </YStack>
      )}
      {isOpenInfoModal && (
        <Dialog
          open
          content={
            <YStack maxHeight="85%" gap="$md">
              <ScrollView
                contentContainerStyle={{ gap: 16, flexGrow: 1 }}
                showsVerticalScrollIndicator={false}
                bounces={false}
              >
                <Typography color="$gray6">{t("info_modal.p1")}</Typography>

                <Typography color="$gray6">{t("info_modal.p2")}</Typography>
              </ScrollView>
            </YStack>
          }
          footer={
            <XStack justifyContent="center">
              <Button preset="chromeless" onPress={handleCloseInfoModal}>
                {t("info_modal.ok")}
              </Button>
            </XStack>
          }
        />
      )}
    </Screen>
  );
};

export default Inbox;
