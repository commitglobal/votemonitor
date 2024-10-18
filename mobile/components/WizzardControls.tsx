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
  actionBtnLabel?: string;
  actionBtnPreset?: "default" | "outlined" | "chromeless" | "yellow" | "red";
}

const WizzardControls = ({
  isFirstElement,
  isLastElement,
  isNextDisabled,
  onPreviousButtonPress,
  onActionButtonPress,
  actionBtnLabel,
  actionBtnPreset = "default",
  ...rest
}: WizzardControlsProps) => {
  const { t } = useTranslation("add_polling_station");
  const paddingBottom = useAnimatedBottomPadding(16);

  const AnimatedXStack = Animated.createAnimatedComponent(XStack);

  return (
    <AnimatedXStack
      elevation={2}
      backgroundColor="white"
      alignItems="center"
      gap="$sm"
      padding="$md"
      paddingBottom={paddingBottom}
      {...rest}
    >
      {!isFirstElement && (
        <XStack flex={0.35}>
          <Button
            width="100%"
            height="100%"
            icon={() => <Icon icon="chevronLeft" color="$purple5" />}
            preset="chromeless"
            onPress={onPreviousButtonPress}
            textStyle={{ textAlign: "center" }}
          >
            {t("actions.back")}
          </Button>
        </XStack>
      )}

      <XStack flex={isFirstElement ? 1 : 0.65}>
        <Button
          preset={actionBtnPreset}
          disabled={isNextDisabled}
          width="100%"
          onPress={onActionButtonPress}
          height="100%"
          textStyle={{ textAlign: "center" }}
        >
          {!isLastElement ? actionBtnLabel || t("actions.next_step") : t("actions.finalize")}
        </Button>
      </XStack>
    </AnimatedXStack>
  );
};

export default WizzardControls;
