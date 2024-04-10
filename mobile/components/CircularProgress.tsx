import React, { useEffect } from "react";
import { Svg, Circle } from "react-native-svg";
import Animated, {
  useSharedValue,
  withTiming,
  useAnimatedProps,
  useDerivedValue,
  interpolateColor,
  useAnimatedStyle,
  SharedValue,
} from "react-native-reanimated";
import { View } from "tamagui";

export interface CircularProgressProps {
  progress: number;
}

// TODO: adjust circle size by modifying these variables
const SIZE = 150;
const STROKEWIDTH = 8;
const RADIUS = (SIZE - STROKEWIDTH) / 2;
const CIRCLE_LENGTH = RADIUS * 2 * Math.PI;
const MAX_PROGRESS = 360;
const duration = 750;

const AnimatedCircle = Animated.createAnimatedComponent(Circle);

const CircularProgress = (props: CircularProgressProps): JSX.Element => {
  const { progress } = props;
  const animatedProgress = useSharedValue(progress);

  useEffect(() => {
    animatedProgress.value = withTiming(progress, { duration });
  }, [progress]);

  return (
    <View alignItems="center" justifyContent="center">
      <AnimatedText progress={progress} animatedProgress={animatedProgress} />

      <Svg width={SIZE} height={SIZE}>
        <BackgroundCircle animatedProgress={animatedProgress}></BackgroundCircle>
        <ProgressCircle animatedProgress={animatedProgress} />
      </Svg>
    </View>
  );
};

const ProgressCircle = ({ animatedProgress }: { animatedProgress: SharedValue<number> }) => {
  const strokeColor = useDerivedValue(() => {
    return interpolateColor(
      animatedProgress.value,
      [0, 1, MAX_PROGRESS - 1, MAX_PROGRESS],
      ["#E4E4E7", "#FFD209", "#FFD209", "#10B981"],
    );
  });

  const animatedProps = useAnimatedProps(() => ({
    strokeDashoffset: (CIRCLE_LENGTH * (MAX_PROGRESS - animatedProgress.value)) / MAX_PROGRESS,
    stroke: strokeColor.value,
  }));

  return (
    <AnimatedCircle
      cx={SIZE / 2}
      cy={SIZE / 2}
      r={RADIUS}
      strokeWidth={STROKEWIDTH}
      strokeLinecap="round"
      fill="none"
      strokeDasharray={`${CIRCLE_LENGTH} ${CIRCLE_LENGTH}`}
      animatedProps={animatedProps}
    />
  );
};

const BackgroundCircle = ({ animatedProgress }: { animatedProgress: SharedValue<number> }) => {
  const animatedProps = useAnimatedProps(() => {
    return {
      stroke: interpolateColor(
        animatedProgress.value,
        [0, 1, MAX_PROGRESS - 1, MAX_PROGRESS],
        ["#E4E4E7", "hsla(49, 100%, 58%, 0.25)", "hsla(49, 100%, 58%, 0.25)", "#10B981"],
      ),
    };
  });

  return (
    <AnimatedCircle
      fill="none"
      cx={SIZE / 2}
      cy={SIZE / 2}
      r={RADIUS}
      strokeWidth={STROKEWIDTH}
      animatedProps={animatedProps}
    />
  );
};

/**
 * Animated text which displays X % completed
 * Animation consist in color changing based on animatedProgress value.
 */
const AnimatedText = ({
  progress,
  animatedProgress,
}: {
  progress: number;
  animatedProgress: SharedValue<number>;
}) => {
  const animatedStyle = useAnimatedStyle(() => {
    return {
      color: interpolateColor(
        animatedProgress.value,
        [0, 1, MAX_PROGRESS - 1, MAX_PROGRESS],
        ["#71717A", "#A16207", "#A16207", "#10B981"],
      ),
    };
  });

  return (
    <View flexDirection="column" alignItems="center" position="absolute">
      <Animated.Text style={[animatedStyle, { fontSize: 14, fontWeight: "700" }]}>
        {Math.round(progress)} %
      </Animated.Text>
      <Animated.Text style={[animatedStyle, { fontSize: 12, fontWeight: "700" }]}>
        {"progress"}
      </Animated.Text>
    </View>
  );
};

export default CircularProgress;
