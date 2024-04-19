import { BaseQuestion, DateQuestion } from '@/common/types';
import { MoveDirection } from '../QuestionsEdit';
import QuestionHeader from './QuestionHeader';

export interface EditDateQuestionProps {
  languageCode: string;
  questionIdx: number;
  isInValid: boolean;
  question: DateQuestion;
  updateQuestion: (questionIndex: number, question: BaseQuestion) => void;
}

function EditDateQuestion({
  languageCode,
  questionIdx,
  isInValid,
  question,
  updateQuestion }: EditDateQuestionProps) {


  return (
    <div>
      <QuestionHeader
        languageCode={languageCode}
        isInValid={isInValid}
        question={question}
        questionIdx={questionIdx}
        updateQuestion={updateQuestion}
      />
    </div>
  )
}


export default EditDateQuestion
