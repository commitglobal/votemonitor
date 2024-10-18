import { useEffect, useRef } from "react";
import { Keyboard, Animated, Easing, KeyboardEvent } from "react-native";
import { useSafeAreaInsets } from "react-native-safe-area-context";
import { useNetInfoContext } from "../contexts/net-info-banner/NetInfoContext";

const useAnimatedBottomPadding = (initialPadding: number) => {
  const insets = useSafeAreaInsets();
  const { shouldDisplayBanner } = useNetInfoContext();
  const bottomInsets = useRef(insets.bottom);

  // we consider the initial value for padding with insets, as we assume the keyboard is hidden
  const paddingBottom = useRef(new Animated.Value(initialPadding + bottomInsets.current)).current;

  const animatePadding = (toValue: number) => {
    Animated.timing(paddingBottom, {
      duration: 100,
      toValue,
      easing: Easing.linear,
      useNativeDriver: false,
    }).start();
  };

  useEffect(() => {
    bottomInsets.current = shouldDisplayBanner ? 0 : insets.bottom;
    // Trigger animation when bottomInsets changes
    animatePadding(initialPadding + bottomInsets.current);
  }, [shouldDisplayBanner, insets.bottom, initialPadding]);

  useEffect(() => {
    // when keyboard is showing, change the paddingBottom to the initialPadding value (no insets)
    const keyboardWillShow = (_event: KeyboardEvent) => {
      animatePadding(initialPadding);
    };

    // when keyboard is hiding, we add the insets.bottom to the padding
    const keyboardWillHide = (_event: KeyboardEvent) => {
      animatePadding(initialPadding + bottomInsets.current);
    };

    const keyboardWillShowSub = Keyboard.addListener("keyboardWillShow", keyboardWillShow);
    const keyboardWillHideSub = Keyboard.addListener("keyboardWillHide", keyboardWillHide);

    return () => {
      keyboardWillShowSub.remove();
      keyboardWillHideSub.remove();
    };
  }, [initialPadding, bottomInsets.current]);

  return paddingBottom;
};

export default useAnimatedBottomPadding;
