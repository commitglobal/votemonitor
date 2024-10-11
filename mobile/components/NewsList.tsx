import { LoadingContent, EmptyContent } from "./ListContent";
import { ListView } from "./ListView";
import { Notification } from "../services/api/notifications/notifications-get.api";
import { RefreshControl, useWindowDimensions } from "react-native";
import { YStack } from "tamagui";
import NotificationListItem from "./NotificationListItem";

const ESTIMATED_ITEM_SIZE = 200;

interface NewsListProps {
  isLoading: boolean;
  news: Notification[];
  isRefetching: boolean;
  translationKey?: string;
  loadMore: () => void;
  refetch: () => void;
}

const NewsList = ({
  isLoading,
  news,
  loadMore,
  isRefetching,
  refetch,
  translationKey = "inbox",
}: NewsListProps) => {
  const { width } = useWindowDimensions();

  if (isLoading) {
    return <LoadingContent />;
  }

  return (
    <YStack padding="$md" flex={1}>
      <ListView<Notification>
        data={news}
        showsVerticalScrollIndicator={false}
        renderItem={({ item }) => {
          return <NotificationListItem notification={item} />;
        }}
        ListEmptyComponent={
          <EmptyContent translationKey={translationKey} illustrationIconKey="undrawInbox" />
        }
        bounces={true}
        estimatedItemSize={ESTIMATED_ITEM_SIZE}
        estimatedListSize={{ height: ESTIMATED_ITEM_SIZE * 5, width: width - 32 }} // for width we need to take into account the padding also
        onEndReached={loadMore}
        onEndReachedThreshold={0.5}
        refreshControl={<RefreshControl refreshing={isRefetching} onRefresh={refetch} />}
      />
    </YStack>
  );
};

export default NewsList;
