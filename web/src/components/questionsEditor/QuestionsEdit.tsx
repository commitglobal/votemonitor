import { BaseQuestion } from "@/common/types"
import { DragDropContext, DropResult } from "react-beautiful-dnd";
import EditQuestionFactory from "./edit/EditQuestionFactory";
import { StrictModeDroppable } from "./StrictModeDroppable";
import { validateQuestion } from "./edit/Validation";
import AddQuestionButton from "./edit/AddQuestionButton";
import { v4 as uuidv4 } from 'uuid';

export interface QuestionsEditProps {
  availableLanguages: string[];
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
  availableLanguages,
  languageCode,
  localQuestions,
  setLocalQuestions,
  activeQuestionId,
  setActiveQuestionId,
  invalidQuestions,
  setInvalidQuestions }: QuestionsEditProps) {

  function handleValidation(question: BaseQuestion) {
    if (invalidQuestions === null) {
      return;
    }

    let temp: string[] = [...invalidQuestions]

    if (validateQuestion(question, languageCode)) {
      temp = invalidQuestions.filter((id) => id !== question.id);
      setInvalidQuestions(temp);
    } else if (!invalidQuestions.includes(question.id)) {
      temp.push(question.id);
      setInvalidQuestions(temp);
    }
  };

  function addQuestion(question: BaseQuestion) {
    localQuestions.push(question);
    const updatedQuestions = Array.from(localQuestions);
    setLocalQuestions(updatedQuestions);
    setActiveQuestionId(question.id);
  }

  function updateQuestion(questionIndex: number, question: BaseQuestion) {
    localQuestions[questionIndex] = { ...question };
    const updatedQuestions = Array.from(localQuestions);

    setLocalQuestions(updatedQuestions);
    handleValidation(question);
    setActiveQuestionId(question.id);
  }

  function duplicateQuestion(questionIndex: number) {
    const question = localQuestions[questionIndex]; // Get the element to duplicate
    localQuestions.splice(questionIndex, 0, {...question!, id: uuidv4()}); // Insert a copy of the element at the specified index
    const updatedQuestions = Array.from(localQuestions);

    setLocalQuestions(updatedQuestions);
  }

  function deleteQuestion(questionIndex: number) {
    const newQuestionsArray = Array.from(localQuestions);
    newQuestionsArray.splice(questionIndex, 1); // Deletes one element at the specified index

    setLocalQuestions(newQuestionsArray);
  }

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
    <div>
      <DragDropContext onDragEnd={onDragEnd}>
        <div className='mb-5 grid grid-cols-1 gap-5 '>
          <StrictModeDroppable droppableId='questionsList'>
            {(provided) => (
              <div className='grid gap-5' ref={provided.innerRef} {...provided.droppableProps}>
                {localQuestions.map((question, questionIdx) => (
                  <EditQuestionFactory
                    key={question.id}
                    availableLanguages={availableLanguages}
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
      <AddQuestionButton availableLanguages={availableLanguages} languageCode={languageCode} addQuestion={addQuestion} />
    </div>
  );
}
export default QuestionsEdit;
