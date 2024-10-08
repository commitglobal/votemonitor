import React from "react";
import { ScrollView, styled, useWindowDimensions, YStack } from "tamagui";
import { Icon } from "./Icon";
import { Typography } from "./Typography";

type OnboardingItemProps = {
  icon: string;
  title: string;
  helper: string;
};

const OnboardingItem = ({ icon, title, helper, ...rest }: OnboardingItemProps) => {
  const { height } = useWindowDimensions();
  return (
    <Page {...rest}>
      <YStack gap="$xl" alignItems="center">
        <Icon icon={icon} />
        <YStack height={height / 3}>
          <ScrollView bounces={false} showsVerticalScrollIndicator={false}>
            <YStack gap="$md">
              <Typography preset="heading" fontWeight="500" textAlign="center" color="white">
                {title}
              </Typography>
              <Typography
                fontSize={18}
                lineHeight={24}
                textAlign="center"
                color="white"
                opacity={0.7}
              >
                {helper}
              </Typography>
            </YStack>
          </ScrollView>
        </YStack>
      </YStack>
    </Page>
  );
};

export default OnboardingItem;

const Page = styled(YStack, {
  collapsable: false,
  height: "100%",
  backgroundColor: "$purple6",
  padding: "$xl",
  justifyContent: "center",
  alignItems: "center",
});
