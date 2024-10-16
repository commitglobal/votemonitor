import { Column, Row, Section, Tailwind, Text } from "@react-email/components";
import * as React from "react";
import { RatingAnswerOptionFragment } from "./rating-answer-option-fragment";
import { RatingAnswerOptionSelectedFragment } from "./rating-answer-option-selected-fragment";
import AnswerNotesFragment from "./answer-notes-fragment";
import AnswerAttachmentsFragment from "./answer-attachments-fragment";

interface RatingAnswerFragmentProps {
  text: string;
  options: React.ReactNode[];
  notes: string[];
  attachments: string[];
}

export const RatingAnswerFragment = ({
  text = "~$text$~",
  options = [],
  notes = [],
  attachments = []
}: RatingAnswerFragmentProps) => (
  <Tailwind>
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
          {notes.length > 0 ? (
            <AnswerNotesFragment notes={notes} listTitle="Notes" />
          ) : (
            "~$notes$~"
          )}

          {attachments.length > 0 ? (
            <AnswerAttachmentsFragment
              attachments={attachments}
              listTitle="Attachments"
            />
          ) : (
            "~$attachments$~"
          )}
        </Column>
      </Row>
    </Section>
  </Tailwind>
);

RatingAnswerFragment.PreviewProps = {
  text: "This is a rating question",
  options: [
    <RatingAnswerOptionFragment value="1" key={1} />,
    <RatingAnswerOptionSelectedFragment value="2" key={2} />,
  ],
  notes: ["Note1", "Note 2"],
  attachments: [
    "https://fastly.picsum.photos/id/1/5000/3333.jpg?hmac=Asv2DU3rA_5D1xSe22xZK47WEAN0wjWeFOhzd13ujW4",
    "https://fastly.picsum.photos/id/0/5000/3333.jpg?hmac=_j6ghY5fCfSD6tvtcV74zXivkJSPIfR9B8w34XeQmvU",
  ],
} as RatingAnswerFragmentProps;

export default RatingAnswerFragment;
