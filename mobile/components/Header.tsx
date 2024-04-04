import {
  StatusBar,
  StyleProp,
  TextStyle,
  TouchableOpacity,
  TouchableOpacityProps,
  ViewStyle,
} from "react-native";
import { useSafeAreaInsets } from "react-native-safe-area-context";
import { useTheme, XStack } from "tamagui";
import { Typography } from "./Typography";
import { tokens } from "../theme/tokens";

interface HeaderProps {
  backgroundColor?: string;
  barStyle?: "light-content" | "dark-content" | "default";
  // title
  title?: string;
  titleColor?: string;
  style?: StyleProp<ViewStyle>;
  // left icon
  leftIcon?: React.ReactNode;
  onLeftPress?: TouchableOpacityProps["onPress"];
  // right icon
  rightIcon?: React.ReactNode;
  onRightPress?: TouchableOpacityProps["onPress"];
}

const Header = ({
  backgroundColor = "gray",
  barStyle = "light-content",
  title,
  titleColor,
  style,
  leftIcon,
  onLeftPress,
  rightIcon,
  onRightPress,
}: HeaderProps) => {
  const insets = useSafeAreaInsets();

  return (
    <XStack
      style={[
        style,
        $headerContainer,
        {
          height: 50 + insets.top,
          paddingTop: insets.top,
          backgroundColor: backgroundColor,
        },
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
