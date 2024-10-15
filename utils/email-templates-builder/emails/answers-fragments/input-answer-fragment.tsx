import { Section, Text } from "@react-email/components";
import * as React from "react";

interface InputAnswerFragmentProps {
  text: string;
  answer: string;
}

export const InputAnswerFragment = ({
  text = "~$text$~",
  answer = "~$answer$~",
}: InputAnswerFragmentProps) => (
  <Section>
    <Text className="font-semibold">{text}</Text>
    <Text className="m-0 p-[10px] text-[16px] border border-solid rounded-[8px] !border-gray-300">
      {answer}
    </Text>
  </Section>
);

InputAnswerFragment.PreviewProps = {
  text: "Input question",
  answer: "user input",
} as InputAnswerFragmentProps;

export default InputAnswerFragment;
