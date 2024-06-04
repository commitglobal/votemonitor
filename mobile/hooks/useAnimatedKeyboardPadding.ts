import { useEffect, useRef } from "react";
import { Keyboard, Animated, Easing, KeyboardEvent, Platform } from "react-native";
import { useSafeAreaInsets } from "react-native-safe-area-context";

const useAnimatedKeyboardPadding = (initialPadding: number) => {
  const insets = useSafeAreaInsets();
  // we consider the initial value for padding with insets, as we assume the keyboard is hidden
  const paddingBottom = useRef(new Animated.Value(initialPadding + insets.bottom)).current;

  useEffect(() => {
    // when keyboard is showing, change the paddingBottom to the initialPadding value + the keyboard height
    const keyboardWillShow = (event: KeyboardEvent) => {
      Animated.timing(paddingBottom, {
        duration: 0,
        toValue: initialPadding + event.endCoordinates.height,
        easing: Easing.linear,
        useNativeDriver: false,
      }).start();
    };

    // when keyboard is hiding, we reset the padding to the initialPadding + insets.bottom
    const keyboardWillHide = (_event: KeyboardEvent) => {
      Animated.timing(paddingBottom, {
        duration: 0,
        toValue: initialPadding + insets.bottom,
        easing: Easing.linear,
        useNativeDriver: false,
      }).start();
    };

    const keyboardWillShowSub =
      Platform.OS === "ios"
        ? Keyboard.addListener("keyboardWillShow", keyboardWillShow)
        : Keyboard.addListener("keyboardDidShow", keyboardWillShow);
    const keyboardWillHideSub =
      Platform.OS === "ios"
        ? Keyboard.addListener("keyboardWillHide", keyboardWillHide)
        : Keyboard.addListener("keyboardDidHide", keyboardWillHide);

    return () => {
      keyboardWillShowSub.remove();
      keyboardWillHideSub.remove();
    };
  }, [initialPadding, insets.bottom]);

  return paddingBottom;
};

export default useAnimatedKeyboardPadding;
