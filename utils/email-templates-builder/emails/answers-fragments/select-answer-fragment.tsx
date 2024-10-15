import { Section, Text } from "@react-email/components";
import * as React from "react";
import { SelectAnswerOptionFragment } from "./select-answer-option-fragment";
import { SelectAnswerOptionSelectedFragment } from "./select-answer-option-selected-fragment";

interface SelectAnswerFragmentProps {
  text: string;
  options: React.ReactNode[];
}

export const SelectAnswerFragment = ({
  text = "~$text$~",
  options = [],
}: SelectAnswerFragmentProps) => (
  <Section>
    <Text className="font-semibold">{text}</Text>
    <Section>
      <table>
        {options.length > 0 ? options.map((option) => option) : "~$options$~"}
      </table>
    </Section>
  </Section>
);

SelectAnswerFragment.PreviewProps = {
  text: "This is a select question",
  options: [
    <SelectAnswerOptionFragment value={"option 1"} key={1} />,
    <SelectAnswerOptionSelectedFragment value={"option 2"} key={2} />,
  ],
} as SelectAnswerFragmentProps;

export default SelectAnswerFragment;
