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
// import { tokens } from "../theme/tokens";

interface HeaderProps {
  backgroundColor?: string;
  // title
  title: string;
  titleColor?: string;
  style?: StyleProp<ViewStyle>;
  // left icon
  leftIcon: React.ReactNode;
  leftIconColor?: string;
  onLeftPress?: TouchableOpacityProps["onPress"];
  // right icon
  rightIconColor?: string;
  rightIcon: React.ReactNode;
  onRightPress?: TouchableOpacityProps["onPress"];
}

const Header = ({
  backgroundColor = "gray",
  title,
  titleColor,
  style,
  leftIcon,
  leftIconColor,
  onLeftPress,
  rightIcon,
  rightIconColor,
  onRightPress,
}: HeaderProps) => {
  const insets = useSafeAreaInsets();
  const theme = useTheme();

  return (
    <XStack
      style={{
        ...style,
        height: 50 + insets.top,
        paddingTop: insets.top,
        backgroundColor: backgroundColor,
      }}
      justifyContent="space-between"
      alignItems="center"
    >
      {/* left header */}
      <StatusBar barStyle={"light-content"} />
      <TouchableOpacity
        onPress={leftIcon && onLeftPress ? onLeftPress : () => void 0}
        style={$leftIconContainer}
      >
        {leftIcon ? leftIcon : null}
      </TouchableOpacity>

      <Typography preset="body2" style={{ ...$title, color: titleColor }}>
        {title}
      </Typography>

      {/* right header */}
      <TouchableOpacity
        onPress={rightIcon && onRightPress ? onRightPress : () => void 0}
        style={$rightIconContainer}
      >
        {rightIcon ? rightIcon : null}
      </TouchableOpacity>
    </XStack>
  );
};

const $leftIconContainer: ViewStyle = {
  flex: 1,
  // padding for larger tapping space
  // paddingVertical: tokens.space.sm.val,
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
  // paddingVertical: tokens.space.sm.val,
  paddingRight: 14,
  flexDirection: "row",
  justifyContent: "flex-end",
};

export default Header;
