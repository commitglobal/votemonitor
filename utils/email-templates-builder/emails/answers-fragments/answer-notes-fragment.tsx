import { Section, Tailwind, Text } from "@react-email/components";
import * as React from "react";
import { NoteFragment } from "./note-fragment";

interface AnswerNotesFragmentProps {
  listTitle: string;
  notes: string[];
}

export const AnswerNotesFragment = ({
  listTitle = "~$listTitle$~",
  notes = [],
}: AnswerNotesFragmentProps) => (
  <Tailwind>
    <Section>
      <Text className="font-semibold">{listTitle}</Text>
      <Section className="m-0 p-[10px] text-[16px] border border-solid rounded-[8px] !border-gray-300">
        {notes.length > 0
          ? notes.map((note, idx) => (
              <NoteFragment
                number={(idx + 1).toString()}
                text={note}
                key={idx}
              />
            ))
          : "~$notes$~"}
      </Section>
    </Section>
  </Tailwind>
);

AnswerNotesFragment.PreviewProps = {
  listTitle: "Notes:",
  notes: ["note 1", "note 2"],
} as AnswerNotesFragmentProps;

export default AnswerNotesFragment;
