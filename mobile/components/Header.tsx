import {
  StatusBar,
  StyleProp,
  TextStyle,
  TouchableOpacity,
  TouchableOpacityProps,
  ViewStyle,
} from "react-native";
import { useSafeAreaInsets } from "react-native-safe-area-context";
import { XStack } from "tamagui";
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
  backgroundColor = "gray",
  barStyle = "light-content",
  title,
  titleColor,
  style: $styleOverride,
  leftIcon,
  onLeftPress,
  rightIcon,
  onRightPress,
}: HeaderProps) => {
  const insets = useSafeAreaInsets();

  return (
    <XStack
      style={[
        $headerContainer,
        {
          height: 50 + insets.top,
          paddingTop: insets.top,
          backgroundColor: backgroundColor,
        },
        $styleOverride,
      ]}
    >
      {/* manipulating status bar icons to desired color */}
      <StatusBar barStyle={barStyle} />

      {/* left icon */}
      <TouchableOpacity
        onPress={leftIcon && onLeftPress ? onLeftPress : () => void 0}
        style={$leftIconContainer}
        disabled={!onLeftPress}
      >
        {leftIcon ? leftIcon : null}
      </TouchableOpacity>

      {/* header title */}
      <Typography preset="body2" style={{ ...$title, color: titleColor }}>
        {title}
      </Typography>

      {/* right icon */}
      <TouchableOpacity
        onPress={rightIcon && onRightPress ? onRightPress : () => void 0}
        style={$rightIconContainer}
        disabled={!onRightPress}
      >
        {rightIcon ? rightIcon : null}
      </TouchableOpacity>
    </XStack>
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

const $title: TextStyle = {
  flex: 6,
  textAlign: "center",
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
