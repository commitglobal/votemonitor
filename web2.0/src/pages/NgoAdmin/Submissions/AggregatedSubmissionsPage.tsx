import { format } from 'date-fns'
import { Link } from '@tanstack/react-router'
import { useSuspenseGetSubmissionsAggregatedDetails } from '@/queries/form-submissions-aggregated'
import { Route } from '@/routes/(app)/elections/$electionRoundId/submissions/by-form/$formId'
import {
  AggregatedAttachmentModel,
  AggregatedNoteModel,
} from '@/types/submissions-aggregate'
import { DownloadIcon, Languages, PlusIcon } from 'lucide-react'
import {
  isDateQuestionAggregate,
  isMultiSelectQuestionAggregate,
  isNumberQuestionAggregate,
  isRatingQuestionAggregate,
  isSingleSelectQuestionAggregate,
} from '@/lib/aggregate-guards'
import { mapFormType } from '@/lib/i18n'
import { getTranslation } from '@/lib/translated-string'
import { downloadFile } from '@/lib/utils'
import { DateTimeFormat } from '@/constants/formats'
import Attachment from '@/components/ui/attachment'
import { Badge } from '@/components/ui/badge'
import {
  Breadcrumb,
  BreadcrumbItem,
  BreadcrumbLink,
  BreadcrumbList,
  BreadcrumbPage,
  BreadcrumbSeparator,
} from '@/components/ui/breadcrumb'
import { Button } from '@/components/ui/button'
import {
  Card,
  CardAction,
  CardContent,
  CardHeader,
  CardTitle,
} from '@/components/ui/card'
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
import { MultiSelectAggregateChart } from '@/components/multi-select-aggregate-chart'
import { SingleSelectAggregateChart } from '@/components/single-select-aggregate-chart'

function AnswerNoteItem(index: number, note: AggregatedNoteModel) {
  return (
    <Item variant='outline' key={index}>
      <ItemContent>
        <ItemTitle>{note.text}</ItemTitle>
        <ItemDescription>
          {format(note.timeSubmitted, DateTimeFormat)}
        </ItemDescription>
        {/* Add link to submission details */}
      </ItemContent>
      <ItemActions>
        <CopyButton value={note.text} />
      </ItemActions>
    </Item>
  )
}

function AnswerAttachmentItem(
  index: number,
  attachment: AggregatedAttachmentModel
) {
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
        {/* Add link to submission details */}
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

function isAttachment(
  item: AggregatedAttachmentModel | AggregatedNoteModel
): item is AggregatedAttachmentModel {
  return 'presignedUrl' in item
}

export function AggregatedSubmissionsPage() {
  const { electionRoundId, formId } = Route.useParams()
  const { formLanguage, ...search } = Route.useSearch()
  const navigate = Route.useNavigate()
  const { data } = useSuspenseGetSubmissionsAggregatedDetails(
    electionRoundId,
    formId,
    search
  )

  const formDisplayLanguage =
    formLanguage ?? data.submissionsAggregate.defaultLanguage

  return (
    <>
      <Breadcrumb className='mb-4'>
        <BreadcrumbList>
          <BreadcrumbItem>
            <BreadcrumbLink asChild>
              <Link
                to='/elections/$electionRoundId/submissions/by-form'
                params={{ electionRoundId }}
                search={search}
                className='text-muted-foreground line-clamp-2 text-sm leading-normal font-normal text-balance underline'
              >
                Submissions by form
              </Link>
            </BreadcrumbLink>
          </BreadcrumbItem>
          <BreadcrumbSeparator />
          <BreadcrumbItem>
            <BreadcrumbPage>{formId}</BreadcrumbPage>
          </BreadcrumbItem>
        </BreadcrumbList>
      </Breadcrumb>
      <Card>
        <CardHeader>
          <CardTitle className='flex items-center gap-2'>
            <span> {data.submissionsAggregate.formCode} </span>
            <span>
              {getTranslation(
                data.submissionsAggregate.name,
                formDisplayLanguage
              )}
            </span>

            <Badge>{mapFormType(data.submissionsAggregate.formType)}</Badge>
          </CardTitle>
          <CardAction>
            <DropdownMenu>
              <DropdownMenuTrigger asChild>
                <Button variant='outline' className='gap-2 bg-transparent'>
                  <Languages className='h-5 w-5' />
                  <span>{formDisplayLanguage}</span>
                </Button>
              </DropdownMenuTrigger>
              <DropdownMenuContent align='end' className='w-48'>
                {data.submissionsAggregate.languages.map((language) => (
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
          </CardAction>
        </CardHeader>
        <CardContent>
          {Object.entries(data.submissionsAggregate.aggregates).map(
            ([questionId, aggregate]) => {
              const notes = data.notes.filter(
                (note) => note.questionId === questionId
              )
              const attachments = data.attachments.filter(
                (attachment) => attachment.questionId === questionId
              )
              const answerAndAttachments = [...notes, ...attachments].sort(
                (a, b) =>
                  new Date(b.timeSubmitted).getTime() -
                  new Date(a.timeSubmitted).getTime()
              )

              return (
                <div key={questionId}>
                  <ItemGroup>
                    <Item>
                      <ItemHeader>
                        {aggregate.question.code} -
                        {getTranslation(
                          aggregate.question.text,
                          formDisplayLanguage
                        )}
                      </ItemHeader>
                      <ItemContent>
                        <ItemDescription className='italic'>
                          {getTranslation(
                            aggregate.question.helptext ?? {},
                            formDisplayLanguage
                          )}
                        </ItemDescription>
                        {isDateQuestionAggregate(aggregate) && <>TBD</>}
                        {isNumberQuestionAggregate(aggregate) && <>TBD</>}
                        {isRatingQuestionAggregate(aggregate) && <>TBD</>}
                        {isSingleSelectQuestionAggregate(aggregate) && (
                          <SingleSelectAggregateChart
                            language={formDisplayLanguage}
                            aggregate={aggregate}
                          />
                        )}
                        {isMultiSelectQuestionAggregate(aggregate) && (
                          <MultiSelectAggregateChart
                            language={formDisplayLanguage}
                            aggregate={aggregate}
                          />
                        )}
                        {isDateQuestionAggregate(aggregate) && <>TBD</>}
                      </ItemContent>
                    </Item>
                  </ItemGroup>
                  {/** TODO: add graphs here */}
                  {(notes.length > 0 || attachments.length > 0) && (
                    <Collapsible className='ml-2'>
                      <CollapsibleTrigger asChild>
                        <Button
                          variant='link'
                          size='sm'
                          className='cursor-pointer'
                        >
                          {
                            <>
                              {notes.length > 0 && `${notes.length} notes`}
                              {notes.length > 0 &&
                                attachments.length > 0 &&
                                ' & '}
                              {attachments.length > 0 &&
                                `${attachments.length} media files`}
                            </>
                          }
                          <PlusIcon />
                        </Button>
                      </CollapsibleTrigger>
                      <CollapsibleContent>
                        <ItemGroup className='gap-2'>
                          {answerAndAttachments.map((data, index) =>
                            isAttachment(data)
                              ? AnswerAttachmentItem(index, data)
                              : AnswerNoteItem(index, data)
                          )}
                        </ItemGroup>
                      </CollapsibleContent>
                    </Collapsible>
                  )}
                </div>
              )
            }
          )}
        </CardContent>
      </Card>
    </>
  )
}
