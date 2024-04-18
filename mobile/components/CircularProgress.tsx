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
  size?: number;
}

const AnimatedCircle = Animated.createAnimatedComponent(Circle);

const CircularProgress = (props: CircularProgressProps): JSX.Element => {
  const { progress: inputProgress } = props;

  const progress = (inputProgress * 360) / 100;
  const animatedProgress = useSharedValue(progress);

  const SIZE = props.size || 150;
  const STROKEWIDTH = 8;
  const RADIUS = (SIZE - STROKEWIDTH) / 2;
  const MAX_PROGRESS = 360;
  const duration = 750;

  useEffect(() => {
    animatedProgress.value = withTiming(progress, { duration });
  }, [progress]);

  return (
    <View alignItems="center" justifyContent="center">
      <AnimatedText
        animatedProgress={animatedProgress}
        maxProgress={MAX_PROGRESS}
        inputProgress={inputProgress}
      />

      <Svg width={SIZE} height={SIZE}>
        <BackgroundCircle
          animatedProgress={animatedProgress}
          maxProgress={MAX_PROGRESS}
          size={SIZE}
          radius={RADIUS}
          strokeWidth={STROKEWIDTH}
        ></BackgroundCircle>
        <ProgressCircle
          animatedProgress={animatedProgress}
          size={SIZE}
          radius={RADIUS}
          maxProgress={MAX_PROGRESS}
          strokeWidth={STROKEWIDTH}
        />
      </Svg>
    </View>
  );
};

const ProgressCircle = ({
  animatedProgress,
  size,
  radius,
  maxProgress,
  strokeWidth,
}: {
  animatedProgress: SharedValue<number>;
  size: number;
  radius: number;
  maxProgress: number;
  strokeWidth: number;
}) => {
  const CIRCLE_LENGTH = radius * 2 * Math.PI;

  const strokeColor = useDerivedValue(() => {
    return interpolateColor(
      animatedProgress.value,
      [0, 1, maxProgress - 1, maxProgress],
      ["#E4E4E7", "#FFD209", "#FFD209", "#10B981"],
    );
  });

  const animatedProps = useAnimatedProps(() => ({
    strokeDashoffset: (CIRCLE_LENGTH * (maxProgress - animatedProgress.value)) / maxProgress,
    stroke: strokeColor.value,
  }));

  return (
    <AnimatedCircle
      cx={size / 2}
      cy={size / 2}
      r={radius}
      strokeWidth={strokeWidth}
      strokeLinecap="round"
      fill="none"
      strokeDasharray={`${CIRCLE_LENGTH} ${CIRCLE_LENGTH}`}
      animatedProps={animatedProps}
    />
  );
};

const BackgroundCircle = ({
  animatedProgress,
  maxProgress,
  size,
  radius,
  strokeWidth,
}: {
  animatedProgress: SharedValue<number>;
  maxProgress: number;
  size: number;
  radius: number;
  strokeWidth: number;
}) => {
  const animatedProps = useAnimatedProps(() => {
    return {
      stroke: interpolateColor(
        animatedProgress.value,
        [0, 1, maxProgress - 1, maxProgress],
        ["#E4E4E7", "hsla(49, 100%, 58%, 0.25)", "hsla(49, 100%, 58%, 0.25)", "#10B981"],
      ),
    };
  });

  return (
    <AnimatedCircle
      fill="none"
      cx={size / 2}
      cy={size / 2}
      r={radius}
      strokeWidth={strokeWidth}
      animatedProps={animatedProps}
    />
  );
};

/**
 * Animated text which displays X % completed
 * Animation consist in color changing based on animatedProgress value.
 */
const AnimatedText = ({
  animatedProgress,
  maxProgress,
  inputProgress,
}: {
  animatedProgress: SharedValue<number>;
  maxProgress: number;
  inputProgress: number;
}) => {
  const animatedStyle = useAnimatedStyle(() => {
    return {
      color: interpolateColor(
        animatedProgress.value,
        [0, 1, maxProgress - 1, maxProgress],
        ["#71717A", "#A16207", "#A16207", "#10B981"],
      ),
    };
  });

  return (
    <View flexDirection="column" alignItems="center" position="absolute">
      <Animated.Text style={[animatedStyle, { fontSize: 14, fontWeight: "700" }]}>
        {Math.round(inputProgress)} %
      </Animated.Text>
      <Animated.Text style={[animatedStyle, { fontSize: 12, fontWeight: "700" }]}>
        {"progress"}
      </Animated.Text>
    </View>
  );
};

export default CircularProgress;
