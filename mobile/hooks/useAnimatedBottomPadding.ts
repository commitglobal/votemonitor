import { useEffect, useRef } from "react";
import { Keyboard, Animated, Easing, KeyboardEvent } from "react-native";
import { useSafeAreaInsets } from "react-native-safe-area-context";

const useAnimatedBottomPadding = (initialPadding: number) => {
  const insets = useSafeAreaInsets();
  const paddingBottom = useRef(new Animated.Value(initialPadding + insets.bottom)).current;

  useEffect(() => {
    const keyboardWillShow = (_event: KeyboardEvent) => {
      Animated.timing(paddingBottom, {
        duration: 0,
        toValue: initialPadding, // Adjust this value as needed
        easing: Easing.linear,
        useNativeDriver: false,
      }).start();
    };

    const keyboardWillHide = (_event: KeyboardEvent) => {
      Animated.timing(paddingBottom, {
        duration: 0,
        toValue: initialPadding + insets.bottom,
        easing: Easing.linear,
        useNativeDriver: false,
      }).start();
    };

    const keyboardWillShowSub = Keyboard.addListener("keyboardWillShow", keyboardWillShow);
    const keyboardWillHideSub = Keyboard.addListener("keyboardWillHide", keyboardWillHide);

    return () => {
      keyboardWillShowSub.remove();
      keyboardWillHideSub.remove();
    };
  }, [insets.bottom]);

  return paddingBottom;
};

export default useAnimatedBottomPadding;
