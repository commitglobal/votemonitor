import { useCallback, useRef, useState } from "react";
import { useFocusEffect, useNavigation } from "expo-router";

// how long to wait for the stack's transitionEnd before showing the latest value anyway
const FALLBACK_MS = 1000;

/**
 * Returns `value` as it was when this screen was last settled: while another screen covers it, and until the
 * transition that reveals it again has ended, the previous value is kept.
 *
 * On Android (Fabric), changing the shape of a screen's view tree while react-native-screens re-attaches it during a
 * pop crashes the app with "addViewAt: failed to insert view". That happens when the screen on top saves and goes
 * back: the save updates the query cache this screen renders (optimistic update, then the response and the refetch)
 * in the middle of the animation. Holding the value back until the screen is settled moves that change after it.
 */
export function useSettledValue<T>(value: T): T {
  const navigation = useNavigation();
  const [settled, setSettled] = useState(true);
  const shown = useRef(value);

  useFocusEffect(
    useCallback(() => {
      // focus comes when the pop starts; the transition events belong to the route in the enclosing stack (this
      // screen can sit inside tabs and a drawer)
      let stack: any = navigation;
      while (stack && stack.getState?.()?.type !== "stack") stack = stack.getParent?.();
      const settle = () => setSettled(true);
      const timer = setTimeout(settle, FALLBACK_MS);
      const unsubscribe = stack?.addListener?.("transitionEnd", settle);
      return () => {
        clearTimeout(timer);
        unsubscribe?.();
        setSettled(false);
      };
    }, [navigation]),
  );

  if (settled) shown.current = value;
  return shown.current;
}
