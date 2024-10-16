import { useSafeAreaInsets } from "react-native-safe-area-context";
import { XStack } from "tamagui";
import { Icon } from "./Icon";
import Button from "./Button";

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
  const insets = useSafeAreaInsets();

  return (
    <XStack
      marginBottom={insets.bottom + 16}
      justifyContent="center"
      alignItems="center"
      paddingRight="$xl"
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
      <Button
        flex={0.8}
        disabled={isPrimaryButtonDisabled || !primaryAction}
        onPress={primaryAction}
      >
        {primaryActionLabel}
      </Button>
    </XStack>
  );
};
