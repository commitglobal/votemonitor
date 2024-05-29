import React from "react";
import { Animated, StyleSheet } from "react-native";
import { View, XStack } from "tamagui";

const DOT_SIZE = 30;

const Pagination = ({
  scrollOffsetAnimatedValue,
  positionAnimatedValue,
  data,
}: {
  scrollOffsetAnimatedValue: Animated.Value;
  positionAnimatedValue: Animated.Value;
  data: number[];
}) => {
  // todo: send the number of children of the AnimatedViewScroll?
  const translateX = Animated.add(scrollOffsetAnimatedValue, positionAnimatedValue).interpolate({
    inputRange: [0, data.length],
    outputRange: [0, data.length * DOT_SIZE],
  });

  return (
    <XStack backgroundColor="$purple6" position="relative" height={DOT_SIZE}>
      <Animated.View
        style={[
          styles.paginationIndicator,
          {
            position: "absolute",
            transform: [{ translateX }],
          },
        ]}
      />
      {data.map((item) => (
        <View width={DOT_SIZE} alignItems="center" justifyContent="center" key={item}>
          <View
            backgroundColor="white"
            borderRadius={DOT_SIZE * 0.15}
            height={DOT_SIZE * 0.3}
            width={DOT_SIZE * 0.3}
          />
        </View>
      ))}
    </XStack>
  );
};

export default Pagination;

const styles = StyleSheet.create({
  paginationIndicator: {
    borderColor: "#ddd",
    borderRadius: DOT_SIZE / 2,
    borderWidth: 2,
    height: DOT_SIZE,
    width: DOT_SIZE,
  },
});
