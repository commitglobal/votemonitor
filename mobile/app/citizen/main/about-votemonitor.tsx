import { Typography } from "../../../components/Typography";
import { Screen } from "../../../components/Screen";
import { useTranslation } from "react-i18next";
import { useSafeAreaInsets } from "react-native-safe-area-context";
import Constants from "expo-constants";
import { ScrollView, XStack, YStack } from "tamagui";
import Header from "../../../components/Header";
import { Icon } from "../../../components/Icon";
import { router } from "expo-router";
import { Image } from "expo-image";
import { useMemo } from "react";

const AboutVoteMonitor = () => {
  const { t } = useTranslation("about_app_citizen");
  const insets = useSafeAreaInsets();
  const appVersion = Constants.expoConfig?.version;

  const paragraphs = useMemo(() => {
    return [t("p1"), t("p2"), t("p3"), t("p4")];
  }, [t]);

  return (
    <Screen preset="fixed" contentContainerStyle={{ flexGrow: 1 }}>
      <Header
        title={t("title")}
        titleColor="white"
        barStyle="light-content"
        leftIcon={<Icon icon="chevronLeft" color="white" />}
        onLeftPress={() => router.back()}
      >
        <Icon icon="loginLogo" paddingVertical="$md" />
      </Header>

      <YStack flex={1} paddingBottom={insets.bottom + 24}>
        <ScrollView
          showsVerticalScrollIndicator={false}
          contentContainerStyle={{ flexGrow: 1, paddingHorizontal: 24 }}
        >
          <YStack gap="$lg">
            <Typography preset="subheading" fontWeight="500" paddingTop="$lg">
              {t("title_observer")}
            </Typography>

            {paragraphs.map((paragraph, index) => (
              <Typography key={index} preset="body1">
                {paragraph}
              </Typography>
            ))}

            <Typography preset="body1" color="$gray3">
              {t("version", { value: appVersion })} ({Constants.expoConfig?.extra?.updateVersion})
            </Typography>
          </YStack>

          <XStack justifyContent="center" alignItems="center" minHeight={115} marginBottom="$lg">
            <Image
              source={require("../../../assets/images/commit-global-color.png")}
              contentFit="contain"
              style={{ width: "100%", height: "100%" }}
            />
          </XStack>
        </ScrollView>
      </YStack>
    </Screen>
  );
};

export default AboutVoteMonitor;
