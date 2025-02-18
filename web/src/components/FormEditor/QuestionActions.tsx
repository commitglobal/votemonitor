import { ArrowUpIcon, ArrowDownIcon, TrashIcon, DocumentDuplicateIcon } from '@heroicons/react/24/solid';

import { MoveDirection } from './QuestionsEditor';

export interface QuestionActionsProps {
  questionIndex: number;
  isLastQuestion: boolean;
  disabled?: boolean;
  duplicateQuestion: (questionIndex: number) => void;
  deleteQuestion: (questionIndex: number) => void;
  moveQuestion: (questionIndex: number, direction: MoveDirection) => void;
}

function QuestionActions({
  questionIndex,
  isLastQuestion,
  duplicateQuestion,
  deleteQuestion,
  moveQuestion,
  disabled = false,
}: QuestionActionsProps) {
  return (
    <div className='flex space-x-4'>
      <ArrowUpIcon
        className={`h-4 cursor-pointer text-slate-500 hover:text-slate-600 ${
          questionIndex === 0 || disabled ? 'opacity-50 pointer-events-none' : ''
        }`}
        onClick={(e) => {
          if (questionIndex !== 0) {
            e.stopPropagation();
            moveQuestion(questionIndex, MoveDirection.UP);
          }
        }}
      />
      <ArrowDownIcon
        className={`h-4 cursor-pointer text-slate-500 hover:text-slate-600 ${
          isLastQuestion || disabled ? 'opacity-50 pointer-events-none' : ''
        }`}
        onClick={(e) => {
          if (!isLastQuestion) {
            e.stopPropagation();
            moveQuestion(questionIndex, MoveDirection.DOWN);
          }
        }}
      />
      <DocumentDuplicateIcon
        className={`h-4 cursor-pointer text-slate-500 hover:text-slate-600 ${
          disabled ? 'opacity-50 pointer-events-none' : ''
        } `}
        onClick={(e) => {
          e.stopPropagation();
          duplicateQuestion(questionIndex);
        }}
      />
      <TrashIcon
        className={`h-4 cursor-pointer text-slate-500 hover:text-slate-600 ${
          disabled ? 'opacity-50 pointer-events-none' : ''
        } `}
        onClick={(e) => {
          e.stopPropagation();
          deleteQuestion(questionIndex);
        }}
      />
    </div>
  );
}

export default QuestionActions;
