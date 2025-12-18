import { useFormAnswers } from '@/contexts/form-answers.context'
import type { FormModel } from '@/types/form'
import { Flag, Pencil } from 'lucide-react'
import {
  isDateAnswer,
  isMultiSelectAnswer,
  isNumberAnswer,
  isRatingAnswer,
  isSingleSelectAnswer,
  isTextAnswer,
} from '@/lib/answer-guards'
import {
  isDateQuestion,
  isMultiSelectQuestion,
  isNumberQuestion,
  isRatingQuestion,
  isSingleSelectQuestion,
  isTextQuestion,
} from '@/lib/question-guards'
import { getTranslation } from '@/lib/translated-string'
import { Badge } from '@/components/ui/badge'
import { Card, CardContent } from '@/components/ui/card'
import { Checkbox } from '@/components/ui/checkbox'
import { Input } from '@/components/ui/input'
import {
  Item,
  ItemContent,
  ItemDescription,
  ItemGroup,
  ItemHeader,
} from '@/components/ui/item'
import { Label } from '@/components/ui/label'
import { RadioGroup, RadioGroupItem } from '@/components/ui/radio-group'
import { RatingGroup } from '@/components/ui/rating-group'
import { Textarea } from '@/components/ui/textarea'
import { P } from '@/components/ui/typography'

interface QuestionsCardProps {
  form: FormModel
  formDisplayLanguage: string
}

function QuestionsCard({ form, formDisplayLanguage }: QuestionsCardProps) {
  const {
    getAnswer,
    setTextAnswer,
    setNumberAnswer,
    setDateAnswer,
    setSingleSelectAnswer,
    toggleMultiSelectOption,
    setRatingAnswer,
  } = useFormAnswers()

  return (
    <Card>
      <CardContent className='space-y-2 pt-6'>
        {form.questions.length === 0 ? (
          <P className='text-muted-foreground'>No questions added yet.</P>
        ) : (
          form.questions.map((question) => {
            const answer = getAnswer(question.id)

            return (
              <div key={question.id} className='mb-4'>
                <ItemGroup>
                  <Item variant='outline'>
                    <ItemHeader>
                      <div className='flex items-start gap-2'>
                        <Badge variant='secondary'> {question.code}</Badge>
                        <div>
                          <div className='font-semibold'>
                            {getTranslation(question.text, formDisplayLanguage)}
                          </div>
                          {question.helptext && (
                            <ItemDescription className='mt-1'>
                              {getTranslation(
                                question.helptext,
                                formDisplayLanguage
                              )}
                            </ItemDescription>
                          )}
                        </div>
                      </div>
                    </ItemHeader>
                    <ItemContent>
                      {/* Text Question */}
                      {isTextQuestion(question) && (
                        <div className='mt-2'>
                          <Textarea
                            value={
                              answer && isTextAnswer(answer)
                                ? answer.text || ''
                                : ''
                            }
                            onChange={(e) =>
                              setTextAnswer(question.id, e.target.value)
                            }
                            placeholder={
                              question.inputPlaceholder
                                ? getTranslation(
                                    question.inputPlaceholder,
                                    formDisplayLanguage
                                  )
                                : ''
                            }
                            rows={3}
                            className='max-w-2xl resize-none'
                          />
                        </div>
                      )}

                      {/* Number Question */}
                      {isNumberQuestion(question) && (
                        <div className='mt-2'>
                          <Input
                            type='number'
                            value={
                              answer &&
                              isNumberAnswer(answer) &&
                              answer.value !== undefined
                                ? answer.value
                                : ''
                            }
                            onChange={(e) => {
                              const value =
                                e.target.value === ''
                                  ? null
                                  : Number(e.target.value)
                              setNumberAnswer(question.id, value)
                            }}
                            placeholder={
                              question.inputPlaceholder
                                ? getTranslation(
                                    question.inputPlaceholder,
                                    formDisplayLanguage
                                  )
                                : ''
                            }
                            className='max-w-xs'
                          />
                        </div>
                      )}

                      {/* Date Question */}
                      {isDateQuestion(question) && (
                        <div className='mt-2'>
                          <Input
                            type='date'
                            value={
                              answer && isDateAnswer(answer)
                                ? answer.date || ''
                                : ''
                            }
                            onChange={(e) =>
                              setDateAnswer(question.id, e.target.value)
                            }
                            className='max-w-xs'
                          />
                        </div>
                      )}

                      {/* Single Select Question */}
                      {isSingleSelectQuestion(question) && (
                        <RadioGroup
                          className='mt-2 space-y-2'
                          value={
                            answer && isSingleSelectAnswer(answer)
                              ? answer.selection?.optionId || ''
                              : ''
                          }
                          onValueChange={(value) =>
                            setSingleSelectAnswer(question.id, value)
                          }
                        >
                          {question.options.map((option) => (
                            <div
                              key={option.id}
                              className='flex items-center space-x-2'
                            >
                              <RadioGroupItem
                                value={option.id}
                                id={option.id}
                              />
                              <Label
                                htmlFor={option.id}
                                className='flex items-center gap-1.5 font-normal'
                              >
                                {getTranslation(
                                  option.text,
                                  formDisplayLanguage
                                )}
                                {option.isFlagged && (
                                  <Flag className='h-4 w-4 text-red-500' />
                                )}
                                {option.isFreeText && (
                                  <Pencil className='h-4 w-4 text-gray-500' />
                                )}
                              </Label>
                            </div>
                          ))}
                        </RadioGroup>
                      )}

                      {/* Multi Select Question */}
                      {isMultiSelectQuestion(question) && (
                        <div className='mt-2 space-y-2'>
                          {question.options.map((option) => {
                            const selectedOptionIds =
                              answer && isMultiSelectAnswer(answer)
                                ? answer.selection
                                    ?.map((s) => s.optionId)
                                    .filter(Boolean) || []
                                : []
                            const isChecked = selectedOptionIds.includes(
                              option.id
                            )

                            return (
                              <div
                                key={option.id}
                                className='flex items-center space-x-2'
                              >
                                <Checkbox
                                  id={option.id}
                                  checked={isChecked}
                                  onCheckedChange={() =>
                                    toggleMultiSelectOption(
                                      question.id,
                                      option.id
                                    )
                                  }
                                />
                                <Label
                                  htmlFor={option.id}
                                  className='flex items-center gap-1.5 font-normal'
                                >
                                  {getTranslation(
                                    option.text,
                                    formDisplayLanguage
                                  )}
                                  {option.isFlagged && (
                                    <Flag className='h-4 w-4 text-red-500' />
                                  )}
                                  {option.isFreeText && (
                                    <Pencil className='h-4 w-4 text-gray-500' />
                                  )}
                                </Label>
                              </div>
                            )
                          })}
                        </div>
                      )}

                      {/* Rating Question */}
                      {isRatingQuestion(question) && (
                        <div className='mt-2 space-y-2'>
                          {question.lowerLabel && (
                            <ItemDescription>
                              {getTranslation(
                                question.lowerLabel,
                                formDisplayLanguage
                              )}
                            </ItemDescription>
                          )}
                          <RatingGroup
                            scale={question.scale}
                            value={
                              answer &&
                              isRatingAnswer(answer) &&
                              answer.value !== undefined
                                ? answer.value.toString()
                                : undefined
                            }
                            onValueChange={(value) =>
                              setRatingAnswer(
                                question.id,
                                value ? Number(value) : null
                              )
                            }
                          />
                          {question.upperLabel && (
                            <ItemDescription>
                              {getTranslation(
                                question.upperLabel,
                                formDisplayLanguage
                              )}
                            </ItemDescription>
                          )}
                        </div>
                      )}
                    </ItemContent>
                  </Item>
                </ItemGroup>
              </div>
            )
          })
        )}
      </CardContent>
    </Card>
  )
}

export default QuestionsCard
