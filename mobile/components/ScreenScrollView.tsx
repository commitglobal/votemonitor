import React, { createContext, forwardRef, useContext } from "react";
import { ScrollView, ScrollViewProps } from "react-native";
import {
  KeyboardAwareScrollView,
  KeyboardAwareScrollViewProps,
} from "react-native-keyboard-controller";

/**
 * Space kept between the focused input and the top of the keyboard (or of the footer, when the screen has one).
 */
const INPUT_KEYBOARD_SPACING = 16;

interface ScreenFooterContextType {
  /**
   * Height of the screen footer, 0 when the screen has no footer.
   */
  footerHeight: number;
  /**
   * Bottom inset included in the footer padding, which is hidden behind the keyboard when it opens.
   */
  bottomInset: number;
}

export const ScreenFooterContext = createContext<ScreenFooterContextType>({
  footerHeight: 0,
  bottomInset: 0,
});

export type ScreenScrollViewProps = ScrollViewProps & KeyboardAwareScrollViewProps;

/**
 * Keyboard aware scroll view to be used inside a `Screen`.
 * It keeps the focused input above the keyboard and above the screen footer (which sticks to the keyboard).
 */
export const ScreenScrollView = forwardRef<ScrollView, ScreenScrollViewProps>(
  ({ bottomOffset, extraKeyboardSpace, keyboardShouldPersistTaps = "handled", ...props }, ref) => {
    const { footerHeight, bottomInset } = useContext(ScreenFooterContext);
    const hasFooter = footerHeight > 0;

    return (
      <KeyboardAwareScrollView
        ref={ref}
        // when the keyboard is open, the visible part of the footer is its height without the bottom inset
        bottomOffset={
          bottomOffset ?? (hasFooter ? footerHeight - bottomInset : 0) + INPUT_KEYBOARD_SPACING
        }
        // the scroll view ends at the top of the footer, so the keyboard covers less of it
        extraKeyboardSpace={extraKeyboardSpace ?? (hasFooter ? -bottomInset : 0)}
        keyboardShouldPersistTaps={keyboardShouldPersistTaps}
        {...props}
      />
    );
  },
);

ScreenScrollView.displayName = "ScreenScrollView";
