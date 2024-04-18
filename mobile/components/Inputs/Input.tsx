import React, { ReactNode } from "react";
import {
  styled,
  Input as TamaguiInput,
  InputProps as TamaguiInputProps,
  TextArea as TamaguiTextArea,
  XStack,
} from "tamagui";

export interface InputProps extends TamaguiInputProps {
  type: "text" | "numeric" | "textarea" | "password";
  iconRight?: ReactNode;
}

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

const Input: React.FC<InputProps> = ({ type, value, iconRight, ...rest }) => {
  return (
    <>
      {type === "textarea" ? (
        <StyledTextArea value={value} {...rest} />
      ) : (
        <InputWrapper>
          <SearchInput
            value={value}
            secureTextEntry={type === "password"}
            keyboardType={type === "numeric" ? type : "default"}
            {...rest}
          />
          {iconRight}
        </InputWrapper>
      )}
    </>
  );
};

const SearchInput = styled(TamaguiInput, {
  backgroundColor: "transparent",
  flex: 1,
  fontSize: 16,
  padding: 0,
  borderWidth: 0,
  borderRadius: 0,
  focusStyle: {
    borderColor: "transparent",
  },
});

const InputWrapper = styled(XStack, {
  backgroundColor: "white",
  width: "100%",
  height: 42,
  alignItems: "center",
  justifyContent: "space-between",
  paddingHorizontal: 14,
  borderWidth: 1,
  borderColor: "$gray3",
  borderRadius: 8,
  focusStyle: {
    borderColor: "$purple5",
  },
});

export default Input;
