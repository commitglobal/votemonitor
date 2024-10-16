import { Text } from "@react-email/components";
import * as React from "react";

interface NoteFragmentProps {
  number: string;
  text: string;
}

export const NoteFragment = ({
  number = "~$number$~",
  text = "~$text$~",
}: NoteFragmentProps) => (
  <>
    <Text><strong>{number}</strong> {text}</Text>
  </>
);

NoteFragment.PreviewProps = {
  number: "1",
  text: "Note",
} as NoteFragmentProps;

export default NoteFragment;
