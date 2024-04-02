import React from "react";
import { StyleProp, TextStyle, ViewStyle} from "react-native";
import { 
  Button as TamaguiButton, 
  ButtonProps as TamaguiButtonProps, 
  Variable, 
  useTheme, 
} from "tamagui";

import { UseThemeResult } from '@tamagui/web/src/hooks/useTheme';
import { Typography } from "./Typography";
import {tokens} from "../theme/tokens"



type PresetType = "default" | "outlined" | "chromeless" | "red";

export interface ButtonProps extends TamaguiButtonProps {
    text: String;

    style? : StyleProp<ViewStyle>;

    /**
     * One of the different types of button presets.
     */
    preset? : PresetType;

    /**
     * Optional styling for overriding presetType text styling
     */
    textStyle? : TextStyle;
}

/**
 * Button components which supports 3 initial presets: filled, outlined and chromeless
 * This component is a HOC over the Tamagui Button.
 * @param {ButtonProps} props - The props for the `Buttom` component.
 * @returns {JSX.Element} The rendered `Button` component.
 */
export function Button(props : ButtonProps): JSX.Element {
    const theme = useTheme()

    const {style: $styleOverride, text, textStyle, ...rest} = props;
    const presetType : PresetType = props.preset ?? "default";
    const preset = $presets(theme, tokens.space)[presetType];

    const $buttonStyles: StyleProp<ViewStyle> = [
      preset,
      $styleOverride,
    ];

    const $presetTextStyles = {
        color: presetType === 'default' || presetType === 'red' ?
          'white' :
          theme.$purple5?.val,
    };

    const $textStyles: TextStyle = {...$presetTextStyles, ...textStyle};

    return (
      <TamaguiButton {...rest} style={$buttonStyles}>
        <Typography
          preset="body1"
          style={{ ...$textStyles, fontWeight: "500" }}>
            {text}
        </Typography>
      </TamaguiButton>
    );
} 



const $presets = (
  theme: UseThemeResult,
  spacing: { [key: string]: Variable<number> }
) => {
  const $baseStyle : StyleProp<ViewStyle> = {
    paddingHorizontal: spacing.md.val,
    paddingVertical: spacing.xs.val,
    borderRadius: 8,
    backgroundColor : theme.$purple5?.val,
    alignItems: 'center',
  };

  return {
      default: $baseStyle, 

      outlined: {
          ...$baseStyle,
          borderWidth: 1,
          borderColor: theme.$purple5?.val,
          backgroundColor: 'transparent',
      } as StyleProp<ViewStyle>,

      chromeless: {
          ...$baseStyle,
          backgroundColor: 'transparent',
      } as StyleProp<ViewStyle>,

      red: {
        ...$baseStyle,
        backgroundColor: theme.$red10?.val,
      } as StyleProp<ViewStyle>,
  
  };

};