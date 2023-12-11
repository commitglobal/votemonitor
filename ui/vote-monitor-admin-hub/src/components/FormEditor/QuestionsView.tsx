
import { useMemo, useState } from "react";
import { DragDropContext } from "react-beautiful-dnd";
import { toast } from "react-toastify";
import { v4 as uuidv4 } from 'uuid';

import AddQuestionButton from "./AddQuestionButton";
import QuestionCard from "./QuestionCard";
import { StrictModeDroppable } from "./StrictModeDroppable";
import { validateQuestion } from "./Validation";
import { FormModel, TFormQuestion } from "@/redux/api/types";

interface QuestionsViewProps {
  localForm: FormModel;
  setLocalForm: (form: FormModel) => void;
  activeQuestionId: string | null;
  setActiveQuestionId: (questionId: string | null) => void;
  invalidQuestions: string[] | null;
  setInvalidQuestions: (invalidQuestions: string[] | null) => void;
}

export default function QuestionsView({
  activeQuestionId,
  setActiveQuestionId,
  localForm,
  setLocalForm,
  invalidQuestions,
  setInvalidQuestions,
}: QuestionsViewProps) {
  const internalQuestionIdMap = useMemo(() => {
    return localForm.questions.reduce((acc, question) => {
      acc[question.id] = uuidv4();
      return acc;
    }, {});
  }, []);


  const handleQuestionLogicChange = (form: FormModel, compareId: string, updatedId: string): FormModel => {
    form.questions.forEach((question) => {
      if (!question.logic) return;
      question.logic.forEach((rule) => {
        if (rule.destination === compareId) {
          rule.destination = updatedId;
        }
      });
    });
    return form;
  };

  // function to validate individual questions
  const validateForm = (question: TFormQuestion) => {
    // prevent this function to execute further if user hasnt still tried to save the form
    if (invalidQuestions === null) {
      return;
    }
    let temp = JSON.parse(JSON.stringify(invalidQuestions));
    if (validateQuestion(question)) {
      temp = invalidQuestions.filter((id) => id !== question.id);
      setInvalidQuestions(temp);
    } else if (!invalidQuestions.includes(question.id)) {
      temp.push(question.id);
      setInvalidQuestions(temp);
    }
  };

  const updateQuestion = (questionIdx: number, updatedAttributes: any) => {
    let updatedForm = { ...localForm };

    if ("id" in updatedAttributes) {
      // if the form whose id is to be changed is linked to logic of any other form then changing it
      const initialQuestionId = updatedForm.questions[questionIdx].id;
      updatedForm = handleQuestionLogicChange(updatedForm, initialQuestionId, updatedAttributes.id);
      if (invalidQuestions?.includes(initialQuestionId)) {
        setInvalidQuestions(
          invalidQuestions.map((id) => (id === initialQuestionId ? updatedAttributes.id : id))
        );
      }

      // relink the question to internal Id
      internalQuestionIdMap[updatedAttributes.id] =
        internalQuestionIdMap[localForm.questions[questionIdx].id];
      delete internalQuestionIdMap[localForm.questions[questionIdx].id];
      setActiveQuestionId(updatedAttributes.id);
    }

    updatedForm.questions[questionIdx] = {
      ...updatedForm.questions[questionIdx],
      ...updatedAttributes,
    };

    setLocalForm(updatedForm);
    validateForm(updatedForm.questions[questionIdx]);
  };

  const deleteQuestion = (questionIdx: number) => {
    const questionId = localForm.questions[questionIdx].id;
    let updatedForm: FormModel = { ...localForm };
    updatedForm.questions.splice(questionIdx, 1);

    updatedForm = handleQuestionLogicChange(updatedForm, questionId, "end");

    setLocalForm(updatedForm);
    delete internalQuestionIdMap[questionId];

    if (questionId === activeQuestionId) {
      if (questionIdx < localForm.questions.length - 1) {
        setActiveQuestionId(localForm.questions[questionIdx + 1].id);
      } else if (localForm.questions.length !== 0) {
        setActiveQuestionId(localForm.questions[questionIdx - 1].id);
      } else {
        setActiveQuestionId(null);
      }
    }
    toast.success("Question deleted.");
  };

  const duplicateQuestion = (questionIdx: number) => {
    const questionToDuplicate = JSON.parse(JSON.stringify(localForm.questions[questionIdx]));

    const newQuestionId = uuidv4();

    // create a copy of the question with a new id
    const duplicatedQuestion = {
      ...questionToDuplicate,
      id: newQuestionId,
    };

    // insert the new question right after the original one
    const updatedForm = { ...localForm };
    updatedForm.questions.splice(questionIdx + 1, 0, duplicatedQuestion);

    setLocalForm(updatedForm);
    setActiveQuestionId(newQuestionId);
    internalQuestionIdMap[newQuestionId] = uuidv4();

    toast.success("Question duplicated.");
  };

  const addQuestion = (question: any) => {
    debugger;
    const updatedForm = { ...localForm };

    updatedForm.questions.push({ ...question, isDraft: true });

    setLocalForm(updatedForm);
    setActiveQuestionId(question.id);
    internalQuestionIdMap[question.id] = uuidv4();
  };

  const onDragEnd = (result) => {
    if (!result.destination) {
      return;
    }
    const newQuestions = Array.from(localForm.questions);
    const [reorderedQuestion] = newQuestions.splice(result.source.index, 1);
    newQuestions.splice(result.destination.index, 0, reorderedQuestion);
    const updatedForm = { ...localForm, questions: newQuestions };
    setLocalForm(updatedForm);
  };

  const moveQuestion = (questionIndex: number, up: boolean) => {
    const newQuestions = Array.from(localForm.questions);
    const [reorderedQuestion] = newQuestions.splice(questionIndex, 1);
    const destinationIndex = up ? questionIndex - 1 : questionIndex + 1;
    newQuestions.splice(destinationIndex, 0, reorderedQuestion);
    const updatedForm = { ...localForm, questions: newQuestions };
    setLocalForm(updatedForm);
  };

  return (
    <div className="mt-12 px-5 py-4">

      <DragDropContext onDragEnd={onDragEnd}>
        <div className="mb-5 grid grid-cols-1 gap-5 ">
          <StrictModeDroppable droppableId="questionsList">
            {(provided) => (
              <div className="grid gap-5" ref={provided.innerRef} {...provided.droppableProps}>
                {localForm.questions.map((question, questionIdx) => (
                  // display a question form
                  <QuestionCard
                    key={internalQuestionIdMap[question.id]}
                    localForm={localForm}
                    questionIdx={questionIdx}
                    moveQuestion={moveQuestion}
                    updateQuestion={updateQuestion}
                    duplicateQuestion={duplicateQuestion}
                    deleteQuestion={deleteQuestion}
                    activeQuestionId={activeQuestionId}
                    setActiveQuestionId={setActiveQuestionId}
                    lastQuestion={questionIdx === localForm.questions.length - 1}
                    isInValid={invalidQuestions ? invalidQuestions.includes(question.id) : false}
                  />
                ))}
                {provided.placeholder}
              </div>
            )}
          </StrictModeDroppable>
        </div>
      </DragDropContext>
      <AddQuestionButton addQuestion={addQuestion} />
    </div>
  );
}
