import { createContext, useContext, useState, ReactNode } from 'react'
import type {
  TextAnswer,
  NumberAnswer,
  DateAnswer,
  RatingAnswer,
  SingleSelectAnswer,
  MultiSelectAnswer,
} from '@/types/forms-submission'

export type Answer =
  | TextAnswer
  | NumberAnswer
  | DateAnswer
  | RatingAnswer
  | SingleSelectAnswer
  | MultiSelectAnswer

export type FormAnswers = Record<string, Answer>

interface FormAnswersContextType {
  answers: FormAnswers
  setTextAnswer: (questionId: string, value: string) => void
  setNumberAnswer: (questionId: string, value: number | null) => void
  setDateAnswer: (questionId: string, value: string) => void
  setSingleSelectAnswer: (questionId: string, value: string | null) => void
  setMultiSelectAnswer: (questionId: string, value: string[]) => void
  toggleMultiSelectOption: (questionId: string, optionId: string) => void
  setRatingAnswer: (questionId: string, value: number | null) => void
  clearAnswers: () => void
  getAnswer: (questionId: string) => Answer | undefined
}

const FormAnswersContext = createContext<FormAnswersContextType | undefined>(
  undefined
)

export function FormAnswersProvider({ children }: { children: ReactNode }) {
  const [answers, setAnswers] = useState<FormAnswers>({})

  const setTextAnswer = (questionId: string, text: string) => {
    setAnswers((prev) => ({
      ...prev,
      [questionId]: {
        $answerType: 'textAnswer',
        questionId,
        text,
      } as TextAnswer,
    }))
  }

  const setNumberAnswer = (questionId: string, value: number | null) => {
    setAnswers((prev) => ({
      ...prev,
      [questionId]: {
        $answerType: 'numberAnswer',
        questionId,
        value: value ?? undefined,
      } as NumberAnswer,
    }))
  }

  const setDateAnswer = (questionId: string, date: string) => {
    setAnswers((prev) => ({
      ...prev,
      [questionId]: {
        $answerType: 'dateAnswer',
        questionId,
        date,
      } as DateAnswer,
    }))
  }

  const setSingleSelectAnswer = (
    questionId: string,
    optionId: string | null
  ) => {
    setAnswers((prev) => ({
      ...prev,
      [questionId]: {
        $answerType: 'singleSelectAnswer',
        questionId,
        selection: optionId ? { optionId } : undefined,
      } as SingleSelectAnswer,
    }))
  }

  const setMultiSelectAnswer = (questionId: string, optionIds: string[]) => {
    setAnswers((prev) => ({
      ...prev,
      [questionId]: {
        $answerType: 'multiSelectAnswer',
        questionId,
        selection: optionIds.map((optionId) => ({ optionId })),
      } as MultiSelectAnswer,
    }))
  }

  const toggleMultiSelectOption = (questionId: string, optionId: string) => {
    setAnswers((prev) => {
      const currentAnswer = prev[questionId] as MultiSelectAnswer | undefined
      const currentSelection = currentAnswer?.selection || []
      const currentOptionIds = currentSelection
        .map((s) => s.optionId)
        .filter(Boolean) as string[]

      const newOptionIds = currentOptionIds.includes(optionId)
        ? currentOptionIds.filter((id) => id !== optionId)
        : [...currentOptionIds, optionId]

      return {
        ...prev,
        [questionId]: {
          $answerType: 'multiSelectAnswer',
          questionId,
          selection: newOptionIds.map((id) => ({ optionId: id })),
        } as MultiSelectAnswer,
      }
    })
  }

  const setRatingAnswer = (questionId: string, value: number | null) => {
    setAnswers((prev) => ({
      ...prev,
      [questionId]: {
        $answerType: 'ratingAnswer',
        questionId,
        value: value ?? undefined,
      } as RatingAnswer,
    }))
  }

  const clearAnswers = () => {
    setAnswers({})
  }

  const getAnswer = (questionId: string): Answer | undefined => {
    return answers[questionId]
  }

  return (
    <FormAnswersContext.Provider
      value={{
        answers,
        setTextAnswer,
        setNumberAnswer,
        setDateAnswer,
        setSingleSelectAnswer,
        setMultiSelectAnswer,
        toggleMultiSelectOption,
        setRatingAnswer,
        clearAnswers,
        getAnswer,
      }}
    >
      {children}
    </FormAnswersContext.Provider>
  )
}

export function useFormAnswers() {
  const context = useContext(FormAnswersContext)
  if (context === undefined) {
    throw new Error('useFormAnswers must be used within a FormAnswersProvider')
  }
  return context
}
