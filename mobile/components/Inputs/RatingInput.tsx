import React, { useMemo } from "react";
import { styled, ToggleGroup, ToggleGroupSingleProps, YStack } from "tamagui";
import { Typography } from "../Typography";

const SCALES = {
  OneTo3: [1, 2, 3],
  OneTo4: [1, 2, 3, 4],
  OneTo5: [1, 2, 3, 4, 5],
  OneTo6: [1, 2, 3, 4, 5, 6],
  OneTo7: [1, 2, 3, 4, 5, 6, 7],
  OneTo8: [1, 2, 3, 4, 5, 6, 7, 8],
  OneTo9: [1, 2, 3, 4, 5, 6, 7, 8, 9],
  OneTo10: [1, 2, 3, 4, 5, 6, 7, 8, 9, 10],
};

export interface RatingInputProps extends ToggleGroupSingleProps {
  id: string;
  scale: string;
  lowerLabel?: string;
  upperLabel?: string;
}

const StyledToggleGroupItem = styled(ToggleGroup.Item, {
  name: "StyledGroupItem",
  borderColor: "$gray3",
  pressStyle: {
    backgroundColor: "transparent",
    borderColor: "$gray3",
  },
  variants: {
    active: {
      true: {
        backgroundColor: "$purple1",
        borderColor: "$purple5",
      },
    },
  },
});

export const RatingInput: React.FC<RatingInputProps> = ({
  id,
  scale,
  value,
  lowerLabel,
  upperLabel,
  onValueChange,
  ...rest
}) => {
  const selectedScale = SCALES[scale as keyof typeof SCALES] || SCALES.OneTo10;
  const fontSize = useMemo(() => (selectedScale === SCALES.OneTo10 ? 12 : 14), [selectedScale]);

  const maxScaleValue = selectedScale.at(-1);
  return (
    <YStack flex={1} gap="$sm">
      <ToggleGroup
        orientation="horizontal"
        id={id}
        height={40}
        width="100%"
        value={value}
        onValueChange={onValueChange}
        {...rest}
      >
        {selectedScale.map((rating, i) => (
          <StyledToggleGroupItem
            key={i}
            value={rating.toString()}
            flex={1}
            active={rating.toString() === value?.toString()}
            padding={0}
            justifyContent="center"
            alignItems="center"
          >
            <Typography fontSize={fontSize} allowFontScaling={false}>
              {rating.toString()}
            </Typography>
          </StyledToggleGroupItem>
        ))}
      </ToggleGroup>
      {lowerLabel && (
        <Typography
          color="$gray5"
          fontSize={fontSize}
          allowFontScaling={false}
          fontStyle={"italic"}
        >
          1 - {lowerLabel}
        </Typography>
      )}
      {upperLabel && (
        <Typography
          color="$gray5"
          fontSize={fontSize}
          allowFontScaling={false}
          fontStyle={"italic"}
        >
          {maxScaleValue} - {upperLabel}
        </Typography>
      )}
    </YStack>
  );
};

export default RatingInput;
