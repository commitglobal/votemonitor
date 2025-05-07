import { useCallback, useState } from "react";
import { useTranslation } from "react-i18next";
import { useWindowDimensions } from "react-native";
import { YStack } from "tamagui";
import { ElectionRoundVM } from "../common/models/election-round.model";
import { useNetInfoContext } from "../contexts/net-info-banner/NetInfoContext";
import Card, { CardProps } from "./Card";
import { EmptyContent, LoadingContent } from "./ListContent";
import { ListView } from "./ListView";
import { Typography } from "./Typography";
import { useRouter } from "expo-router";

interface PastElectionCardProps extends CardProps {
  electionRound: ElectionRoundVM;
}

const PastElectionCard = ({ electionRound, ...rest }: PastElectionCardProps) => {
  const { t } = useTranslation(["er-statistics"]);
  const router = useRouter();

  return (
    <Card gap="$xxs" onPress={() => router.push(`/er-statistics/${electionRound.id}`)} {...rest}>
      <Typography preset="body1" fontWeight="700">
        {electionRound.title}
      </Typography>

      <Typography preset="body1" color="$gray6">
        {electionRound.startDate}
      </Typography>
    </Card>
  );
};

const ESTIMATED_ITEM_SIZE = 115;

interface PastElectionsListProps {
  isLoading: boolean;
  elections: ElectionRoundVM[];
  refetch: () => Promise<unknown>;
  header?: JSX.Element;
  emptyContainerMarginTop?: string;
}

const PastElectionsList = ({
  isLoading,
  elections = [],
  refetch,
  header,
  emptyContainerMarginTop,
}: PastElectionsListProps) => {
  const { width } = useWindowDimensions();
  const { isOnline } = useNetInfoContext();
  const [refreshing, setRefreshing] = useState(false);

  const onRefresh = useCallback(() => {
    setRefreshing(true);
    refetch().finally(() => {
      setRefreshing(false);
    });
  }, [refetch]);

  if (isLoading) {
    return <LoadingContent />;
  }

  return (
    <YStack flex={1}>
      <ListView<ElectionRoundVM>
        data={elections}
        showsVerticalScrollIndicator={false}
        contentContainerStyle={{ padding: 16 }}
        bounces={isOnline}
        ListHeaderComponent={header}
        ListEmptyComponent={
          <EmptyContent
            translationKey={"er-statistics"}
            illustrationIconKey="undrawReading"
            emptyContainerMarginTop={emptyContainerMarginTop}
          />
        }
        estimatedItemSize={ESTIMATED_ITEM_SIZE}
        estimatedListSize={{ height: ESTIMATED_ITEM_SIZE * 5, width: width - 32 }}
        renderItem={({ item, index }) => (
          <PastElectionCard key={index} electionRound={item} marginBottom="$md" />
        )}
        onRefresh={onRefresh}
        refreshing={refreshing}
      />
    </YStack>
  );
};

export default PastElectionsList;
