import React, { useState } from "react";
import { GestureResponderEvent, TextStyle } from "react-native";
import { GetProps, styled, useTheme, XStack } from "tamagui";
import { Typography } from "./Typography";

type PresetType = "default" | "outlined" | "chromeless" | "red" | "yellow";

/*
  Built on a plain XStack instead of tamagui's Button on purpose: on Android, tamagui's Button kept
  its internal "pressed" state after the press ended and stayed drawn with the pressed theme
  (faded, with a grey border, or grey like a disabled button) until the screen was left.
  The pressed and disabled looks are therefore plain variants, driven by our own state and props.
*/
const ButtonFrame = styled(XStack, {
  name: "AppButton",
  role: "button",
  focusable: true,
  flexDirection: "row",
  flexWrap: "nowrap",
  justifyContent: "center",
  alignItems: "center",
  gap: "$xs",
  height: "$true",
  paddingHorizontal: "$md",
  paddingVertical: "$xs",
  borderRadius: 8,
  borderWidth: 1,
  borderColor: "transparent",
  backgroundColor: "$purple5",
  variants: {
    presets: {
      default: {},
      outlined: {
        borderColor: "$purple5",
        backgroundColor: "transparent",
      },
      chromeless: {
        backgroundColor: "transparent",
      },
      red: {
        backgroundColor: "$red10",
      },
      yellow: {
        backgroundColor: "$yellow5",
      },
    },
    isPressed: {
      true: {
        opacity: 0.8,
      },
    },
    // filled presets turn grey, the transparent ones fade
    disabledLook: {
      filled: {
        backgroundColor: "$gray3",
        borderColor: "transparent",
      },
      transparent: {
        opacity: 0.5,
      },
    },
  } as const,
});

type ButtonFrameProps = GetProps<typeof ButtonFrame>;

export interface ButtonProps extends Omit<
  ButtonFrameProps,
  "presets" | "isPressed" | "disabledLook" | "children"
> {
  children: string;
  /**
   * One of the different types of button presets.
   */
  preset?: PresetType;
  /**
   * Optional styling for overriding presetType text styling
   */
  textStyle?: TextStyle;
  /**
   * Shown before the text
   */
  icon?: React.ReactNode | (() => React.ReactNode);
  disabled?: boolean;
}

/**
 * Button components which supports 3 initial presets: filled, outlined and chromeless
 * @param {ButtonProps} props - The props for the `Button` component.
 * @returns {JSX.Element} The rendered `Button` component.
 */
const Button = (props: ButtonProps): React.ReactElement => {
  const theme = useTheme();
  const [isPressed, setIsPressed] = useState(false);

  const {
    style: $styleOverride,
    children,
    textStyle,
    icon,
    preset,
    disabled,
    onPress,
    onPressIn,
    onPressOut,
    ...rest
  } = props;
  const presetType: PresetType = preset ?? "default";
  const $presetTextStyles = {
    color: presetType === "default" || presetType === "red" ? "white" : theme.$purple5?.val,
  };

  const $textStyles: TextStyle = { ...$presetTextStyles, ...textStyle };

  return (
    <ButtonFrame
      presets={presetType}
      isPressed={isPressed && !disabled}
      disabledLook={
        disabled
          ? presetType === "outlined" || presetType === "chromeless"
            ? "transparent"
            : "filled"
          : undefined
      }
      aria-disabled={disabled}
      pointerEvents={disabled ? "none" : "auto"}
      style={$styleOverride}
      onPress={
        disabled
          ? undefined
          : (e: GestureResponderEvent) => {
              // onPressOut is not always delivered on Android when the press re-renders the screen,
              // onPress is, so the pressed look is also cleared here
              setIsPressed(false);
              onPress?.(e);
            }
      }
      onPressIn={(e: GestureResponderEvent) => {
        setIsPressed(true);
        onPressIn?.(e);
      }}
      onPressOut={(e: GestureResponderEvent) => {
        setIsPressed(false);
        onPressOut?.(e);
      }}
      {...rest}
    >
      {typeof icon === "function" ? icon() : icon}
      <Typography preset="body2" style={$textStyles}>
        {children}
      </Typography>
    </ButtonFrame>
  );
};

export default Button;
