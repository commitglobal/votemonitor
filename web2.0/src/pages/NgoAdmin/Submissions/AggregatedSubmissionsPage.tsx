import { format } from 'date-fns'
import { Link } from '@tanstack/react-router'
import { useSuspenseGetSubmissionsAggregatedDetails } from '@/queries/form-submissions-aggregated'
import { Route } from '@/routes/(app)/elections/$electionRoundId/submissions/by-form/$formId'
import {
  AggregatedAttachmentModel,
  AggregatedNoteModel,
} from '@/types/submissions-aggregate'
import { DownloadIcon, PlusIcon } from 'lucide-react'
import { mapFormType } from '@/lib/i18n'
import { getTranslation } from '@/lib/translated-string'
import { downloadFile } from '@/lib/utils'
import { DateTimeFormat } from '@/constants/formats'
import Attachment from '@/components/ui/attachment'
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
import {
  Collapsible,
  CollapsibleContent,
  CollapsibleTrigger,
} from '@/components/ui/collapsible'
import { CopyButton } from '@/components/ui/copy-button'
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
  const { data: data } = useSuspenseGetSubmissionsAggregatedDetails(
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
        <CardContent>
          <ItemGroup className='flex flex-row justify-between gap-2'>
            {/* <Item>
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
            </Item> */}
          </ItemGroup>

          <ItemGroup>
            {/*  {!submission.isOwnObserver ? (
              <Item>
                <ItemContent>
                  <ItemTitle>NGO</ItemTitle>
                  <ItemDescription>{submission.ngoName}</ItemDescription>
                </ItemContent>
              </Item>
            ) : null} */}

            <Item>
              <ItemContent>
                <ItemTitle>Form type</ItemTitle>
                <ItemDescription>
                  {mapFormType(data.submissionsAggregate.formType)}
                </ItemDescription>
              </ItemContent>
            </Item>
            <Item>
              <ItemContent>
                <ItemTitle>Form code</ItemTitle>
                <ItemDescription>
                  {data.submissionsAggregate.formCode}
                </ItemDescription>
              </ItemContent>
            </Item>
            <Item>
              <ItemContent>
                <ItemTitle>Form name</ItemTitle>
                <ItemDescription>
                  {getTranslation(
                    data.submissionsAggregate.name,
                    formDisplayLanguage
                  )}
                </ItemDescription>
              </ItemContent>
            </Item>
          </ItemGroup>

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
                      </ItemContent>
                    </Item>
                  </ItemGroup>
                  {/** TODO: add graphs here */}
                  <ItemGroup>
                    {(notes.length > 0 || attachments.length > 0) && (
                      <Collapsible>
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
                  </ItemGroup>
                </div>
              )
            }
          )}
        </CardContent>
      </Card>
    </>
  )
}
