'use client'

import { Link, useRouter } from '@tanstack/react-router'
import { queryClient } from '@/main'
import { useUpdateFormSubmissionFollowUpStatusMutation } from '@/mutations/form-submissions'
import { useElectionRoundDetails } from '@/queries/elections'
import {
  formSubmissionKyes,
  useSuspenseGetFormSubmissionDetails,
} from '@/queries/form-submissions'
import { Route } from '@/routes/(app)/elections/$electionRoundId/submissions/$submissionId'
import { ElectionRoundStatus } from '@/types/election'
import {
  FormSubmissionFollowUpStatus,
  type FormSubmissionDetailedModel,
} from '@/types/forms-submission'
import { DownloadIcon, EditIcon } from 'lucide-react'
import { toast } from 'sonner'
import { getTranslatedStringOrDefault } from '@/lib/translated-string'
import { downloadFile } from '@/lib/utils'
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
import { Card, CardAction, CardContent, CardHeader } from '@/components/ui/card'
import {
  Collapsible,
  CollapsibleContent,
  CollapsibleTrigger,
} from '@/components/ui/collapsible'
import {
  Item,
  ItemActions,
  ItemContent,
  ItemDescription,
  ItemGroup,
  ItemMedia,
  ItemTitle,
} from '@/components/ui/item'
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
  const { formLanguage } = Route.useSearch()

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

function Page() {
  const { electionRoundId, submissionId } = Route.useParams()
  const { formLanguage, from } = Route.useSearch()
  const { invalidate } = useRouter()
  const { data: submission } = useSuspenseGetFormSubmissionDetails(
    electionRoundId,
    submissionId
  )
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
                to='/elections/$electionRoundId/submissions/by-form'
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
        <CardHeader>
          <CardAction>
            <Button variant='outline'>
              <EditIcon className='size-4' />
              Edit Submission
            </Button>
          </CardAction>
        </CardHeader>
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
            {/* <Item>
              <ItemContent>
                <ItemTitle>Incident category</ItemTitle>
                <ItemDescription>
                  {mapQuickReportIncidentCategory(submission.incidentCategory)}
                </ItemDescription>
              </ItemContent>
            </Item>
            <Item>
              <ItemContent>
                <ItemTitle>Location type</ItemTitle>
                <ItemDescription>
                  {mapQuickReportLocationType(
                    submission.quickReportLocationType
                  )}
                </ItemDescription>
              </ItemContent>
            </Item> */}
            <Item>
              <ItemContent>
                <ItemTitle>Form name</ItemTitle>
                <ItemDescription>
                  {getTranslatedStringOrDefault(
                    submission.formName,
                    submission.defaultLanguage,
                    formLanguage
                  )}
                </ItemDescription>
              </ItemContent>
            </Item>

            <PollingStationDetails submission={submission} />

            {/* 
            
            <Item>
              <ItemContent>
                <ItemTitle>Description</ItemTitle>
                <ItemDescription>{submission.description}</ItemDescription>
              </ItemContent>
            </Item> */}
          </ItemGroup>
          <ItemGroup>
            <Item>
              <ItemContent>
                <ItemTitle>Attachments</ItemTitle>
              </ItemContent>
            </Item>
            <Collapsible>
              <CollapsibleTrigger asChild>
                <Button variant='outline' className='mb-2 w-fit'>
                  {submission.attachments.length > 0
                    ? `Show Attachments (${submission.attachments.length})`
                    : 'No Attachments'}
                </Button>
              </CollapsibleTrigger>
              <CollapsibleContent>
                <div className='mt-2 flex flex-col gap-2'>
                  {submission.attachments.map((attachment, index) => (
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
                      <ItemContent className='gap-1'>
                        <ItemTitle>{attachment.fileName}</ItemTitle>
                        <ItemDescription>{attachment.mimeType}</ItemDescription>
                      </ItemContent>
                      <ItemActions>
                        <Button
                          variant='outline'
                          size='icon'
                          onClick={() =>
                            downloadFile(
                              attachment.presignedUrl,
                              attachment.fileName
                            )
                          }
                        >
                          <DownloadIcon className='size-4' />
                        </Button>
                      </ItemActions>
                    </Item>
                  ))}
                </div>
              </CollapsibleContent>
            </Collapsible>
          </ItemGroup>
        </CardContent>
      </Card>
    </>
  )
}

export default Page
