import React from "react";
import { StyleProp, TextStyle, ViewStyle } from "react-native";
import {
  Button as TamaguiButton,
  ButtonProps as TamaguiButtonProps,
  styled,
  useTheme,
} from "tamagui";
import { Typography } from "./Typography";

type PresetType = "default" | "outlined" | "chromeless" | "red" | "yellow";
export interface ButtonProps extends TamaguiButtonProps {
  children?: string;
  /**
   * Style overrides
   */
  style?: StyleProp<ViewStyle>;
  /**
   * One of the different types of button presets.
   */
  preset?: PresetType;
  /**
   * Optional styling for overriding presetType text styling
   */
  textStyle?: TextStyle;
}

/**
 * Button components which supports 3 initial presets: filled, outlined and chromeless
 * This component is a HOC over the Tamagui Button.
 * @param {ButtonProps} props - The props for the `Button` component.
 * @returns {JSX.Element} The rendered `Button` component.
 */
const Button = React.forwardRef((props: ButtonProps, _): JSX.Element => {
  const theme = useTheme();

  const { style: $styleOverride, children, textStyle, ...rest } = props;
  const presetType: PresetType = props.preset ?? "default";
  const $presetTextStyles = {
    color: presetType === "default" || presetType === "red" ? "white" : theme.$purple5?.val,
  };

  const $textStyles: TextStyle = { ...$presetTextStyles, ...textStyle };

  const StyledButton = styled(TamaguiButton, {
    name: "StyledButton",
    paddingHorizontal: "$md",
    paddingVertical: "$xs",
    borderRadius: 8,
    backgroundColor: "$purple5",
    alignItems: "center",
    disabledStyle: {
      backgroundColor: "$gray3",
    },
    variants: {
      presets: {
        default: {
          pressStyle: {
            backgroundColor: "$purple5",
            opacity: 0.8,
          },
        },
        outlined: {
          borderWidth: 1,
          borderColor: "$purple5",
          backgroundColor: "transparent",
          pressStyle: {
            backgroundColor: "transparent",
            opacity: 0.8,
            borderColor: "$purple7",
          },
          disabledStyle: {
            backgroundColor: "transparent",
            opacity: 0.5,
          },
        },
        chromeless: {
          backgroundColor: "transparent",
          pressStyle: {
            backgroundColor: "transparent",
            opacity: 0.8,
            borderColor: "transparent",
          },
          disabledStyle: {
            backgroundColor: "transparent",
            opacity: 0.5,
          },
        },
        red: {
          backgroundColor: "$red10",
          pressStyle: {
            backgroundColor: "$red10",
            opacity: 0.8,
          },
        },
        yellow: {
          backgroundColor: "$yellow5",
          color: "$purple6",
          pressStyle: {
            backgroundColor: "$yellow5",
            opacity: 0.8,
            borderColor: "$yellow5",
          },
        },
      },
    } as const,
  });

  return (
    <StyledButton presets={presetType} style={$styleOverride} {...rest}>
      <Typography preset="body2" style={$textStyles}>
        {children}
      </Typography>
    </StyledButton>
  );
});

export default Button;
