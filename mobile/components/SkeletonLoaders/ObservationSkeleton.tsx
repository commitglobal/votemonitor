import React from "react";
import { ScrollView, YStack } from "tamagui";
import { MotiView } from "moti";
import { Skeleton } from "moti/skeleton";

const formsSkeleton = [1, 2, 3, 4];

const ObservationSkeleton = () => {
  return (
    <ScrollView flex={1} showsVerticalScrollIndicator={false}>
      <MotiView
        transition={{
          translateX: {
            type: "spring",
          },
        }}
        style={{ flexDirection: "column", gap: 8 }}
      >
        <Skeleton colorMode="light" width={"100%"} height={100} radius={3} />
        <Skeleton colorMode="light" width={"100%"} height={100} radius={3} />

        <YStack marginTop="$md" gap="$md">
          <Skeleton colorMode="light" width={"25%"} height={16} />
          {formsSkeleton.map((el) => (
            <Skeleton key={el} colorMode="light" width={"100%"} height={100} radius={3} />
          ))}
        </YStack>
      </MotiView>
    </ScrollView>
  );
};

export default ObservationSkeleton;
