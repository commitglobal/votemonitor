import React, { ReactNode, useEffect } from "react";
import { useSafeAreaInsets } from "react-native-safe-area-context";
import { Sheet, SheetProps } from "tamagui";
import { Icon } from "./Icon";
import { KeyboardAwareScrollView } from "react-native-keyboard-aware-scroll-view";
import { BackHandler, Platform } from "react-native";

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
  const insets = useSafeAreaInsets();

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
        paddingBottom="$xl"
        marginBottom={insets.bottom}
      >
        <Icon paddingVertical="$md" alignSelf="center" icon="dragHandle" />
        <KeyboardAwareScrollView keyboardShouldPersistTaps="handled">
          {children}
        </KeyboardAwareScrollView>
      </Sheet.Frame>
    </Sheet>
  );
};

export default OptionsSheet;
