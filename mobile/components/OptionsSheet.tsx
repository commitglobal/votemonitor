import React, { ReactNode, useEffect, useMemo } from "react";
import { BackHandler, Platform } from "react-native";
import { Sheet, SheetProps, YStack } from "tamagui";
import { Icon } from "./Icon";
import useAnimatedKeyboardPadding from "../hooks/useAnimatedKeyboardPadding";
import { Animated, ScrollView } from "react-native";

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

  const animatedPaddingBottom = useAnimatedKeyboardPadding(16);
  const AnimatedYStack = useMemo(() => Animated.createAnimatedComponent(YStack), []);

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
      <Sheet.Frame
        borderTopLeftRadius={28}
        borderTopRightRadius={28}
        gap="$sm"
        paddingHorizontal="$md"
      >
        <Icon paddingVertical="$md" alignSelf="center" icon="dragHandle" />
        <ScrollView keyboardShouldPersistTaps="handled">
          <AnimatedYStack paddingBottom={animatedPaddingBottom}>{children}</AnimatedYStack>
        </ScrollView>
      </Sheet.Frame>
    </Sheet>
  );
};

export default OptionsSheet;
