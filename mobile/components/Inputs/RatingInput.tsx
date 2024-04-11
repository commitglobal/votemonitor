import React from "react";
import { styled, ToggleGroup, ToggleGroupSingleProps } from "tamagui";
import { Typography } from "../Typography";
import { Text } from "react-native";

const ratings = [1, 2, 3, 4, 5];

export interface RatingInputProps extends ToggleGroupSingleProps {
  id: string;
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

export const RatingInput: React.FC<RatingInputProps> = ({ id, value, onValueChange, ...rest }) => {
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
      {ratings.map((rating, i) => (
        <StyledToggleGroupItem
          key={i}
          value={rating.toString()}
          flex={1}
          active={rating.toString() === value}
        >
          <Text>{rating.toString()}</Text>
          {/*//TODO: removed typography because it was causing a warning, should we add it back? */}
          {/* <Typography>{rating.toString()}</Typography> */}
        </StyledToggleGroupItem>
      ))}
    </ToggleGroup>
  );
};

export default RatingInput;
