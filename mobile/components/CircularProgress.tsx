import React, { useState, useEffect } from "react";
import { View } from "react-native";
import { Svg, Circle } from "react-native-svg";
import Animated, { useSharedValue, withTiming, useAnimatedProps } from "react-native-reanimated";
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

  const gap = CIRCLE_LENGTH * 0.03;
  const seg1 = (CIRCLE_LENGTH * (100 - progress)) / 100 - 2 * gap;
  const seg2 = CIRCLE_LENGTH * (progress / 100);

  const animatedProps2 = useAnimatedProps(() => ({
    strokeDashoffset: (CIRCLE_LENGTH * (100 - animatedProgress.value)) / 100 - gap,
  }));

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
            stroke="#FFD209"
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
