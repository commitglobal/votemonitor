'use client'

import { format } from 'date-fns'
import { Link, useRouter } from '@tanstack/react-router'
import { queryClient } from '@/main'
import { useUpdateFormSubmissionFollowUpStatusMutation } from '@/mutations/form-submissions'
import { useElectionRoundDetails } from '@/queries/elections'
import {
  formSubmissionKyes,
  useSuspenseGetFormSubmissionDetails,
} from '@/queries/form-submissions'
import { useSuspenseGetFormDetails } from '@/queries/forms'
import { Route } from '@/routes/(app)/elections/$electionRoundId/submissions/$submissionId'
import { ElectionRoundStatus } from '@/types/election'
import {
  AttachmentModel,
  FormSubmissionFollowUpStatus,
  NoteModel,
  RatingAnswer,
  type FormSubmissionDetailedModel,
} from '@/types/forms-submission'
import { DownloadIcon, Languages, PlusIcon } from 'lucide-react'
import { toast } from 'sonner'
import {
  isDateAnswer,
  isMultiSelectAnswer,
  isNumberAnswer,
  isSingleSelectAnswer,
  isTextAnswer,
} from '@/lib/answer-guards'
import { mapFormType } from '@/lib/i18n'
import {
  isMultiSelectQuestion,
  isRatingQuestion,
  isSingleSelectQuestion,
} from '@/lib/question-guards'
import { getTranslation } from '@/lib/translated-string'
import { arrayToKeyObject, downloadFile, groupArrayByKey } from '@/lib/utils'
import { DateTimeFormat } from '@/constants/formats'
import { Attachment } from '@/components/ui/attachment'
import {
  Breadcrumb,
  BreadcrumbItem,
  BreadcrumbLink,
  BreadcrumbList,
  BreadcrumbPage,
  BreadcrumbSeparator,
} from '@/components/ui/breadcrumb'
import { Button } from '@/components/ui/button'
import { Card, CardContent } from '@/components/ui/card'
import { Checkbox } from '@/components/ui/checkbox'
import {
  Collapsible,
  CollapsibleContent,
  CollapsibleTrigger,
} from '@/components/ui/collapsible'
import { CopyButton } from '@/components/ui/copy-button'
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from '@/components/ui/dropdown-menu'
import {
  Item,
  ItemActions,
  ItemContent,
  ItemDescription,
  ItemGroup,
  ItemHeader,
  ItemMedia,
  ItemTitle,
} from '@/components/ui/item'
import { Label } from '@/components/ui/label'
import { RadioGroup, RadioGroupItem } from '@/components/ui/radio-group'
import { RatingGroup } from '@/components/ui/rating-group'
import {
  Select,
  SelectContent,
  SelectGroup,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/components/ui/select'
import FormSubmissionFollowUpStatusBadge from '@/components/form-submission-follow-up-status-badge'

const buildSearchFilters = (
  submission: FormSubmissionDetailedModel,
  level: number
) => {
  const filters: Record<string, string> = {}
  const levels = [
    { key: 'level1Filter', value: submission.level1 },
    { key: 'level2Filter', value: submission.level2 },
    { key: 'level3Filter', value: submission.level3 },
    { key: 'level4Filter', value: submission.level4 },
    { key: 'level5Filter', value: submission.level5 },
  ]

  levels.slice(0, level).forEach(({ key, value }) => {
    if (value) filters[key] = value
  })

  return filters
}

function PollingStationDetails({
  submission,
}: {
  submission: FormSubmissionDetailedModel
}) {
  const { electionRoundId } = Route.useParams()

  const levels = [
    { value: submission.level1, level: 1 },
    { value: submission.level2, level: 2 },
    { value: submission.level3, level: 3 },
    { value: submission.level4, level: 4 },
    { value: submission.level5, level: 5 },
  ].filter((item) => item.value)

  return (
    <Item>
      <ItemContent>
        <ItemTitle>Polling station</ItemTitle>
        <div>
          <Breadcrumb>
            <BreadcrumbList>
              {levels.map(({ value, level }, index) => (
                <div key={level} className='flex items-center'>
                  {index > 0 && <BreadcrumbSeparator />}
                  <BreadcrumbItem>
                    <BreadcrumbLink asChild>
                      <Link
                        to='/elections/$electionRoundId/submissions/by-form'
                        search={buildSearchFilters(submission, level)}
                        params={{ electionRoundId }}
                        className='text-muted-foreground line-clamp-2 text-sm leading-normal font-normal text-balance underline'
                      >
                        {value}
                      </Link>
                    </BreadcrumbLink>
                  </BreadcrumbItem>
                </div>
              ))}

              {submission.number && (
                <>
                  <BreadcrumbSeparator />
                  <BreadcrumbItem>
                    <BreadcrumbLink asChild>
                      <Link
                        to='/elections/$electionRoundId/submissions/by-form'
                        search={{
                          ...buildSearchFilters(submission, 5),
                          pollingStationNumberFilter: submission.number,
                        }}
                        params={{ electionRoundId }}
                        className='text-muted-foreground line-clamp-2 text-sm leading-normal font-normal text-balance underline'
                      >
                        {submission.number}
                      </Link>
                    </BreadcrumbLink>
                  </BreadcrumbItem>
                </>
              )}
            </BreadcrumbList>
          </Breadcrumb>
        </div>
      </ItemContent>
    </Item>
  )
}

export function Page() {
  const { electionRoundId, submissionId } = Route.useParams()
  const { formLanguage, from } = Route.useSearch()
  const navigate = Route.useNavigate()

  const { invalidate } = useRouter()
  const { data: submission } = useSuspenseGetFormSubmissionDetails(
    electionRoundId,
    submissionId
  )

  const { data: form } = useSuspenseGetFormDetails(
    electionRoundId,
    submission.formId
  )

  const formDisplayLanguage = formLanguage ?? form.defaultLanguage

  const answersMap = arrayToKeyObject(submission.answers || [], 'questionId')
  const attachmentsMap = groupArrayByKey(
    submission.attachments || [],
    'questionId'
  )
  const notesMap = groupArrayByKey(submission.notes || [], 'questionId')

  const mappedQuestions = form.questions.map((question) => ({
    ...question,
    notes: notesMap[question.id] || [],
    attachments: attachmentsMap[question.id] || [],
    answerAndAttachments: [
      ...(notesMap[question.id] || []),
      ...(attachmentsMap[question.id] || []),
    ].sort(
      (a, b) =>
        new Date(b.timeSubmitted).getTime() -
        new Date(a.timeSubmitted).getTime()
    ),
    answer: answersMap[question.id],
  }))

  const { data: electionRound } = useElectionRoundDetails(electionRoundId)
  const { mutate: updateStatus } =
    useUpdateFormSubmissionFollowUpStatusMutation()

  const handleFollowUpStatusChange = (
    followUpStatus: FormSubmissionFollowUpStatus
  ) => {
    updateStatus(
      { electionRoundId, formSubmissionId: submissionId, followUpStatus },
      {
        onSuccess: async (_, { electionRoundId }) => {
          toast.success('Follow-up status updated')
          invalidate()
          await queryClient.invalidateQueries({
            queryKey: formSubmissionKyes.all(electionRoundId),
          })
        },
        onError: () => {
          toast.error('Error updating follow up status', {
            description: 'Please contact tech support',
          })
        },
      }
    )
  }

  const isReadOnly =
    !submission.isOwnObserver ||
    electionRound?.status === ElectionRoundStatus.Archived

  return (
    <>
      <Breadcrumb className='mb-4'>
        <BreadcrumbList>
          <BreadcrumbItem>
            <BreadcrumbLink asChild>
              <Link
                to='/elections/$electionRoundId/submissions'
                params={{ electionRoundId }}
                search={from}
                className='text-muted-foreground line-clamp-2 text-sm leading-normal font-normal text-balance underline'
              >
                Submissions
              </Link>
            </BreadcrumbLink>
          </BreadcrumbItem>
          <BreadcrumbSeparator />
          <BreadcrumbItem>
            <BreadcrumbPage>{submissionId}</BreadcrumbPage>
          </BreadcrumbItem>
        </BreadcrumbList>
      </Breadcrumb>
      <Card>
        <CardContent>
          <ItemGroup className='flex flex-row justify-between gap-2'>
            <Item>
              <ItemContent>
                <ItemTitle>Observer</ItemTitle>
                <ItemDescription>
                  <Link
                    to='/elections/$electionRoundId/observers/$observerId'
                    params={{
                      electionRoundId,
                      observerId: submission.monitoringObserverId,
                    }}
                  >
                    {submission.observerName}
                  </Link>
                </ItemDescription>
              </ItemContent>
            </Item>

            <Item>
              <ItemContent>
                <ItemActions>
                  <DropdownMenu>
                    <DropdownMenuTrigger asChild>
                      <Button
                        variant='outline'
                        className='gap-2 bg-transparent'
                      >
                        <Languages className='h-5 w-5' />
                        <span>{formDisplayLanguage}</span>
                      </Button>
                    </DropdownMenuTrigger>
                    <DropdownMenuContent align='end' className='w-48'>
                      {form.languages.map((language) => (
                        <DropdownMenuItem
                          key={language}
                          onClick={() =>
                            navigate({
                              to: '.',
                              search: (prev) => ({
                                ...prev,
                                formLanguage: language,
                              }),
                            })
                          }
                          className='flex cursor-pointer items-center gap-2'
                        >
                          {/* <span className='text-lg'>{language.flag}</span> */}
                          <span className='flex-1'>{language}</span>
                          {formDisplayLanguage === language && (
                            <span className='text-primary'>✓</span>
                          )}
                        </DropdownMenuItem>
                      ))}
                    </DropdownMenuContent>
                  </DropdownMenu>
                </ItemActions>
              </ItemContent>
            </Item>
          </ItemGroup>

          <ItemGroup>
            {!submission.isOwnObserver ? (
              <Item>
                <ItemContent>
                  <ItemTitle>NGO</ItemTitle>
                  <ItemDescription>{submission.ngoName}</ItemDescription>
                </ItemContent>
              </Item>
            ) : null}

            <Item>
              <ItemContent>
                <ItemTitle>Form type</ItemTitle>
                <ItemDescription>{mapFormType(form.formType)}</ItemDescription>
              </ItemContent>
            </Item>
            <Item>
              <ItemContent>
                <ItemTitle>Form code</ItemTitle>
                <ItemDescription>{form.code}</ItemDescription>
              </ItemContent>
            </Item>
            <Item>
              <ItemContent>
                <ItemTitle>Form name</ItemTitle>
                <ItemDescription>
                  {getTranslation(form.name, formDisplayLanguage)}
                </ItemDescription>
              </ItemContent>
            </Item>
          </ItemGroup>

          <PollingStationDetails submission={submission} />
          <Item>
            <ItemContent>
              <ItemTitle>Follow up status</ItemTitle>
              <ItemDescription>
                {isReadOnly ? (
                  <FormSubmissionFollowUpStatusBadge
                    followUpStatus={submission.followUpStatus}
                  />
                ) : (
                  <Select
                    onValueChange={handleFollowUpStatusChange}
                    defaultValue={submission.followUpStatus}
                    value={submission.followUpStatus}
                    disabled={isReadOnly}
                  >
                    <SelectTrigger className='w-full sm:w-[220px]'>
                      <SelectValue placeholder='Follow-up status' />
                    </SelectTrigger>
                    <SelectContent>
                      <SelectGroup>
                        {Object.values(FormSubmissionFollowUpStatus).map(
                          (status) => (
                            <SelectItem key={status} value={status}>
                              <FormSubmissionFollowUpStatusBadge
                                followUpStatus={status}
                              />
                            </SelectItem>
                          )
                        )}
                      </SelectGroup>
                    </SelectContent>
                  </Select>
                )}
              </ItemDescription>
            </ItemContent>
          </Item>
          {mappedQuestions.map((question) => {
            const answer = answersMap[question.id]
            return (
              <div key={question.id}>
                <ItemGroup>
                  <Item>
                    <ItemHeader>
                      {question.code} -
                      {getTranslation(question.text, formDisplayLanguage)}
                    </ItemHeader>
                    <ItemContent>
                      <ItemDescription className='italic'>
                        {getTranslation(
                          question.helptext ?? {},
                          formDisplayLanguage
                        )}
                      </ItemDescription>
                      {isDateAnswer(answer) && (
                        <p>
                          {answer.date
                            ? format(answer.date, DateTimeFormat)
                            : '-'}
                        </p>
                      )}

                      {isNumberAnswer(answer) && <p>{answer.value ?? '-'}</p>}

                      {isTextAnswer(answer) && (
                        <div className='rounded-lg border p-3'>
                          {answer.text}
                        </div>
                      )}

                      {isSingleSelectAnswer(answer) &&
                        isSingleSelectQuestion(question) && (
                          <RadioGroup
                            value={answer.selection?.optionId}
                            disabled={true}
                          >
                            {question.options.map((option) => (
                              <div
                                className='flex items-center space-x-2'
                                key={option.id}
                              >
                                <RadioGroupItem
                                  value={option.id}
                                  id={option.id}
                                  className='disabled:cursor-default disabled:opacity-100'
                                />
                                <Label
                                  htmlFor={option.id}
                                  className='peer-disabled:cursor-default peer-disabled:opacity-100'
                                >
                                  {getTranslation(
                                    option.text,
                                    formDisplayLanguage
                                  )}
                                </Label>
                              </div>
                            ))}
                          </RadioGroup>
                        )}

                      {isMultiSelectAnswer(answer) &&
                        isMultiSelectQuestion(question) && (
                          <div className='flex flex-col gap-3'>
                            {question.options.map((option) => (
                              <div
                                className='flex items-center gap-3'
                                key={option.id}
                              >
                                <Checkbox
                                  id={option.id}
                                  disabled
                                  className='disabled:cursor-default disabled:opacity-100'
                                  checked={answer.selection?.some(
                                    (selection) =>
                                      selection.optionId === option.id
                                  )}
                                />
                                <Label
                                  htmlFor={option.id}
                                  className='peer-disabled:cursor-default peer-disabled:opacity-100'
                                >
                                  {getTranslation(
                                    option.text,
                                    formDisplayLanguage
                                  )}
                                </Label>
                              </div>
                            ))}
                          </div>
                        )}

                      {isRatingQuestion(question) && (
                        <RatingGroup
                          scale={question.scale}
                          value={(answer as RatingAnswer)?.value?.toString()}
                          disabled={true}
                          lowerLabel={getTranslation(
                            question.lowerLabel ?? {},
                            formDisplayLanguage
                          )}
                          upperLabel={getTranslation(
                            question.upperLabel ?? {},
                            formDisplayLanguage
                          )}
                        />
                      )}
                    </ItemContent>
                  </Item>
                </ItemGroup>

                <ItemGroup>
                  {(question.notes.length > 0 ||
                    question.attachments.length > 0) && (
                    <Collapsible>
                      <CollapsibleTrigger asChild>
                        <Button
                          variant='link'
                          size='sm'
                          className='cursor-pointer'
                        >
                          {
                            <>
                              {question.notes.length > 0 &&
                                `${question.notes.length} notes`}
                              {question.notes.length > 0 &&
                                question.attachments.length > 0 &&
                                ' & '}
                              {question.attachments.length > 0 &&
                                `${question.attachments.length} media files`}
                            </>
                          }
                          <PlusIcon />
                        </Button>
                      </CollapsibleTrigger>
                      <CollapsibleContent>
                        <ItemGroup className='gap-2'>
                          {question.answerAndAttachments.map((data, index) =>
                            isAttachment(data)
                              ? AnswerAttachment(index, data)
                              : AnswerNote(index, data)
                          )}
                        </ItemGroup>
                      </CollapsibleContent>
                    </Collapsible>
                  )}
                </ItemGroup>
              </div>
            )
          })}
        </CardContent>
      </Card>
    </>
  )
}

function isAttachment(
  item: AttachmentModel | NoteModel
): item is AttachmentModel {
  return 'presignedUrl' in item // `fileName` only exists on AttachmentModel
}

function AnswerNote(index: number, note: NoteModel) {
  return (
    <Item variant='outline' key={index}>
      <ItemContent>
        <ItemTitle>{note.text}</ItemTitle>
        <ItemDescription>
          {format(note.timeSubmitted, DateTimeFormat)}
        </ItemDescription>
      </ItemContent>
      <ItemActions>
        <CopyButton value={note.text} />
      </ItemActions>
    </Item>
  )
}

function AnswerAttachment(index: number, attachment: AttachmentModel) {
  return (
    <Item variant='outline' key={index}>
      <ItemMedia>
        <Attachment
          src={attachment.presignedUrl}
          mimeType={attachment.mimeType}
          fileName={attachment.fileName}
          width='530px'
          height='300px'
        />
      </ItemMedia>
      <ItemContent>
        <ItemTitle>
          {format(attachment.timeSubmitted, DateTimeFormat)}
        </ItemTitle>
        <ItemDescription>{attachment.mimeType}</ItemDescription>
      </ItemContent>
      <ItemActions>
        <Button
          variant='outline'
          size='icon'
          onClick={() =>
            downloadFile(attachment.presignedUrl, attachment.fileName)
          }
        >
          <DownloadIcon className='size-4' />
        </Button>
      </ItemActions>
    </Item>
  )
}
