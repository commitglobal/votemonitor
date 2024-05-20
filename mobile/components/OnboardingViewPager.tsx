import React from "react";
import { Animated } from "react-native";
import PagerView, { PagerViewOnPageScrollEventData } from "react-native-pager-view";

import OnboardingItem from "./OnboardingItem";
import { useTranslation } from "react-i18next";

type OnboardingViewPager = {
  scrollOffsetAnimatedValue: Animated.Value;
  positionAnimatedValue: Animated.Value;
  pagerViewRef: React.MutableRefObject<null>;
  currentPage: number;
  setCurrentPage: React.Dispatch<React.SetStateAction<number>>;
};

const OnboardingViewPager = ({
  scrollOffsetAnimatedValue,
  positionAnimatedValue,
  pagerViewRef,
  currentPage,
  setCurrentPage,
}: OnboardingViewPager) => {
  const { t } = useTranslation("login");
  const AnimatedPagerView = Animated.createAnimatedComponent(PagerView);

  return (
    // @ts-ignore
    <AnimatedPagerView
      ref={pagerViewRef}
      initialPage={currentPage}
      style={{ flex: 1, backgroundColor: "#5F288D" }}
      orientation="horizontal"
      onPageScroll={Animated.event<PagerViewOnPageScrollEventData>(
        [
          {
            nativeEvent: {
              offset: scrollOffsetAnimatedValue,
              position: positionAnimatedValue,
            },
          },
        ],
        {
          useNativeDriver: true,
        },
      )}
      onPageSelected={(e) => setCurrentPage(e.nativeEvent.position)}
    >
      <OnboardingItem
        key="1"
        icon="monitorPollingStations"
        title={t("onboarding.monitor_polling_stations.title")}
        helper={t("onboarding.monitor_polling_stations.description")}
      />
      <OnboardingItem
        key="2"
        icon="observationForms"
        title={t("onboarding.observation_forms.title")}
        helper={t("onboarding.observation_forms.description")}
      />
      <OnboardingItem
        key="3"
        icon="notesOrMedia"
        title={t("onboarding.notes_media.title")}
        helper={t("onboarding.notes_media.description")}
      />
    </AnimatedPagerView>
  );
};

export default OnboardingViewPager;
