import React from "react";
import { styled, ToggleGroup, Text, ToggleGroupSingleProps } from "tamagui";
import { Typography } from "../Typography";

const ratings = [1, 2, 3, 4, 5];

interface RatingInputProps extends ToggleGroupSingleProps {
  id: string;
}

const StyledToggleGroupItem = styled(ToggleGroup.Item, {
  name: "Styled Group Item",
  //   backgroundColor: "red",
  borderColor: "$gray3",
  pressStyle: {
    backgroundColor: "transparent",
    borderColor: "$gray3",
  },
  variants: {
    active: {
      true: {
        backgroundColor: "red",
        borderColor: "black",
      },
    },
  },
});

export const RatingInput: React.FC<RatingInputProps> = ({ id, ...rest }) => {
  return (
    <ToggleGroup orientation="horizontal" id={id} height={40} width="100%" {...rest}>
      {ratings.map((rating, i) => (
        <StyledToggleGroupItem key={i} value={rating.toString()} flex={1}>
          <Typography>{rating.toString()}</Typography>
        </StyledToggleGroupItem>
      ))}
    </ToggleGroup>
  );
};
