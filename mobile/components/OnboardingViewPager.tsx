import React from "react";
import { Animated } from "react-native";
import PagerView, { PagerViewOnPageScrollEventData } from "react-native-pager-view";

import OnboardingItem from "./OnboardingItem";

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
        title="Monitor polling stations"
        helper="and document in real-time the entire election process, on forms which your accrediting
              NGO has set up"
      />
      <OnboardingItem
        key="2"
        icon="observationForms"
        title="Fill in observation forms"
        helper="and track their progress during each monitoring stage"
      />
      <OnboardingItem
        key="3"
        icon="notesOrMedia"
        title="Add notes or media files"
        helper="if you notice any problems to further support your answers"
      />
    </AnimatedPagerView>
  );
};

export default OnboardingViewPager;
