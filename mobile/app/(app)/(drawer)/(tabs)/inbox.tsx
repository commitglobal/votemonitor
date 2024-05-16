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
import { useSafeAreaInsets } from "react-native-safe-area-context";
import { useNotifications } from "../../../../services/queries/notifications.query";
import { useUserData } from "../../../../contexts/user/UserContext.provider";
import { useMemo, useState } from "react";
import { useTranslation } from "react-i18next";
import NotificationListItem from "../../../../components/NotificationListItem";

const Inbox = () => {
  const { i18n } = useTranslation("inbox");
  const navigation = useNavigation();
  const { height } = useWindowDimensions();
  const insets = useSafeAreaInsets();
  // height for the scrollview with the notifications received
  const scrollHeight = height - 100 - 60 - insets.top - insets.bottom;

  const { activeElectionRound } = useUserData();
  const { data, isLoading } = useNotifications(activeElectionRound?.id);
  const notifications = data?.notifications;

  const [sliceNumber, setSliceNumber] = useState(10);
  const loadMore = () => {
    setSliceNumber((sliceNum) => sliceNum + 10);
  };

  const displayedNotifications = useMemo(
    () => notifications?.slice(0, sliceNumber),
    // todo: is this ok, to add the language for displayed notifications?
    [notifications, sliceNumber, i18n.language],
  );

  if (!isLoading && (!notifications || !notifications.length)) {
    return <NoNotificationsReceived />;
  }

  return (
    <Screen preset="fixed" contentContainerStyle={{ flexGrow: 1 }}>
      <Header
        title={"Inbox"}
        titleColor="white"
        barStyle="light-content"
        leftIcon={<Icon icon="menuAlt2" color="white" />}
        onLeftPress={() => navigation.dispatch(DrawerActions.openDrawer)}
        rightIcon={<Icon icon="dotsVertical" color="white" />}
        onRightPress={() => {
          console.log("Right icon pressed");
        }}
      />

      {isLoading ? (
        <YStack flex={1} justifyContent="center" alignItems="center">
          <Spinner size="large" color="$purple5" />
        </YStack>
      ) : (
        <>
          <YStack backgroundColor="$yellow6" paddingVertical="$xxs">
            {/* //todo: ngo name */}
            <Typography textAlign="center" color="$purple5" fontWeight="500">
              Messages from [NGO Name]
            </Typography>
          </YStack>
          <YStack paddingHorizontal="$md" height={scrollHeight}>
            <ListView<any>
              data={displayedNotifications}
              showsVerticalScrollIndicator={false}
              // todo: keyextractor with the right key
              // keyExtractor={(item) => item.id}
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
    </Screen>
  );
};

export default Inbox;
