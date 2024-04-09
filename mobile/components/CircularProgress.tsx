import React, { useState, useEffect } from "react";
import { Animated as RNAnimated, View } from "react-native";
import { Svg, Circle } from "react-native-svg";
import Animated, { useAnimatedProps, useSharedValue, withTiming } from "react-native-reanimated";
import Button from "./Button";

// Pentru cerc
const size = 200;
const strokeWidth = 10;
const radius = (size - strokeWidth) / 2;
const CIRCLE_LENGTH = radius * 2 * Math.PI;
const duration = 1000;

const AnimatedCircle = Animated.createAnimatedComponent(Circle);

const CircularProgress = (): JSX.Element => {
  const [progress, setProgress] = useState(0);
  const animatedProgress = useSharedValue(progress);

  // Progress Indicator animation
  useEffect(() => {
    animatedProgress.value = withTiming(progress, { duration });
  }, [progress]);

  const animatedProps = useAnimatedProps(() => ({
    strokeDashoffset: (CIRCLE_LENGTH * (100 - animatedProgress.value)) / 100,
  }));

  // Background Circle
  // const progressLess = 100 - progress;
  // const animatedProgressLess = useSharedValue(progressLess);

  return (
    <View style={{ alignItems: "center", justifyContent: "center" }}>
      <Button
        preset="outlined"
        style={{ marginBottom: 20 }}
        onPress={() => {
          setProgress((prevProgress) => (prevProgress + 10) % 110);
        }}
      >
        Progress
      </Button>

      <View style={{ alignItems: "center", justifyContent: "center" }}>
        <Animated.Text
          style={{
            color: "#37306B",
            fontSize: 24,
            fontWeight: "bold",
            position: "absolute",
          }}
        >
          {Math.round(progress)} %
        </Animated.Text>

        <Svg width={size} height={size}>
          {/* Background Circle */}
          <Circle
            stroke="#D9D9D9"
            fill="none"
            cx={size / 2}
            cy={size / 2}
            r={radius}
            strokeWidth={strokeWidth}
          />

          {/* Progress indicator */}
          <AnimatedCircle
            cx={size / 2}
            cy={size / 2}
            r={radius}
            stroke="#00C282"
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
