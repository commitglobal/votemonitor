import {
  StatusBar,
  StyleProp,
  TouchableOpacity,
  TouchableOpacityProps,
  ViewStyle,
} from "react-native";
import { useSafeAreaInsets } from "react-native-safe-area-context";
import { styled, XStack } from "tamagui";
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
}

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
}: HeaderProps) => {
  const insets = useSafeAreaInsets();

  const StyledWrapper = styled(XStack, {
    name: "StyledWrapper",
    backgroundColor,
    minHeight: 50 + insets.top,
    paddingTop: insets.top,
  });

  return (
    <>
      <StyledWrapper style={[$headerContainer, $styleOverride]}>
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
        <Typography preset="body2" color={titleColor} flex={6} textAlign="center" numberOfLines={2}>
          {title}
        </Typography>

        {/* right icon */}
        <TouchableOpacity
          onPress={rightIcon && onRightPress ? onRightPress : undefined}
          style={$rightIconContainer}
          disabled={!onRightPress}
        >
          {rightIcon || null}
        </TouchableOpacity>
      </StyledWrapper>
    </>
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

export default Header;
