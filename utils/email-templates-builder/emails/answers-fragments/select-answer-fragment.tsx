import { Section, Tailwind, Text } from "@react-email/components";
import * as React from "react";
import { SelectAnswerOptionFragment } from "./select-answer-option-fragment";
import { SelectAnswerOptionSelectedFragment } from "./select-answer-option-selected-fragment";
import AnswerNotesFragment from "./answer-notes-fragment";
import AnswerAttachmentsFragment from "./answer-attachments-fragment";

interface SelectAnswerFragmentProps {
  text: string;
  options: React.ReactNode[];
  notes: string[];
  attachments: string[];
}

export const SelectAnswerFragment = ({
  text = "~$text$~",
  options = [],
  notes = [],
  attachments = [],
}: SelectAnswerFragmentProps) => (
  <Tailwind>
    <Section>
      <Text className="font-semibold">{text}</Text>
      <Section>
        <table>
          {options.length > 0 ? options.map((option) => option) : "~$options$~"}
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
      </Section>
    </Section>
  </Tailwind>
);

SelectAnswerFragment.PreviewProps = {
  text: "This is a select question",
  options: [
    <SelectAnswerOptionFragment value={"option 1"} key={1} />,
    <SelectAnswerOptionSelectedFragment value={"option 2"} key={2} />,
  ],
  notes: ["Note1", "Note 2"],
  attachments: [
    "https://fastly.picsum.photos/id/1/5000/3333.jpg?hmac=Asv2DU3rA_5D1xSe22xZK47WEAN0wjWeFOhzd13ujW4",
    "https://fastly.picsum.photos/id/0/5000/3333.jpg?hmac=_j6ghY5fCfSD6tvtcV74zXivkJSPIfR9B8w34XeQmvU",
  ],
} as SelectAnswerFragmentProps;

export default SelectAnswerFragment;
