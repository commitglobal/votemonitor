import {
  StatusBar,
  StyleProp,
  TouchableOpacity,
  TouchableOpacityProps,
  ViewStyle,
} from "react-native";
import { useSafeAreaInsets } from "react-native-safe-area-context";
import { styled, XStack, YStack } from "tamagui";
import { Typography } from "./Typography";
import { tokens } from "../theme/tokens";

interface HeaderProps {
  /**
   * Background color
   */
  backgroundColor?: string;
  /**
   * Title
   */
  title?: string;
  /**
   * Title color
   */
  titleColor?: string;
  /**
   * Optional inner header wrapper style override.
   */
  style?: StyleProp<ViewStyle>;
  barStyle?: "light-content" | "dark-content" | "default";
  /**
   * Icon that should appear on the left.
   * Can be used with `onLeftPress`.
   */
  leftIcon?: React.ReactNode;
  /**
   * What happens when you press the left icon or text action.
   */
  onLeftPress?: TouchableOpacityProps["onPress"];
  /**
   * Icon that should appear on the right.
   * Can be used with `onRightPress`.
   */
  rightIcon?: React.ReactNode;
  /**
   * What happens when you press the right icon or text action.
   */
  onRightPress?: TouchableOpacityProps["onPress"];
  /**
   * A wider element (e.g. a labelled button) shown on the right instead of `rightIcon`.
   * It takes the width it needs (up to 45%) and the title wraps / shrinks to fit next to it.
   */
  rightAction?: React.ReactNode;
  /**
   * children
   */
  children?: React.ReactNode;
}

const StyledWrapper = styled(YStack, {
  name: "StyledWrapper",
  justifyContent: "center",
  alignItems: "center",
});

const Header = ({
  backgroundColor = "$purple5",
  barStyle = "light-content",
  title,
  titleColor = "white",
  style: $styleOverride,
  leftIcon,
  onLeftPress,
  rightIcon,
  onRightPress,
  rightAction,
  children,
}: HeaderProps) => {
  const insets = useSafeAreaInsets();

  return (
    <StyledWrapper
      backgroundColor={backgroundColor}
      minHeight={50 + insets.top}
      paddingTop={insets.top}
    >
      <XStack style={[$headerContainer, $styleOverride] as any}>
        {/* manipulating status bar icons to desired color */}
        <StatusBar barStyle={barStyle} />
        {/* left icon */}
        <TouchableOpacity
          onPress={leftIcon && onLeftPress ? onLeftPress : undefined}
          style={$leftIconContainer}
          disabled={!onLeftPress}
        >
          {leftIcon || null}
        </TouchableOpacity>
        {/* header title */}

        <YStack flex={6} justifyContent="center" alignItems="center">
          <Typography preset="body2" color={titleColor} textAlign="center" numberOfLines={2}>
            {title}
          </Typography>
        </YStack>
        {/* right icon */}
        {rightAction ? (
          <XStack style={$rightActionContainer}>{rightAction}</XStack>
        ) : (
          <TouchableOpacity
            onPress={rightIcon && onRightPress ? onRightPress : undefined}
            style={$rightIconContainer}
            disabled={!onRightPress}
          >
            {rightIcon || null}
          </TouchableOpacity>
        )}
      </XStack>
      {children}
    </StyledWrapper>
  );
};

const $headerContainer: ViewStyle = {
  justifyContent: "space-between",
  alignItems: "center",
};

const $leftIconContainer: ViewStyle = {
  flex: 1,
  // padding for larger tapping space
  paddingVertical: tokens.space.sm.val,
  paddingLeft: 14,
  flexDirection: "row",
  justifyContent: "flex-start",
};

const $rightIconContainer: ViewStyle = {
  flex: 1,
  // padding for larger tapping space
  paddingVertical: tokens.space.sm.val,
  paddingRight: 14,
  flexDirection: "row",
  justifyContent: "flex-end",
};

const $rightActionContainer: ViewStyle = {
  flexShrink: 0,
  maxWidth: "45%",
  paddingVertical: tokens.space.sm.val,
  paddingLeft: tokens.space.xs.val,
  paddingRight: 14,
  justifyContent: "flex-end",
};

export default Header;
