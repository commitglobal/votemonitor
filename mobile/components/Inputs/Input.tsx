import React from "react";
import {
  styled,
  Input as TamaguiInput,
  InputProps as TamaguiInputProps,
  TextArea as TamaguiTextArea,
} from "tamagui";

interface InputProps extends TamaguiInputProps {
  type: "text" | "numeric";
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

const StyledTextArea = styled(TamaguiTextArea, {
  backgroundColor: "white",
  minHeight: 98,
  paddingVertical: "$xs",
  paddingHorizontal: 14,
  fontSize: 16,
  lineHeight: 24,
  fontWeight: "400",
  focusStyle: {
    borderColor: "$purple5",
  },
});

const Input: React.FC<InputProps> = ({ type, ...rest }) => {
  return (
    <>{type === "text" ? <StyledTextArea /> : <StyledInput {...rest} keyboardType="numeric" />}</>
  );
};

export default Input;
