import { XStack } from "tamagui";
import Button from "./Button";
import { Icon } from "./Icon";
import { useTranslation } from "react-i18next";
import { useSafeAreaInsets } from "react-native-safe-area-context";

interface WizzardControlsProps {
  isFirstElement?: boolean;
  isLastElement?: boolean;
  isNextDisabled?: boolean;
  onPreviousButtonPress: () => void;
  onNextButtonPress: () => void;
}

const WizzardControls = ({
  isFirstElement,
  isLastElement,
  isNextDisabled,
  onPreviousButtonPress,
  onNextButtonPress,
}: WizzardControlsProps) => {
  const { t } = useTranslation("add_polling_station");
  const insets = useSafeAreaInsets();

  return (
    <XStack
      elevation={2}
      backgroundColor="white"
      gap="$sm"
      padding="$md"
      paddingBottom={insets.bottom}
    >
      {isFirstElement && (
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
      <XStack flex={!isFirstElement ? 1 : 0.75} marginBottom="$md">
        <Button disabled={isNextDisabled} width="100%" onPress={onNextButtonPress}>
          {!isLastElement ? t("actions.next_step") : t("actions.finalize")}
        </Button>
      </XStack>
    </XStack>
  );
};

export default WizzardControls;
