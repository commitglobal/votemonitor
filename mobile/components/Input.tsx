import React from "react";
import {
  styled,
  Input as TamaguiInput,
  InputProps as TamaguiInputProps,
} from "tamagui";

interface InputProps extends TamaguiInputProps {
  // TODO: do we want to give it a placeholder?
  placeholder?: string;
}

const StyledInput = styled(TamaguiInput, {
  backgroundColor: "white",
  borderColor: "$gray3",
  height: 42,
  width: "100%",
  focusStyle: {
    borderColor: "$purple5",
  },
});

const Input: React.FC<InputProps> = ({ placeholder, ...rest }) => {
  return <StyledInput {...rest} />;
};

export default Input;
