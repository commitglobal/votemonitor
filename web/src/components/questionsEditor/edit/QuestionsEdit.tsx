import { DragDropContext, DropResult } from 'react-beautiful-dnd';
import { v4 as uuidv4 } from 'uuid';

import { EditFormType } from '@/components/FormEditor/FormEditor';
import { useFieldArray, useFormContext, useWatch } from 'react-hook-form';
import { StrictModeDroppable } from '../StrictModeDroppable';
import AddQuestionButton from './AddQuestionButton';
import EditQuestionFactory from './EditQuestionFactory';
import { EditQuestionType } from '@/common/form-requests';

export enum MoveDirection {
  UP = 'UP',
  DOWN = 'DOWN',
}

export interface QuestionsEditProps {
  activeQuestionId: string | undefined;
  setActiveQuestionId: (questionId: string | undefined) => void;
}

function QuestionsEdit({ activeQuestionId, setActiveQuestionId }: QuestionsEditProps) {
  const { control, trigger } = useFormContext<EditFormType>();

  const { fields, append, swap, remove, insert } = useFieldArray({
    name: 'questions',
    control: control,
  });

  const languageCode = useWatch({
    control,
    name: `languageCode`,
  });

  const availableLanguages = useWatch({
    control,
    name: `languages`,
  });

  function addQuestion(question: EditQuestionType) {
    append(question);
    setActiveQuestionId(question.questionId);
    trigger('questions');
  }

  function duplicateQuestion(questionIndex: number) {
    const newQuestion = { ...fields[questionIndex]!, questionId: uuidv4() };
    insert(questionIndex + 1, newQuestion);
  }

  function deleteQuestion(questionIndex: number) {
    remove(questionIndex);
  }

  function onDragEnd(result: DropResult) {
    if (!result.destination) {
      return;
    }

    swap(result.source.index, result.destination.index);
  }

  function moveQuestion(questionIndex: number, direction: MoveDirection) {
    swap(questionIndex, direction === MoveDirection.UP ? questionIndex - 1 : questionIndex + 1);
  }

  return (
    <div>
      <DragDropContext onDragEnd={onDragEnd}>
        <div className='grid grid-cols-1 gap-5 mb-5'>
          <StrictModeDroppable droppableId='questionsList'>
            {(provided) => (
              <div className='grid gap-5' ref={provided.innerRef} {...provided.droppableProps}>
                {fields.map((field, questionIndex) => (
                  <EditQuestionFactory
                    key={field.id}
                    questionIndex={questionIndex}
                    moveQuestion={moveQuestion}
                    duplicateQuestion={duplicateQuestion}
                    deleteQuestion={deleteQuestion}
                    activeQuestionId={activeQuestionId}
                    setActiveQuestionId={setActiveQuestionId}
                    isLastQuestion={questionIndex === fields.length - 1}
                  />
                ))}
                {provided.placeholder}
              </div>
            )}
          </StrictModeDroppable>
        </div>
      </DragDropContext>
      <AddQuestionButton
        languageCode={languageCode}
        availableLanguages={availableLanguages}
        addQuestion={addQuestion}
      />
    </div>
  );
}
export default QuestionsEdit;
