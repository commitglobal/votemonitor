import { useEffect, useRef } from "react";
import { Keyboard, Animated, Easing, KeyboardEvent } from "react-native";
import { useSafeAreaInsets } from "react-native-safe-area-context";

const useAnimatedBottomPadding = (initialPadding: number) => {
  const insets = useSafeAreaInsets();
  // we consider the initial value for padding with insets, as we assume the keyboard is hidden
  const paddingBottom = useRef(new Animated.Value(initialPadding + insets.bottom)).current;

  useEffect(() => {
    // when keyboard is showing, change the paddingBottom to the initialPadding value (no insets)
    const keyboardWillShow = (_event: KeyboardEvent) => {
      Animated.timing(paddingBottom, {
        duration: 0,
        toValue: initialPadding,
        easing: Easing.linear,
        useNativeDriver: false,
      }).start();
    };

    // when keyboard is hiding, we add the insets.bottom to the padding
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
  }, [initialPadding, insets.bottom]);

  return paddingBottom;
};

export default useAnimatedBottomPadding;
