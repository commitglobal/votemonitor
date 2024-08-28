import React from "react";
import { YStack } from "tamagui";
import { MotiView } from "moti";
import { Skeleton } from "moti/skeleton";

const formsSkeleton = [1, 2];

const NotesSkeleton = () => {
  return (
    <>
      <MotiView
        transition={{
          translateX: {
            type: "spring",
          },
        }}
        style={{ flexDirection: "column", gap: 8 }}
      >
        <YStack marginTop="$md" gap="$md">
          <Skeleton colorMode="light" width={"25%"} height={16} />
          {formsSkeleton.map((el) => (
            <Skeleton key={el} colorMode="light" width={"100%"} height={50} radius={3} />
          ))}
        </YStack>
      </MotiView>
    </>
  );
};

export default NotesSkeleton;
