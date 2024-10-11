import { YStack } from "tamagui";
import { Typography } from "./Typography";
import { ListView } from "./ListView";
import { Guide, guideType } from "../services/api/get-guides.api";
import * as Linking from "expo-linking";
import { RefreshControl } from "react-native-gesture-handler";
import { EmptyContent, LoadingContent } from "./ListContent";
import { useTranslation } from "react-i18next";
import { useWindowDimensions } from "react-native";
import Card, { CardProps } from "./Card";
import CardFooter from "./CardFooter";

interface GuideCardProps extends CardProps {
  guide: Guide;
  onResourcePress?: (guide: Guide) => void;
}

const GuideCard = ({ guide, onResourcePress, ...rest }: GuideCardProps) => {
  const { t } = useTranslation(["guides", "resources"]);
  const createdOn = new Date(guide.createdOn);

  const handleGuidePress = () => {
    if (guide.guideType === guideType.WEBSITE) {
      Linking.openURL(guide.websiteUrl);
    }

    if (guide.guideType === guideType.DOCUMENT) {
      Linking.openURL(guide.presignedUrl);
    }

    if (guide.guideType === guideType.TEXT) {
      onResourcePress && onResourcePress(guide);
    }
  };

  return (
    <Card gap="$xxs" onPress={handleGuidePress} {...rest}>
      <Typography preset="body1" fontWeight="700">
        {guide.title || t("list.guide.backup_title")}
      </Typography>
      {guide.guideType && (
        <Typography preset="body1" color="$gray6">
          {t(`types.${guide.guideType}`, { ns: "resources" })}
        </Typography>
      )}
      {createdOn && (
        <CardFooter
          text={`${t("list.guide.created_on")} ${createdOn.toLocaleDateString()}, ${createdOn.toLocaleTimeString([], { hour: "2-digit", minute: "2-digit" })}`}
        />
      )}
    </Card>
  );
};

const ESTIMATED_ITEM_SIZE = 115;

interface ResourcesListProps {
  isLoading: boolean;
  isRefetching: boolean;
  resources: Guide[];
  refetch: () => void;
  header?: JSX.Element;
  translationKey?: string;
  emptyContainerMarginTop?: string;
  onResourcePress?: (resource: Guide) => void;
}

const ResourcesGuidesList = ({
  isLoading,
  isRefetching,
  resources,
  refetch,
  header,
  translationKey = "guides",
  emptyContainerMarginTop,
  onResourcePress,
}: ResourcesListProps) => {
  const { width } = useWindowDimensions();

  if (isLoading) {
    return <LoadingContent />;
  }

  return (
    <YStack padding="$md" flex={1}>
      <ListView<Guide>
        data={resources}
        showsVerticalScrollIndicator={false}
        bounces={true}
        ListHeaderComponent={header}
        ListEmptyComponent={
          <EmptyContent
            translationKey={translationKey}
            illustrationIconKey="undrawReading"
            emptyContainerMarginTop={emptyContainerMarginTop}
          />
        }
        estimatedItemSize={ESTIMATED_ITEM_SIZE}
        estimatedListSize={{ height: ESTIMATED_ITEM_SIZE * 5, width: width - 32 }}
        renderItem={({ item, index }) => (
          <GuideCard
            key={index}
            guide={item}
            marginBottom="$md"
            onResourcePress={onResourcePress}
          />
        )}
        refreshControl={<RefreshControl refreshing={isRefetching} onRefresh={refetch} />}
      />
    </YStack>
  );
};

export default ResourcesGuidesList;
