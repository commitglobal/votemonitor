import { Column, Row, Section, Text } from "@react-email/components";
import * as React from "react";
import { RatingAnswerOptionFragment } from "./rating-answer-option-fragment";
import { RatingAnswerOptionSelectedFragment } from "./rating-answer-option-selected-fragment";

interface RatingAnswerFragmentProps {
  text: string;
  options: React.ReactNode[];
}

export const RatingAnswerFragment = ({
  text = "~$text$~",
  options = [],
}: RatingAnswerFragmentProps) => (
  <Section>
    <Text className="font-semibold">{text}</Text>
    <Row>
      <Column align="left">
        <table>
          <tr>
            {options.length > 0
              ? options.map((option) => option)
              : "~$options$~"}
          </tr>
        </table>
      </Column>
    </Row>
  </Section>
);

RatingAnswerFragment.PreviewProps = {
  text: "This is a rating question",
  options: [
    <RatingAnswerOptionFragment value="1"  key={1}/>,
    <RatingAnswerOptionSelectedFragment value="2" key={2} />,
  ],
} as RatingAnswerFragmentProps;

export default RatingAnswerFragment;
