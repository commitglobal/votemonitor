import { Spinner, YStack } from "tamagui";
import { Screen } from "../../../../components/Screen";
import Header from "../../../../components/Header";
import { Typography } from "../../../../components/Typography";
import { Icon } from "../../../../components/Icon";
import { useNavigation } from "expo-router";
import { DrawerActions } from "@react-navigation/native";
import { useTranslation } from "react-i18next";
import { useUserData } from "../../../../contexts/user/UserContext.provider";
import { useGuides } from "../../../../services/queries/guides.query";
import { ListView } from "../../../../components/ListView";
import { Guide, guideType } from "../../../../services/api/get-guides.api";
import { useSafeAreaInsets } from "react-native-safe-area-context";
import { useWindowDimensions } from "react-native";
import GuideCard from "../../../../components/GuideCard";
import * as Linking from "expo-linking";

const Guides = () => {
  const { activeElectionRound } = useUserData();
  const { data: guides, isLoading: isLoadingGuides } = useGuides(activeElectionRound?.id);

  const { t } = useTranslation("guides");
  const navigation = useNavigation();
  const insets = useSafeAreaInsets();
  const { height } = useWindowDimensions();
  const scrollHeight = height - insets.top - insets.bottom - 55 - 55;

  if (isLoadingGuides) {
    return (
      <Screen
        preset="auto"
        ScrollViewProps={{
          bounces: false,
        }}
        contentContainerStyle={{
          flexGrow: 1,
        }}
      >
        <Header
          title={activeElectionRound?.title}
          titleColor="white"
          barStyle="light-content"
          leftIcon={<Icon icon="menuAlt2" color="white" />}
          onLeftPress={() => navigation.dispatch(DrawerActions.openDrawer)}
          rightIcon={<Icon icon="dotsVertical" color="white" />}
          onRightPress={() => {
            console.log("Right icon pressed");
          }}
        />
        <YStack backgroundColor="white" flex={1} justifyContent="center" alignItems="center">
          <Spinner size="large" color="$purple5" />
        </YStack>
      </Screen>
    );
  }

  return (
    <Screen
      preset="auto"
      ScrollViewProps={{
        bounces: false,
      }}
      contentContainerStyle={{
        flexGrow: 1,
      }}
    >
      <Header
        title={activeElectionRound?.title}
        titleColor="white"
        barStyle="light-content"
        leftIcon={<Icon icon="menuAlt2" color="white" />}
        onLeftPress={() => navigation.dispatch(DrawerActions.openDrawer)}
        rightIcon={<Icon icon="dotsVertical" color="white" />}
        // todo: manage my polling stations
        onRightPress={() => {
          console.log("Right icon pressed");
        }}
      />
      {guides && guides.length !== 0 ? (
        <YStack paddingHorizontal="$md" paddingTop="$xl" paddingBottom="$md" height={scrollHeight}>
          <Typography preset="body1" fontWeight="700" marginBottom="$xs">
            {t("list.heading")}
          </Typography>
          <ListView<Guide>
            data={guides}
            showsVerticalScrollIndicator={false}
            bounces={false}
            renderItem={({ item, index }) => {
              return (
                <GuideCard
                  key={index}
                  guide={item}
                  marginBottom="$md"
                  onPress={
                    item.guideType === guideType.WEBSITE
                      ? () => Linking.openURL(item.websiteUrl)
                      : () => Linking.openURL(item.presignedUrl)
                  }
                />
              );
            }}
            estimatedItemSize={115}
          />
        </YStack>
      ) : (
        <YStack flex={1} alignItems="center" justifyContent="center" gap="$md">
          <Icon icon="undrawReading" size={190} />

          <YStack gap="$xxxs" paddingHorizontal="$lg">
            <Typography preset="subheading" textAlign="center">
              {t("empty.heading")}
            </Typography>
            <Typography preset="body1" textAlign="center" color="$gray12">
              {t("empty.paragraph")}
            </Typography>
          </YStack>
        </YStack>
      )}
    </Screen>
  );
};

export default Guides;
