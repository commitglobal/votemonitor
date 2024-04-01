import React from "react";
import { StyleProp, ViewStyle} from "react-native";
import { Button as TamaguiButton, ButtonProps as TamaguiButtonProps } from "tamagui";

/*
  TODO(maybe):
  Currently, the border of button changes its color to grey while pressing.
  Cutomize this?
*/

type Presets = keyof typeof $presets

export interface ButtonProps extends TamaguiButtonProps {
    style? : StyleProp<ViewStyle>;

    /**
     * One of the different types of button presets.
     */
    preset? : Presets;

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
    const {children, style: $styleOverride, ...rest} = props;

    const preset : Presets = props.preset ?? "default";
    const $styles: StyleProp<ViewStyle> = [
      $presets[preset],
      $styleOverride,
    ];

    return (
      <TamaguiButton {...rest} style={$styles}>
        {children}
      </TamaguiButton>
    );
}

const $baseStyle : StyleProp<ViewStyle> = {
    paddingHorizontal: 16,
    paddingVertical: 8,
    backgroundColor : "#7833B3",
    alignItems: 'center',
    borderRadius: 8,
};

const $presets = {
    default: $baseStyle,

    outlined: {
        ...$baseStyle,
        borderWidth: 1,
        borderColor: '#7833B3',
        backgroundColor: 'transparent',
    } as StyleProp<ViewStyle>,

    chromeless: {
        ...$baseStyle,
        backgroundColor: 'transparent',
    } as StyleProp<ViewStyle>,

};