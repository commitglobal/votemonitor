import { ScrollView, Spinner, YStack } from "tamagui";
import { Screen } from "../../../../components/Screen";
import Header from "../../../../components/Header";
import { Typography } from "../../../../components/Typography";
import { Icon } from "../../../../components/Icon";
import { router, useNavigation } from "expo-router";
import { DrawerActions } from "@react-navigation/native";
import { useTranslation } from "react-i18next";
import { useUserData } from "../../../../contexts/user/UserContext.provider";
import { useGuides } from "../../../../services/queries/guides.query";
import { ListView } from "../../../../components/ListView";
import { Guide, guideType } from "../../../../services/api/get-guides.api";
import { useWindowDimensions } from "react-native";
import GuideCard from "../../../../components/GuideCard";
import * as Linking from "expo-linking";
import { useState } from "react";
import OptionsSheet from "../../../../components/OptionsSheet";
import { RefreshControl } from "react-native-gesture-handler";

const ESTIMATED_ITEM_SIZE = 115;

const Guides = () => {
  const { t } = useTranslation("guides");
  const navigation = useNavigation();

  const { width } = useWindowDimensions();

  const { activeElectionRound } = useUserData();
  const {
    data: guides,
    isLoading: isLoadingGuides,
    refetch: refetchGuides,
    isRefetching: isRefetchingGuides,
  } = useGuides(activeElectionRound?.id);
  const [optionsSheetOpen, setOptionsSheetOpen] = useState(false);

  if (isLoadingGuides) {
    return (
      <Screen preset="fixed" contentContainerStyle={{ flexGrow: 1 }}>
        <Header
          title={t("title")}
          titleColor="white"
          barStyle="light-content"
          leftIcon={<Icon icon="menuAlt2" color="white" />}
          onLeftPress={() => navigation.dispatch(DrawerActions.openDrawer)}
        />
        <YStack flex={1} justifyContent="center" alignItems="center">
          <Spinner size="large" color="$purple5" />
        </YStack>
      </Screen>
    );
  }

  return (
    <Screen
      preset="fixed"
      contentContainerStyle={{
        flexGrow: 1,
      }}
    >
      <Header
        title={t("title")}
        titleColor="white"
        barStyle="light-content"
        leftIcon={<Icon icon="menuAlt2" color="white" />}
        onLeftPress={() => navigation.dispatch(DrawerActions.openDrawer)}
        rightIcon={<Icon icon="dotsVertical" color="white" />}
        onRightPress={() => setOptionsSheetOpen(true)}
      />
      {guides && guides.length !== 0 ? (
        <YStack padding="$md" flex={1}>
          <ListView<Guide>
            data={guides}
            showsVerticalScrollIndicator={false}
            bounces={true}
            ListHeaderComponent={
              guides?.length > 0 ? (
                <Typography preset="body1" fontWeight="700" marginBottom="$xs">
                  {t("list.heading")}
                </Typography>
              ) : (
                <></>
              )
            }
            estimatedItemSize={ESTIMATED_ITEM_SIZE}
            estimatedListSize={{ height: ESTIMATED_ITEM_SIZE * 5, width: width - 32 }}
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
            refreshControl={
              <RefreshControl refreshing={isRefetchingGuides} onRefresh={refetchGuides} />
            }
          />
        </YStack>
      ) : (
        <ScrollView
          contentContainerStyle={{
            flex: 1,
            alignItems: "center",
            justifyContent: "center",
            gap: 16,
          }}
          showsVerticalScrollIndicator={false}
          refreshControl={
            <RefreshControl refreshing={isRefetchingGuides} onRefresh={refetchGuides} />
          }
        >
          <Icon icon="undrawReading" size={190} />

          <YStack gap="$xxxs" paddingHorizontal="$lg">
            <Typography preset="subheading" textAlign="center">
              {t("empty.heading")}
            </Typography>
            <Typography preset="body1" textAlign="center" color="$gray12">
              {t("empty.paragraph")}
            </Typography>
          </YStack>
        </ScrollView>
      )}
      {optionsSheetOpen && (
        <OptionsSheet open setOpen={setOptionsSheetOpen} key={"GuidesSheet"}>
          <YStack
            paddingVertical="$xxs"
            paddingHorizontal="$sm"
            onPress={() => {
              setOptionsSheetOpen(false);
              router.push("/manage-polling-stations");
            }}
          >
            <Typography preset="body1" color="$gray7" lineHeight={24}>
              {t("options_sheet.manage_my_polling_stations")}
            </Typography>
          </YStack>
        </OptionsSheet>
      )}
    </Screen>
  );
};

export default Guides;
