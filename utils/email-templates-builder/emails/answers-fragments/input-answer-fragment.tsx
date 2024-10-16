import { Section, Tailwind, Text } from "@react-email/components";
import { AnswerNotesFragment } from "./answer-notes-fragment";
import * as React from "react";
import AnswerAttachmentsFragment from "./answer-attachments-fragment";

interface InputAnswerFragmentProps {
  text: string;
  answer: string;
  notes: string[];
  attachments: string[];
}

export const InputAnswerFragment = ({
  text = "~$text$~",
  answer = "~$answer$~",
  notes = [],
  attachments = [],
}: InputAnswerFragmentProps) => (
  <Tailwind>
    <Section>
      <Text className="font-semibold">{text}</Text>
      <Text className="m-0 p-[10px] text-[16px] border border-solid rounded-[8px] !border-gray-300">
        {answer}
      </Text>
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
  </Tailwind>
);

InputAnswerFragment.PreviewProps = {
  text: "Input question",
  answer: "user input",
  notes: ["Note1", "Note 2"],
  attachments: [
    "https://fastly.picsum.photos/id/1/5000/3333.jpg?hmac=Asv2DU3rA_5D1xSe22xZK47WEAN0wjWeFOhzd13ujW4",
    "https://fastly.picsum.photos/id/0/5000/3333.jpg?hmac=_j6ghY5fCfSD6tvtcV74zXivkJSPIfR9B8w34XeQmvU",
  ],
} as InputAnswerFragmentProps;

export default InputAnswerFragment;
