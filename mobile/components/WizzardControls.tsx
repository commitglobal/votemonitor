import { XStack, XStackProps } from "tamagui";
import Button from "./Button";
import { Icon } from "./Icon";
import { useTranslation } from "react-i18next";
import { Animated } from "react-native";
import useAnimatedBottomPadding from "../hooks/useAnimatedBottomPadding";

interface WizzardControlsProps extends XStackProps {
  isFirstElement?: boolean;
  isLastElement?: boolean;
  isNextDisabled?: boolean;
  onPreviousButtonPress?: () => void;
  onActionButtonPress?: () => void;
  actionText?: string;
}

const WizzardControls = ({
  isFirstElement,
  isLastElement,
  isNextDisabled,
  onPreviousButtonPress,
  onActionButtonPress,
  actionText,
  ...rest
}: WizzardControlsProps) => {
  const { t } = useTranslation("add_polling_station");
  const paddingBottom = useAnimatedBottomPadding(16);

  const AnimatedXStack = Animated.createAnimatedComponent(XStack);

  return (
    <AnimatedXStack
      elevation={2}
      backgroundColor="pink"
      alignItems="center"
      gap="$sm"
      padding="$md"
      paddingBottom={paddingBottom}
      {...rest}
    >
      {!isFirstElement && (
        <XStack flex={0.25}>
          <Button
            width="100%"
            icon={() => <Icon icon="chevronLeft" color="$purple5" />}
            preset="chromeless"
            onPress={onPreviousButtonPress}
          >
            {t("actions.back")}
          </Button>
        </XStack>
      )}

      <XStack flex={isFirstElement ? 1 : 0.75}>
        <Button disabled={isNextDisabled} width="100%" onPress={onActionButtonPress}>
          {!isLastElement ? actionText || t("actions.next_step") : t("actions.finalize")}
        </Button>
      </XStack>
    </AnimatedXStack>
  );
};

export default WizzardControls;
