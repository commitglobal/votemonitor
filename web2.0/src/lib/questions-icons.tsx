import { QuestionType } from "@/types/form";

import {
    TextIcon,
    CalculatorIcon,
    CalendarIcon,
    CheckCircleIcon,
    CheckSquareIcon,
    StarIcon
  } from 'lucide-react';

export const questionsIconMapping = {
    [QuestionType.TextQuestionType]: TextIcon,
    [QuestionType.NumberQuestionType]: CalculatorIcon,
    [QuestionType.DateQuestionType]: CalendarIcon,
    [QuestionType.RatingQuestionType]: StarIcon,
    [QuestionType.SingleSelectQuestionType]: CheckCircleIcon,
    [QuestionType.MultiSelectQuestionType]: CheckSquareIcon,
  };