import React from "react";
import { StyleProp, ViewStyle} from "react-native";
import { Button as TamaguiButton, ButtonProps as TamaguiButtonProps } from "tamagui";
import { useTheme } from 'tamagui'
import { getTokens } from 'tamagui'

/*
  TODO(maybe):
  Currently, the border of button changes its color to grey while pressing.
  Cutomize this?
*/

type PresetType = "default" | "outlined" | "chromeless" | "red";

export interface ButtonProps extends TamaguiButtonProps {
    style? : StyleProp<ViewStyle>;

    /**
     * One of the different types of button presets.
     */
    preset? : PresetType;

    /**
     * Children components.
     */
    children?: React.ReactNode;
}

/**
 * Button components which supports 3 initial presets: filled, outlined and chromeless
 * This component is a HOC over the Tamagui Button.
 * @param {ButtonProps} props - The props for the `Buttom` component.
 * @returns {JSX.Element} The rendered `Button` component.
 */
export function Button(props : ButtonProps): JSX.Element {
    const theme = useTheme()
    const tokens = getTokens()

    const {children, style: $styleOverride, ...rest} = props;
    const presetType : PresetType = props.preset ?? "default";

    const preset = $presets(theme, tokens)[presetType]

    const $styles: StyleProp<ViewStyle> = [
      preset,
      $styleOverride,
    ];

    return (
      <TamaguiButton {...rest} style={$styles}>
        {children}
      </TamaguiButton>
    );
}


const $presets = (theme: any, tokens: any) => {
  const $baseStyle : StyleProp<ViewStyle> = {
    paddingHorizontal: tokens.space.md,
    paddingVertical: tokens.space.xs,
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
        backgroundColor: theme.$red10.val,
      } as StyleProp<ViewStyle>,
  
  };

};