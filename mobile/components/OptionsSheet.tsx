import React, { ReactNode } from "react";
import { useSafeAreaInsets } from "react-native-safe-area-context";
import { Sheet, SheetProps } from "tamagui";
import { Icon } from "./Icon";

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
        {children}
      </Sheet.Frame>
    </Sheet>
  );
};

export default OptionsSheet;
