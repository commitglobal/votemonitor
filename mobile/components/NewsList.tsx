import { LoadingContent, EmptyContent } from "./ListContent";
import { ListView } from "./ListView";
import { Notification } from "../services/api/notifications/notifications-get.api";
import { useWindowDimensions } from "react-native";
import { YStack } from "tamagui";
import NotificationListItem from "./NotificationListItem";
import { useCallback, useMemo, useState } from "react";
import { useTranslation } from "react-i18next";

const ESTIMATED_ITEM_SIZE = 200;

interface NewsListProps {
  isLoading: boolean;
  news: Notification[];
  translationKey?: string;
  refetch: () => Promise<unknown>;
}

const NewsList = ({ isLoading, news = [], refetch, translationKey = "inbox" }: NewsListProps) => {
  const { i18n } = useTranslation(translationKey);
  const { width } = useWindowDimensions();
  const [sliceNumber, setSliceNumber] = useState<number>(10);
  const [refreshing, setRefreshing] = useState(false);

  const onRefresh = useCallback(() => {
    setRefreshing(true);
    refetch().finally(() => {
      setRefreshing(false);
    });
  }, []);

  const handleLoadMore = () => {
    setSliceNumber((prev) => prev + 10);
  };

  const displayedNews = useMemo(
    () => news?.slice(0, sliceNumber) || [],
    [news, sliceNumber, i18n.language],
  );

  if (isLoading) {
    return <LoadingContent />;
  }

  return (
    <YStack padding="$md" flex={1}>
      <ListView<Notification>
        data={displayedNews}
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
        onEndReached={handleLoadMore}
        refreshing={refreshing}
        onEndReachedThreshold={0.5}
        onRefresh={onRefresh}
      />
    </YStack>
  );
};

export default NewsList;
