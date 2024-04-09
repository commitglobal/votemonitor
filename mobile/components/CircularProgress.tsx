import React, { useState, useEffect } from "react";
import { View } from "react-native";
import { Svg, Circle } from "react-native-svg";
import Animated, {
  useSharedValue,
  withTiming,
  useAnimatedProps,
  useDerivedValue,
  interpolateColor,
} from "react-native-reanimated";
import Button from "./Button";
import { Typography } from "./Typography";

export interface CircularProgressProps {
  progress: number;
}

const size = 200;
const strokeWidth = 8;
const radius = (size - strokeWidth) / 2;
const CIRCLE_LENGTH = radius * 2 * Math.PI;
const duration = 1000;

const AnimatedCircle = Animated.createAnimatedComponent(Circle);

const CircularProgress = (props: CircularProgressProps): JSX.Element => {
  const { progress } = props;
  const animatedProgress = useSharedValue(progress);

  // Color
  const strokeColor = useDerivedValue(() => {
    return interpolateColor(
      animatedProgress.value,
      [0, 10, 99, 100],
      ["grey", "yellow", "yellow", "green"],
    );
  });

  // Progress Indicator animation
  useEffect(() => {
    animatedProgress.value = withTiming(progress, { duration });
  }, [progress]);

  const animatedProps = useAnimatedProps(() => ({
    strokeDashoffset: (CIRCLE_LENGTH * (100 - animatedProgress.value)) / 100,
    stroke: strokeColor.value,
  }));

  // Background Circle Animation
  const gap = progress == 0 ? 0 : CIRCLE_LENGTH * 0.05;
  const seg1 = (CIRCLE_LENGTH * (100 - progress)) / 100 - 2 * gap;
  const seg2 = CIRCLE_LENGTH * (progress / 100);

  const strokeColor2 = useDerivedValue(() => {
    return interpolateColor(
      animatedProgress.value,
      [0, 10, 99, 100],
      ["grey", "#FFFDD0", "#FFFDD0", "green"],
    );
  });

  const animatedProps2 = useAnimatedProps(() => ({
    strokeDashoffset: (CIRCLE_LENGTH * (100 - animatedProgress.value)) / 100 - gap,
    stroke: strokeColor2.value,
  }));

  return (
    <View style={{ alignItems: "center", justifyContent: "center" }}>
      <View style={{ alignItems: "center", justifyContent: "center" }}>
        <Animated.Text
          style={{
            color: "#37306B",
            fontSize: 24,
            fontWeight: "bold",
            position: "absolute",
          }}
        >
          <View style={{ flexDirection: "column", alignItems: "center" }}>
            <Typography>{Math.round(progress)} %</Typography>
            <Typography> completed </Typography>
          </View>
        </Animated.Text>

        <Svg width={size} height={size}>
          {/* Background Background Circle */}
          <Circle
            stroke="white"
            opacity={25}
            fill="none"
            cx={size / 2}
            cy={size / 2}
            r={radius}
            strokeWidth={strokeWidth}
          />

          {/* Background Circle */}
          <AnimatedCircle
            stroke="yellow"
            fill="none"
            strokeLinecap="round"
            cx={size / 2}
            cy={size / 2}
            r={radius}
            strokeWidth={strokeWidth}
            strokeDasharray={`${seg1} ${gap} ${seg2} ${gap}`}
            animatedProps={animatedProps2}
          />

          {/* Progress indicator */}
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
        </Svg>
      </View>
    </View>
  );
};

export default CircularProgress;
