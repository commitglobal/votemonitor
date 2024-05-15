import { BaseQuestion, DateQuestion } from '@/common/types';

import DisplayLogicEditor from './DisplayLogicEditor';
import QuestionHeader from './QuestionHeader';

export interface EditDateQuestionProps {
  formQuestions: BaseQuestion[];
  languageCode: string;
  availableLanguages: string[];
  questionIdx: number;
  isInValid: boolean;
  question: DateQuestion;
  updateQuestion: (questionIndex: number, question: BaseQuestion) => void;
}

function EditDateQuestion({
  formQuestions,
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

      <DisplayLogicEditor
        formQuestions={formQuestions}
        questionIndex={questionIdx}
        question={question}
        languageCode={languageCode}
        updateQuestion={updateQuestion} />
    </div>
  )
}


export default EditDateQuestion
