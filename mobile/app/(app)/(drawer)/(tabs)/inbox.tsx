import { Spinner, YStack, useWindowDimensions } from "tamagui";
import { Screen } from "../../../../components/Screen";
import Header from "../../../../components/Header";
import { Typography } from "../../../../components/Typography";
import { Icon } from "../../../../components/Icon";
import { router, useNavigation } from "expo-router";
import { DrawerActions } from "@react-navigation/native";
import NoNotificationsReceived from "../../../../components/NoNotificationsReceived";
import { ListView } from "../../../../components/ListView";
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
import { useSafeAreaInsets } from "react-native-safe-area-context";
import { Notification } from "../../../../services/api/get-notifications.api";

const ESTIMATED_ITEM_SIZE = 200;

const Inbox = () => {
  const queryClient = useQueryClient();
  const { t, i18n } = useTranslation("inbox");
  const navigation = useNavigation();

  const { height, width } = useWindowDimensions();
  const insets = useSafeAreaInsets();
  // height for the scrollview with the notifications received
  // 60 = bottom navigation tabs
  // 100 = header(60) + yellow banner (40)
  const scrollHeight = height - insets.top - insets.bottom - 100 - 60;

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

  return (
    <Screen
      preset="scroll"
      ScrollViewProps={{
        showsVerticalScrollIndicator: false,
        stickyHeaderIndices: [0],
        bounces: false,
      }}
    >
      <YStack>
        <Header
          title={activeElectionRound?.title}
          titleColor="white"
          barStyle="light-content"
          leftIcon={<Icon icon="menuAlt2" color="white" />}
          onLeftPress={() => navigation.dispatch(DrawerActions.openDrawer)}
          rightIcon={<Icon icon="dotsVertical" color="white" />}
          onRightPress={() => setOpenContextualMenu(true)}
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
        <YStack justifyContent="center" alignItems="center" minHeight={scrollHeight}>
          <Spinner size="large" color="$purple5" />
        </YStack>
      ) : (
        <YStack padding="$md" minHeight={scrollHeight}>
          <ListView<Notification>
            data={displayedNotifications}
            showsVerticalScrollIndicator={false}
            bounces={false}
            renderItem={({ item }) => {
              return <NotificationListItem notification={item} />;
            }}
            estimatedItemSize={ESTIMATED_ITEM_SIZE}
            estimatedListSize={{ height: ESTIMATED_ITEM_SIZE * 5, width: width - 32 }} // for width we need to take into account the padding also
            onEndReached={loadMore}
            onEndReachedThreshold={0.5}
          />
        </YStack>
      )}
      {openContextualMenu && (
        <OptionsSheet open={openContextualMenu} setOpen={setOpenContextualMenu} key={"InboxSheet"}>
          <YStack
            paddingVertical="$xxs"
            paddingHorizontal="$sm"
            onPress={() => {
              setOpenContextualMenu(false);
              return router.push("manage-polling-stations");
            }}
          >
            <Typography preset="body1" color="$gray7" lineHeight={24}>
              {t("menu.manage_polling_stations")}
            </Typography>
          </YStack>
        </OptionsSheet>
      )}
    </Screen>
  );
};

export default Inbox;
