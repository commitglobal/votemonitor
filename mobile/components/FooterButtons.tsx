import { XStack } from "tamagui";
import { Icon } from "./Icon";
import Button from "./Button";
import { Animated } from "react-native";
import useAnimatedBottomPadding from "../hooks/useAnimatedBottomPadding";

export const FooterButtons = ({
  primaryAction,
  primaryActionLabel,
  isPrimaryButtonDisabled,
  handleGoBack,
}: {
  primaryAction?: () => void;
  primaryActionLabel: string;
  isPrimaryButtonDisabled?: boolean;
  handleGoBack: () => void;
}) => {
  const paddingBottom = useAnimatedBottomPadding(16);

  const AnimatedXStack = Animated.createAnimatedComponent(XStack);

  return (
    <AnimatedXStack
      justifyContent="center"
      alignItems="center"
      paddingRight="$xl"
      paddingTop="$md"
      paddingBottom={paddingBottom}
    >
      <XStack
        justifyContent="center"
        alignItems="center"
        height="100%"
        flex={0.2}
        pressStyle={{ opacity: 0.5 }}
        paddingLeft="$xl"
        onPress={handleGoBack}
      >
        <Icon icon="chevronLeft" size={24} color="$purple5" />
      </XStack>
      <XStack flex={0.8}>
        <Button
          width="100%"
          height="100%"
          textStyle={{ textAlign: "center" }}
          disabled={isPrimaryButtonDisabled || !primaryAction}
          onPress={primaryAction}
        >
          {primaryActionLabel}
        </Button>
      </XStack>
    </AnimatedXStack>
  );
};
