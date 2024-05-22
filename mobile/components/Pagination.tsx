import React from "react";
import { Animated, StyleSheet } from "react-native";
import { View } from "tamagui";

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

  const inputRange = [0, data.length];
  const translateX = Animated.add(scrollOffsetAnimatedValue, positionAnimatedValue).interpolate({
    inputRange,
    outputRange: [0, data.length * DOT_SIZE],
  });

  return (
    <View style={styles.pagination}>
      <Animated.View
        style={[
          styles.paginationIndicator,
          {
            position: "absolute",
            transform: [{ translateX }],
          },
        ]}
      />
      {data.map((item) => {
        return (
          <View key={item} style={styles.paginationDotContainer}>
            <View style={styles.paginationDot} />
          </View>
        );
      })}
    </View>
  );
};

export default Pagination;

const DOT_SIZE = 30;

const styles = StyleSheet.create({
  pagination: {
    backgroundColor: "#5F288D",
    flexDirection: "row",
    height: DOT_SIZE,
  },
  paginationDot: {
    backgroundColor: "white",
    borderRadius: DOT_SIZE * 0.15,
    height: DOT_SIZE * 0.3,
    width: DOT_SIZE * 0.3,
  },
  paginationDotContainer: {
    alignItems: "center",
    justifyContent: "center",
    width: DOT_SIZE,
  },
  paginationIndicator: {
    borderColor: "#ddd",
    borderRadius: DOT_SIZE / 2,
    borderWidth: 2,
    height: DOT_SIZE,
    width: DOT_SIZE,
  },
});
