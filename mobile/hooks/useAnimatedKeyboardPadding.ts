import { useEffect, useRef } from "react";
import { Keyboard, Animated, Easing, KeyboardEvent, Platform } from "react-native";
import { useSafeAreaInsets } from "react-native-safe-area-context";

// instead of using the 'moveOnKeyboardChange' prop for the bottomsheets, we can use this hook to add a custom padding depending on whether or not the keyboard is open;
// this approach has better response to the keyboard closing/opening, as well as to the situations where it's already opened when the bottomsheet appears;
// !this hook for padding can only be used for bottomsheets with snapPointsMode="fit" (so no snap points are taken into consideration)

// the initialPadding prop is the default padding the bottomsheet should have when the keyboard is not open
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

    // for ios we will listen for the 'keyboardWillShow'/'keyboardWillHide' events, so the position change of the bottomsheet will be fast and smooth
    // for android, we only have available the 'keyboardDidShow'/'keyboardDidHide' events, so in this case the bottomsheet will move only after the keyboard has moved
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
