import React from "react";
import { StyleProp, ViewStyle, View} from "react-native";
import { Card as TamaguiCard, CardProps as TamaguiCardProps} from 'tamagui'
import { tokens } from "../theme/tokens";

export interface CardProps extends TamaguiCardProps {
    customStyle?: StyleProp<ViewStyle>
    children? : React.ReactNode;
}

/**
 * This component is a HOC over the Tamagui Button.
 * @param {ButtonProps} props - The props for the `Buttom` component.
 * @returns {JSX.Element} The rendered `Card` component.
 */
export function Card(props : CardProps) : JSX.Element {
    const {children, customStyle, ...rest} = props

    return (
        <View style={[{ padding: tokens.space.md.val, backgroundColor: 'white'}, customStyle]}>
          <TamaguiCard style={[{backgroundColor: 'white'}, customStyle]} {...rest} >
            {children}
          </TamaguiCard>
        </View>
    )
}