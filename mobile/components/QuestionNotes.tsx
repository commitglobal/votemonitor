import React, { useState } from "react";
import { YStack } from "tamagui";
import { Typography } from "./Typography";
import Card from "./Card";
import { Icon } from "./Icon";
import { Note } from "../common/models/note";
import EditNoteSheet from "./EditNoteSheet";
import { Platform } from "react-native";

const QuestionNotes = ({
  notes,
  electionRoundId,
  pollingStationId,
  formId,
  questionId,
}: {
  notes: Note[];
  electionRoundId: string;
  pollingStationId: string;
  formId: string;
  questionId: string;
}) => {
  const [selectedNote, setSelectedNote] = useState<Note | null>(null);

  return (
    <YStack marginTop="$lg" gap="$xxs">
      <Typography fontWeight="500">Notes</Typography>
      {notes.map((note) => {
        return (
          <Card
            key={note.id}
            flexDirection="row"
            justifyContent="space-between"
            padding="$0"
            paddingLeft="$md"
            pressStyle={{ opacity: 1 }}
          >
            <Typography paddingVertical="$md" maxWidth="85%" numberOfLines={5}>
              {note.text}
            </Typography>
            <YStack
              onPress={() => {
                setSelectedNote(note);
              }}
              pressStyle={{ opacity: 0.5 }}
              padding="$md"
            >
              <Icon icon="pencilAlt" size={24} />
            </YStack>
          </Card>
        );
      })}
      {/* this weird condition is a workaround for fixing bottomsheet jump on ios/ sheet not opening on android on back button press */}
      {(selectedNote || Platform.OS === "ios") && (
        <EditNoteSheet
          selectedNote={selectedNote}
          setSelectedNote={setSelectedNote}
          questionId={questionId}
          electionRoundId={electionRoundId}
          pollingStationId={pollingStationId}
          formId={formId as string}
        />
      )}
    </YStack>
  );
};

export default QuestionNotes;
