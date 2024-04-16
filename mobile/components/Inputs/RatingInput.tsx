import React from "react";
import { styled, ToggleGroup, ToggleGroupSingleProps } from "tamagui";
// import { Typography } from "../Typography";
import { Text } from "react-native";

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
  onValueChange,
  ...rest
}) => {
  const selectedScale = SCALES[scale as keyof typeof SCALES] || SCALES.OneTo10;

  return (
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
          active={rating.toString() === value}
        >
          <Text>{rating.toString()}</Text>
          {/* //TODO: removed typography because it was causing a warning, should we add it back? */}
          {/* <Typography>{rating.toString()}</Typography> */}
        </StyledToggleGroupItem>
      ))}
    </ToggleGroup>
  );
};

export default RatingInput;
