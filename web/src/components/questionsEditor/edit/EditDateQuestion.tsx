import { BaseQuestion, DateQuestion } from '@/common/types';
import QuestionHeader from './QuestionHeader';

export interface EditDateQuestionProps {
  languageCode: string;
  availableLanguages: string[];
  questionIdx: number;
  isInValid: boolean;
  question: DateQuestion;
  updateQuestion: (questionIndex: number, question: BaseQuestion) => void;
}

function EditDateQuestion({
  availableLanguages,
  languageCode,
  questionIdx,
  isInValid,
  question,
  updateQuestion }: EditDateQuestionProps) {

  return (
    <div>
      <QuestionHeader
        availableLanguages={availableLanguages}
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
