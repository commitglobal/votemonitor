import React, { useEffect } from "react";
import { Svg, Circle } from "react-native-svg";
import Animated, {
  useSharedValue,
  useAnimatedProps,
  withTiming,
  interpolateColor,
  useAnimatedStyle,
} from "react-native-reanimated";
import { View, Text } from "tamagui";

const AnimatedCircle = Animated.createAnimatedComponent(Circle);
const AnimatedText = Animated.createAnimatedComponent(Text);

export interface CircularProgressProps {
  progress: number;
  size?: number;
  strokeWidth?: number;
  progressColors?: [string, string, string];
  backgroundColors?: [string, string, string];
  textColors?: [string, string, string];
  duration?: number;
  showPercentage?: boolean;
  label?: string;
}

const CircularProgress: React.FC<CircularProgressProps> = ({
  progress,
  size = 150,
  strokeWidth = 8,
  progressColors = ["#E4E4E7", "#FFD209", "#10B981"],
  backgroundColors = ["#E4E4E7", "rgba(255, 210, 9, 0.25)", "rgba(16, 185, 129, 0.25)"],
  textColors = ["#71717A", "#A16207", "#10B981"],
  duration = 750,
  showPercentage = true,
  label = "Progress",
}) => {
  // Calculate dimensions
  const radius = (size - strokeWidth) / 2;
  const circumference = radius * 2 * Math.PI;
  const center = size / 2;

  // Clamp progress between 0 and 100
  const clampedProgress = Math.min(Math.max(progress, 0), 100);

  // Shared value for animation - always starts at 0 on mount for consistent behavior
  const animatedProgress = useSharedValue(0);

  // Update progress when prop changes
  useEffect(() => {
    animatedProgress.value = withTiming(clampedProgress, {
      duration,
    });
  }, [clampedProgress, duration, animatedProgress]);

  // Animated props for progress circle
  const progressCircleProps = useAnimatedProps(() => {
    "worklet";
    const strokeDashoffset = circumference - (circumference * animatedProgress.value) / 100;
    const color = interpolateColor(animatedProgress.value, [0, 50, 100], progressColors);

    return {
      strokeDashoffset,
      stroke: color,
    };
  });

  // Animated props for background circle
  const backgroundCircleProps = useAnimatedProps(() => {
    "worklet";
    const color = interpolateColor(animatedProgress.value, [0, 50, 100], backgroundColors);

    return {
      stroke: color,
    };
  });

  // Animated text style
  const textStyle = useAnimatedStyle(() => {
    return {
      color: interpolateColor(animatedProgress.value, [0, 50, 100], textColors),
    };
  });

  return (
    <View alignItems="center" justifyContent="center">
      {showPercentage && (
        <View flexDirection="column" alignItems="center" position="absolute" zIndex={10}>
          <AnimatedText
            style={[
              textStyle,
              {
                fontSize: 16,
                fontWeight: "700",
              },
            ]}
          >
            {Math.round(clampedProgress)}%
          </AnimatedText>
          {label && (
            <AnimatedText
              style={[
                textStyle,
                {
                  fontSize: 12,
                  fontWeight: "600",
                  marginTop: 4,
                },
              ]}
            >
              {label}
            </AnimatedText>
          )}
        </View>
      )}

      <Svg width={size} height={size}>
        {/* Background Circle */}
        <AnimatedCircle
          cx={center}
          cy={center}
          r={radius}
          fill="none"
          strokeWidth={strokeWidth}
          animatedProps={backgroundCircleProps}
        />

        {/* Progress Circle */}
        <AnimatedCircle
          cx={center}
          cy={center}
          r={radius}
          fill="none"
          strokeWidth={strokeWidth}
          strokeDasharray={`${circumference} ${circumference}`}
          strokeLinecap="round"
          rotation="-90"
          origin={`${center}, ${center}`}
          animatedProps={progressCircleProps}
        />
      </Svg>
    </View>
  );
};

export default CircularProgress;
