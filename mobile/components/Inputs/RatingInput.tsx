import React, { useState } from "react";
import { styled, ToggleGroup, ToggleGroupSingleProps } from "tamagui";
import { Typography } from "../Typography";

const ratings = [1, 2, 3, 4, 5];

interface RatingInputProps extends ToggleGroupSingleProps {
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

export const RatingInput: React.FC<RatingInputProps> = ({ id, defaultValue, ...rest }) => {
  // TODO: styling without state?
  const [selected, setSelected] = useState(defaultValue);

  console.log(selected);
  return (
    <ToggleGroup
      onValueChange={(val: any) => setSelected(val)}
      orientation="horizontal"
      id={id}
      height={40}
      width="100%"
      {...rest}
    >
      {ratings.map((rating, i) => (
        <StyledToggleGroupItem
          key={i}
          value={rating.toString()}
          flex={1}
          active={rating.toString() === selected}
        >
          <Typography>{rating.toString()}</Typography>
        </StyledToggleGroupItem>
      ))}
    </ToggleGroup>
  );
};
