import React, { useMemo, useState } from "react";
import { Typography } from "../../../../../components/Typography";
import { Spinner, View, YStack } from "tamagui";
import { Icon } from "../../../../../components/Icon";
import { Screen } from "../../../../../components/Screen";
import { router, useNavigation } from "expo-router";
import Header from "../../../../../components/Header";
import { DrawerActions } from "@react-navigation/native";
import Button from "../../../../../components/Button";
import OptionsSheet from "../../../../../components/OptionsSheet";
import { useQuickReports } from "../../../../../services/queries/quick-reports.query";
import { useUserData } from "../../../../../contexts/user/UserContext.provider";
import { ListView } from "../../../../../components/ListView";
import ReportCard from "../../../../../components/ReportCard";
import { useWindowDimensions, ViewStyle } from "react-native";
import { QuickReportsAPIResponse } from "../../../../../services/api/quick-report/get-quick-reports.api";
import { useTranslation } from "react-i18next";
import { useSafeAreaInsets } from "react-native-safe-area-context";

const QuickReport = () => {
  const navigation = useNavigation();
  const [openContextualMenu, setOpenContextualMenu] = useState(false);
  const { activeElectionRound } = useUserData();
  const { data: quickReports, isLoading, error } = useQuickReports(activeElectionRound?.id);
  const { t } = useTranslation("quick_report");

  return (
    <>
      <Screen
        preset="auto"
        ScrollViewProps={{
          showsVerticalScrollIndicator: false,
          stickyHeaderIndices: [0],
          bounces: false,
        }}
        contentContainerStyle={$containerStyle}
      >
        <Header
          title={"Quick report"}
          titleColor="white"
          barStyle="light-content"
          leftIcon={<Icon icon="menuAlt2" color="white" />}
          onLeftPress={() => navigation.dispatch(DrawerActions.openDrawer)}
          rightIcon={<Icon icon="dotsVertical" color="white" />}
          onRightPress={() => {
            setOpenContextualMenu(true);
          }}
        />
        {quickReports ? (
          <QuickReportContent quickReports={quickReports} isLoading={isLoading} error={error} />
        ) : (
          false
        )}
        {openContextualMenu && (
          <OptionsSheet open setOpen={setOpenContextualMenu}>
            <View
              paddingVertical="$xxs"
              paddingHorizontal="$sm"
              onPress={() => {
                setOpenContextualMenu(false);
                router.push("manage-polling-stations");
              }}
            >
              <Typography preset="body1" color="$gray7" lineHeight={24}>
                {t("options_menu.manage_my_polling_stations")}
              </Typography>
            </View>
          </OptionsSheet>
        )}
      </Screen>
      {quickReports?.length ? (
        <YStack width="100%" paddingHorizontal="$md" marginVertical="$xxs">
          <Button
            preset="outlined"
            backgroundColor="white"
            onPress={router.push.bind(null, "/report-issue")}
          >
            {t("list.add")}
          </Button>
        </YStack>
      ) : (
        false
      )}
    </>
  );
};

interface QuickReportContentProps {
  quickReports: QuickReportsAPIResponse[];
  isLoading: boolean;
  error: Error | null;
}

const ESTIMATED_ITEM_SIZE = 200;

const QuickReportContent = ({ quickReports, isLoading, error }: QuickReportContentProps) => {
  const { t } = useTranslation(["quick_report", "common"]);
  const insets = useSafeAreaInsets();
  const { height, width } = useWindowDimensions();
  const scrollHeight = useMemo(() => height - insets.top - insets.bottom - 100 - 60, []);

  if (isLoading) {
    return (
      <YStack justifyContent="center" alignItems="center" minHeight={scrollHeight}>
        <Spinner size="large" color="$purple5" />
      </YStack>
    );
  }

  if (error) {
    return <Typography>{t("list.error")}</Typography>;
  }

  return (
    <YStack padding="$md" minHeight={scrollHeight}>
      <ListView<any>
        data={quickReports}
        showsVerticalScrollIndicator={false}
        ListHeaderComponent={
          quickReports.length > 0 ? (
            <Typography
              marginBottom="$xxs"
              preset="body1"
              textAlign="left"
              color="$gray7"
              fontWeight="700"
            >
              {t("list.heading")}
            </Typography>
          ) : (
            <></>
          )
        }
        ListEmptyComponent={
          <YStack alignItems="center" justifyContent="center" gap="$md" marginTop="40%">
            <Icon icon="undrawFlag" />
            <YStack gap="$md" paddingHorizontal="$xl">
              <Typography preset="body1" textAlign="center" color="$gray12" lineHeight={24}>
                {t("list.empty")}
              </Typography>
              <Button
                preset="outlined"
                onPress={router.push.bind(null, "/report-issue")}
                backgroundColor="white"
              >
                {t("list.add")}
              </Button>
            </YStack>
          </YStack>
        }
        bounces={false}
        renderItem={({ item, index }) => (
          <ReportCard
            key={`${index}`}
            title={item.title}
            description={item.description}
            numberOfAttachments={item.attachments.length}
            onPress={() => router.push(`/report-details/${item.id}?reportTitle=${item.title}`)}
          />
        )}
        estimatedItemSize={ESTIMATED_ITEM_SIZE}
        estimatedListSize={
          !quickReports.length
            ? undefined
            : {
                height: ESTIMATED_ITEM_SIZE * 5,
                width: width - 32,
              }
        }
      />
    </YStack>
  );
};

const $containerStyle: ViewStyle = {
  flexGrow: 1,
};

export default QuickReport;
