import {
  StatusBar,
  StyleProp,
  TextStyle,
  TouchableOpacity,
  TouchableOpacityProps,
  ViewStyle,
} from "react-native";
import { useSafeAreaInsets } from "react-native-safe-area-context";
import { styled, XStack, Stack } from "tamagui";
import { Typography } from "./Typography";
import { Icon } from "../components/Icon";
import { tokens } from "../theme/tokens";
import { useEffect, useState } from "react";
import NetInfo from "@react-native-community/netinfo";

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
  const [isOnline, setIsOnline] = useState(true);
  const [showNetInfoBanner, setShowNetInfoBanner] = useState(true);

  useEffect(() => {
    const unsubscribe = NetInfo.addEventListener((state) => {
      const status = !!state.isConnected;
      setIsOnline(status);
    });
    return unsubscribe();
  }, []);

  // show online banner again after user is connected again
  useEffect(() => {
    if (isOnline) setShowNetInfoBanner(true);
  }, [isOnline]);

  const insets = useSafeAreaInsets();

  const StyledWrapper = styled(XStack, {
    name: "StyledWrapper",
    backgroundColor,
    height: 50 + insets.top,
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
        <Typography preset="body2" style={{ ...$title, color: titleColor }}>
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
      {isOnline ? (
        showNetInfoBanner && (
          <XStack
            backgroundColor="$green1"
            paddingLeft={20}
            justifyContent="space-between"
            alignItems="center"
            position="absolute"
            width="100%"
            top={50 + insets.top}
          >
            <Typography fontWeight="500" color="$gray7">
              App online. All answers sent to server.
            </Typography>
            <Stack
              onPress={() => setShowNetInfoBanner(false)}
              paddingVertical="$xxs"
              paddingHorizontal={20}
            >
              <Icon icon="x" size={16} />
            </Stack>
          </XStack>
        )
      ) : (
        <XStack
          backgroundColor="$red1"
          paddingVertical="$xxs"
          paddingHorizontal={20}
          position="absolute"
          width="100%"
          top={50 + insets.top}
        >
          <Typography fontWeight="500" color="$gray7">
            Offline mode. Saving answers locally.
          </Typography>
        </XStack>
      )}
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
