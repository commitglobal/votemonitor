import React from "react";
import { useSafeAreaInsets } from "react-native-safe-area-context";
import { YStack } from "tamagui";
import { Icon } from "./Icon";
import { Typography } from "./Typography";
import Select from "./Select";
import Button from "./Button";

const ChooseOnboardingLanguage = ({
  setLanguageSelectionApplied,
}: {
  setLanguageSelectionApplied: React.Dispatch<React.SetStateAction<boolean>>;
}) => {
  const insets = useSafeAreaInsets();
  return (
    <YStack
      key="1"
      collapsable={false}
      height="100%"
      backgroundColor="$purple6"
      padding="$xl"
      paddingBottom={insets.bottom + 32}
      justifyContent="space-between"
      alignItems="center"
    >
      <YStack gap="$xl" alignItems="center" marginTop={100}>
        <Icon icon="onboardingLanguage" />

        <Typography preset="heading" fontWeight="500" textAlign="center" color="white">
          Choose your language
        </Typography>
        {/* //todo: lanuage options */}
        {/* //todo: controller */}
        <Select
          options={[
            { value: "EN", id: "1", label: "English" },
            { value: "RO", id: "2", label: "Romanian" },
          ]}
          // todo: use the right value when right options
          defaultValue="EN"
        />
        <Typography fontSize={18} lineHeight={24} textAlign="center" color="white" opacity={0.7}>
          This will be the language in which your app will be displayed, and can be changed at any
          time from the More Section of the application.
        </Typography>
      </YStack>
      <Button
        backgroundColor="$yellow6"
        paddingHorizontal="$xxxl"
        textStyle={{ color: "#5F288D", fontSize: 16, fontWeight: "500" }}
        justifyContent="center"
        alignItems="center"
        // marginBottom="$xxl"
        onPress={() => setLanguageSelectionApplied(true)}
      >
        Apply selection
      </Button>
    </YStack>
  );
};

export default ChooseOnboardingLanguage;
