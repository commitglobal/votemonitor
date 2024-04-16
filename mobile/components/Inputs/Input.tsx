import React from "react";
import {
  styled,
  Input as TamaguiInput,
  InputProps as TamaguiInputProps,
  TextArea as TamaguiTextArea,
} from "tamagui";

export interface InputProps extends TamaguiInputProps {
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
  borderColor: "$gray3",
  textAlignVertical: "top",
  fontSize: 16,
  lineHeight: 24,
  fontWeight: "400",
  focusStyle: {
    borderColor: "$purple5",
  },
});

const Input: React.FC<InputProps> = ({ type, value, ...rest }) => {
  return (
    <>
      {type === "text" ? (
        <>
          <StyledTextArea value={value} {...rest} />
        </>
      ) : (
        <StyledInput value={value} keyboardType="numeric" fontSize={16} {...rest} />
      )}
    </>
  );
};

export default Input;
