import React, { ReactNode } from "react";
import {
  styled,
  Input as TamaguiInput,
  InputProps as TamaguiInputProps,
  TextArea as TamaguiTextArea,
  XStack,
} from "tamagui";

export interface InputProps extends TamaguiInputProps {
  type: "text" | "numeric" | "textarea" | "password" | "email-address";
  iconRight?: ReactNode;
  onIconRightPress?: () => void;
}

const StyledTextArea = styled(TamaguiTextArea, {
  backgroundColor: "white",
  minHeight: 98,
  maxHeight: 150,
  paddingVertical: "$xs",
  paddingHorizontal: 14,
  textAlignVertical: "top",
  fontSize: 16,
  lineHeight: 24,
  borderColor: "$gray3",
  fontWeight: "400",
  focusStyle: {
    borderColor: "$purple5",
  },
});

const Input: React.FC<InputProps> = ({
  type,
  value,
  iconRight,
  borderColor,
  onIconRightPress,
  ...rest
}) => {
  return (
    <>
      {type === "textarea" ? (
        <StyledTextArea value={value} borderColor={borderColor || "$gray3"} {...rest} />
      ) : (
        <InputWrapper borderColor={borderColor || "$gray3"}>
          <SearchInput
            value={value}
            secureTextEntry={type === "password"}
            keyboardType={type === "numeric" || type === "email-address" ? type : "default"}
            // fix ios keyboard flicker bug
            textContentType={"oneTimeCode"}
            {...rest}
          />
          {iconRight && <IconWrapper onPress={onIconRightPress}>{iconRight}</IconWrapper>}
        </InputWrapper>
      )}
    </>
  );
};

const InputWrapper = styled(XStack, {
  backgroundColor: "white",
  width: "100%",
  height: 42,
  alignItems: "center",
  justifyContent: "space-between",
  borderWidth: 1,
  // borderColor: "$gray3",
  borderRadius: 8,
  focusStyle: {
    borderColor: "$purple5",
  },
});

const SearchInput = styled(TamaguiInput, {
  backgroundColor: "transparent",
  flex: 9,
  fontSize: 16,
  padding: 0,
  paddingLeft: 14,
  borderWidth: 0,
  borderRadius: 0,
  focusStyle: {
    borderColor: "transparent",
  },
});

const IconWrapper = styled(XStack, {
  flex: 1,
  paddingVertical: 11,
  justifyContent: "flex-end",
  paddingRight: 14,
});

export default Input;
