import React from "react";
import { Animated } from "react-native";
import PagerView, { PagerViewOnPageScrollEventData } from "react-native-pager-view";

import OnboardingItem from "./OnboardingItem";
import { useTranslation } from "react-i18next";

type OnboardingViewPagerProps = {
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
}: OnboardingViewPagerProps) => {
  const { t } = useTranslation("onboarding");
  const AnimatedPagerView = Animated.createAnimatedComponent(PagerView);

  return (
    // @ts-ignore
    <AnimatedPagerView
      ref={pagerViewRef}
      initialPage={currentPage}
      style={{ height: "100%", backgroundColor: "#5F288D" }}
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
        title={t("polling_stations.heading")}
        helper={t("polling_stations.description")}
      />
      <OnboardingItem
        key="2"
        icon="observationForms"
        title={t("forms.heading")}
        helper={t("forms.description")}
      />
      <OnboardingItem
        key="3"
        icon="notesOrMedia"
        title={t("media.heading")}
        helper={t("media.description")}
      />
    </AnimatedPagerView>
  );
};

export default OnboardingViewPager;
