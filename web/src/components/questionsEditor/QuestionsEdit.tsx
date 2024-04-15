import { BaseQuestion } from '@/common/types';
import { DragDropContext, DropResult } from 'react-beautiful-dnd';
import EditQuestionFactory from './edit/EditQuestionFactory';
import { StrictModeDroppable } from './StrictModeDroppable';

export interface QuestionsEditProps {
  languageCode: string;
  localQuestions: BaseQuestion[];
  setLocalQuestions: (questions: BaseQuestion[]) => void;
  activeQuestionId: string | undefined;
  setActiveQuestionId: (questionId: string | undefined) => void;
  invalidQuestions: string[] | null;
  setInvalidQuestions: (questions: string[]) => void;
}

export enum MoveDirection {
  UP = 'UP',
  DOWN = 'DOWN',
}

function QuestionsEdit({
  languageCode,
  localQuestions,
  setLocalQuestions,
  activeQuestionId,
  setActiveQuestionId,
  invalidQuestions,
  setInvalidQuestions,
}: QuestionsEditProps) {
  function addQuestion() {}
  function updateQuestion(questionIndex: number, question: BaseQuestion) {}
  function duplicateQuestion(questionIndex: number) {}
  function deleteQuestion(questionIndex: number) {}

  function onDragEnd(result: DropResult) {
    if (!result.destination) {
      return;
    }
    const newQuestions = Array.from(localQuestions);
    const [reorderedQuestion] = newQuestions.splice(result.source.index, 1);
    newQuestions.splice(result.destination.index, 0, reorderedQuestion!);
    setLocalQuestions([...newQuestions]);
  }

  function moveQuestion(questionIndex: number, direction: MoveDirection) {
    const newQuestions = Array.from(localQuestions);
    const [reorderedQuestion] = newQuestions.splice(questionIndex, 1);
    const destinationIndex = direction === MoveDirection.UP ? questionIndex - 1 : questionIndex + 1;
    newQuestions.splice(destinationIndex, 0, reorderedQuestion!);
    setLocalQuestions([...newQuestions]);
  }

  return (
    <div className='mt-12 px-5 py-4'>
      <DragDropContext onDragEnd={onDragEnd}>
        <div className='mb-5 grid grid-cols-1 gap-5 '>
          <StrictModeDroppable droppableId='questionsList'>
            {(provided) => (
              <div className='grid gap-5' ref={provided.innerRef} {...provided.droppableProps}>
                {localQuestions.map((question, questionIdx) => (
                  <EditQuestionFactory
                    key={question.id}
                    languageCode={languageCode}
                    question={question}
                    questionIdx={questionIdx}
                    moveQuestion={moveQuestion}
                    updateQuestion={updateQuestion}
                    duplicateQuestion={duplicateQuestion}
                    deleteQuestion={deleteQuestion}
                    activeQuestionId={activeQuestionId}
                    setActiveQuestionId={setActiveQuestionId}
                    isLastQuestion={questionIdx === localQuestions.length - 1}
                    isInValid={invalidQuestions ? invalidQuestions.includes(question.id) : false}
                  />
                ))}
                {provided.placeholder}
              </div>
            )}
          </StrictModeDroppable>
        </div>
      </DragDropContext>
      {/* <AddQuestionButton addQuestion={addQuestion} /> */}
    </div>
  );
}
export default QuestionsEdit;
