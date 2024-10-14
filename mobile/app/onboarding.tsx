import * as SecureStore from "expo-secure-store";
import { SECURE_STORAGE_KEYS } from "../common/constants";
import { useRef, useState } from "react";
import ChooseOnboardingLanguage from "../components/ChooseOnboardingLanguage";
import OnboardingViewPager from "../components/OnboardingViewPager";
import { Screen } from "../components/Screen";
import { XStack, YStack } from "tamagui";
import { t } from "i18next";
import { Animated } from "react-native";
import { Typography } from "../components/Typography";
import { useSafeAreaInsets } from "react-native-safe-area-context";
import { useRouter } from "expo-router";

export default function Onboarding() {
  const [languageSelectionApplied, setLanguageSelectionApplied] = useState(false);

  const insets = useSafeAreaInsets();

  const scrollOffsetAnimatedValue = useRef(new Animated.Value(0)).current;
  const positionAnimatedValue = useRef(new Animated.Value(0)).current;
  const [currentPage, setCurrentPage] = useState(0);
  const pagerViewRef = useRef(null);

  const router = useRouter();

  const data = [1, 2, 3];

  const onOnboardingComplete = () => {
    SecureStore.setItemAsync(SECURE_STORAGE_KEYS.ONBOARDING_NEW_COMPLETE, "true");
    router.replace("select-app-mode");
  };

  const onNextButtonPress = () => {
    if (currentPage !== data.length - 1) {
      // @ts-ignore
      pagerViewRef?.current?.setPage(currentPage + 1);
    } else {
      onOnboardingComplete();
    }
  };

  if (!languageSelectionApplied) {
    return <ChooseOnboardingLanguage setLanguageSelectionApplied={setLanguageSelectionApplied} />;
  }

  return (
    <Screen preset="fixed">
      <OnboardingViewPager
        scrollOffsetAnimatedValue={scrollOffsetAnimatedValue}
        positionAnimatedValue={positionAnimatedValue}
        pagerViewRef={pagerViewRef}
        currentPage={currentPage}
        setCurrentPage={setCurrentPage}
      />

      <XStack
        backgroundColor="$purple6"
        padding="$md"
        paddingBottom={16}
        position="absolute"
        bottom={insets.bottom}
        justifyContent="center"
        width="100%"
      >
        {/* <Pagination
                  scrollOffsetAnimatedValue={scrollOffsetAnimatedValue}
                  positionAnimatedValue={positionAnimatedValue}
                  data={data}
                  currentPage={currentPage + 1}
                /> */}
        <YStack
          width="100%"
          alignItems="flex-end"
          padding="$xxs"
          pressStyle={{ opacity: 0.5 }}
          onPress={onNextButtonPress}
        >
          <Typography color="white" preset="body2" textAlign="center">
            {currentPage !== data.length - 1
              ? t("skip", { ns: "common" })
              : t("media.save", { ns: "onboarding" })}
          </Typography>
        </YStack>
      </XStack>
    </Screen>
  );
}
