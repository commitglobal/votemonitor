import React, { ReactNode } from "react";
import { YStack } from "tamagui";

const GeneralCard = ({ children, ...props }: { children: ReactNode }) => {
  return (
    <YStack
      {...props}
      borderRadius="$1"
      backgroundColor="white"
      shadowColor="gray"
      shadowOpacity={0.1}
      shadowRadius={10}
      shadowOffset={{ width: 0, height: 10 }}
      padding="$4"
      elevation={0.5}
    >
      {children}
    </YStack>
  );
};

export default GeneralCard;
