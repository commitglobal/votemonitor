import React, { ReactNode, useEffect, useMemo } from "react";
import { Sheet, SheetProps } from "tamagui";
import { Icon } from "./Icon";
import { Animated, BackHandler, Platform } from "react-native";
import useAnimatedKeyboardPadding from "../hooks/useAnimatedKeyboardPadding";

export interface OptionsSheetProps extends SheetProps {
  /* The current state of the sheet */
  open: boolean;

  /* Control the state of the sheet */
  setOpen: (state: boolean) => void;

  /* For future: Triggered action for pressing "Clear form" */
  onClear?: () => void;

  isLoading?: boolean;

  children?: ReactNode;
}

const OptionsSheet = (props: OptionsSheetProps) => {
  const { open, setOpen, isLoading = false, children, ...rest } = props;

  // on Android back button press, if the sheet is open, we first close the sheet
  // and on the 2nd press we will navigate back
  useEffect(() => {
    if (Platform.OS !== "android") {
      return;
    }
    const onBackPress = () => {
      // close sheet
      if (open) {
        setOpen(false);
        return true;
      } else {
        // navigate back
        return false;
      }
    };
    const subscription = BackHandler.addEventListener("hardwareBackPress", onBackPress);
    return () => subscription.remove();
  }, [open, setOpen]);

  const AnimatedSheetFrame = useMemo(() => Animated.createAnimatedComponent(Sheet.Frame), []);
  const paddingBottom = useAnimatedKeyboardPadding(16);

  return (
    <Sheet
      modal
      open={open}
      onOpenChange={setOpen}
      zIndex={100_001}
      snapPointsMode="fit"
      dismissOnSnapToBottom={!isLoading}
      dismissOnOverlayPress={!isLoading}
      {...rest}
    >
      <Sheet.Overlay />
      <AnimatedSheetFrame
        borderTopLeftRadius={28}
        borderTopRightRadius={28}
        gap="$sm"
        paddingHorizontal="$md"
        // padding bottom responsive to keyboard presence
        paddingBottom={paddingBottom}
      >
        <Icon paddingVertical="$md" alignSelf="center" icon="dragHandle" />
        {children}
      </AnimatedSheetFrame>
    </Sheet>
  );
};

export default OptionsSheet;
