import { format } from 'date-fns'
import { Link } from '@tanstack/react-router'
import type { ColumnDef } from '@tanstack/react-table'
import { ElectionRoundStatus } from '@/types/election'
import { FormModel, FormStatus } from '@/types/form'
import { ChevronRightIcon, EllipsisVerticalIcon } from 'lucide-react'
import { mapFormType, mapLanguageNameByCode } from '@/lib/i18n'
import { DateTimeFormat } from '@/constants/formats'
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from '@/components/ui/dropdown-menu'
import {
  Tooltip,
  TooltipContent,
  TooltipTrigger,
} from '@/components/ui/tooltip'
import { DataTableColumnHeader } from '@/components/data-table-column-header'
import FormStatusBadge from '@/components/from-status-badge'

interface GetFormsTableColumnsProps {
  electionRoundId: string
  electionStatus: ElectionRoundStatus | undefined
}

export function getFormsTableColumns({
  electionRoundId,
  electionStatus,
}: GetFormsTableColumnsProps): ColumnDef<FormModel>[] {
  return [
    {
      header: ({ column }) => (
        <DataTableColumnHeader title='Form code' column={column} />
      ),
      accessorFn: (row) => row.code,
      id: 'code',
      enableSorting: true,
      enableGlobalFilter: true,
      meta: {
        label: 'Form code',
      },
    },
    {
      header: ({ column }) => (
        <DataTableColumnHeader title='Form name' column={column} />
      ),
      id: 'name',
      cell: ({ row }) => (
        <div>{row.original.name[row.original.defaultLanguage] ?? '-'}</div>
      ),

      enableSorting: true,
      enableGlobalFilter: true,

      meta: {
        label: 'Form name',
      },
    },
    {
      header: ({ column }) => (
        <DataTableColumnHeader title='Form type' column={column} />
      ),
      accessorFn: (row) => row.formType,
      id: 'formType',
      enableSorting: true,
      enableGlobalFilter: true,
      cell: ({ row }) => <div>{mapFormType(row.original.formType)}</div>,

      meta: {
        label: 'Form type',
      },
    },
    {
      header: ({ column }) => (
        <DataTableColumnHeader title='Language' column={column} />
      ),
      accessorFn: (row) => row.defaultLanguage,
      id: 'defaultLanguage',
      enableSorting: false,
      enableGlobalFilter: true,
      cell: ({ row }) => (
        <div>{mapLanguageNameByCode(row.original.defaultLanguage)}</div>
      ),

      meta: {
        label: 'Language',
      },
    },

    {
      header: ({ column }) => (
        <DataTableColumnHeader title='# of questions' column={column} />
      ),
      accessorFn: (row) => row.numberOfQuestions,
      id: 'numberOfQuestions',
      enableSorting: true,
      enableGlobalFilter: true,
      cell: ({ row }) => <div>{row.original.numberOfQuestions}</div>,

      meta: {
        label: '# of questions',
      },
    },

    {
      header: ({ column }) => (
        <DataTableColumnHeader title='Status' column={column} />
      ),
      accessorFn: (row) => row.status,
      id: 'status',
      enableSorting: true,
      enableGlobalFilter: true,
      cell: ({ row }) => <FormStatusBadge formStatus={row.original.status} />,

      meta: {
        label: 'Issue title',
      },
    },
    {
      header: ({ column }) => (
        <DataTableColumnHeader title='Last updated on' column={column} />
      ),
      accessorFn: (row) => row.lastModifiedOn,
      id: 'lastModifiedOn',
      enableSorting: true,
      enableGlobalFilter: true,
      size: 200,
      cell: ({ row }) => (
        <Tooltip>
          <TooltipTrigger asChild>
            <div
              className='cursor-pointer hover:underline'
              title={row.original.lastModifiedBy}
            >
              {format(row.original.lastModifiedOn, DateTimeFormat)}
            </div>
          </TooltipTrigger>
          <TooltipContent className='max-w-md break-words whitespace-pre-wrap'>
            {row.original.lastModifiedBy}
          </TooltipContent>
        </Tooltip>
      ),

      meta: {
        label: 'Last updated on',
      },
    },

    {
      header: '',
      id: 'actions',
      enableSorting: false,
      cell: ({ row }) => (
        <DropdownMenu modal={false}>
          <DropdownMenuTrigger asChild>
            <EllipsisVerticalIcon className='tex t-purple-600 h-[24px] w-[24px]' />
          </DropdownMenuTrigger>
          <DropdownMenuContent>
            <DropdownMenuItem asChild>
              <Link
                to={`/elections/$electionRoundId/forms/$formId`}
                params={{ electionRoundId, formId: row.original.id }}
              >
                View
              </Link>
            </DropdownMenuItem>
            {/* {row.depth === 0 && row.original.status === FormStatus.Published ? (
              <DropdownMenuItem
                disabled={electionStatus === ElectionRoundStatus.Archived}
                onClick={() => editFormAccessDialog.trigger(row.original.id)}
              >
                Form access
              </DropdownMenuItem>
            ) : null} */}
            {row.depth === 0 ? (
              <DropdownMenuItem
                disabled={
                  row.original.status !== FormStatus.Drafted ||
                  electionStatus === ElectionRoundStatus.Archived
                }
              >
                <Link
                  to={`/elections/$electionRoundId/forms/$formId/edit/$languageCode`}
                  params={{
                    electionRoundId,
                    formId: row.original.id,
                    languageCode: row.original.defaultLanguage,
                  }}
                >
                  Edit
                </Link>
              </DropdownMenuItem>
            ) : (
              <DropdownMenuItem
                disabled={
                  row.original.status !== FormStatus.Drafted ||
                  electionStatus === ElectionRoundStatus.Archived
                }
              >
                <Link
                  to={`/elections/$electionRoundId/forms/$formId/edit/$languageCode`}
                  params={{
                    electionRoundId,
                    formId: row.original.id,
                    languageCode: row.original.defaultLanguage,
                  }}
                >
                  Edit
                </Link>
              </DropdownMenuItem>
            )}

            {row.depth === 0 && row.original.status === FormStatus.Published ? (
              <DropdownMenuItem
                disabled={electionStatus === ElectionRoundStatus.Archived}
                // onClick={() => handleObsoleteForm(row.original)}
              >
                Obsolete
              </DropdownMenuItem>
            ) : null}
            {row.depth === 0 && row.original.status === FormStatus.Drafted ? (
              <DropdownMenuItem
                disabled={electionStatus === ElectionRoundStatus.Archived}
                // onClick={() => handlePublishForm(row.original)}
              >
                Publish
              </DropdownMenuItem>
            ) : null}
            {row.depth === 0 ? (
              <DropdownMenuItem
                disabled={electionStatus === ElectionRoundStatus.Archived}
                // onClick={() => handleDuplicateForm(row.original)}
              >
                Duplicate
              </DropdownMenuItem>
            ) : null}
            {/* {row.depth === 0 ? (
              <DropdownMenuItem
                className='text-red-600'
                disabled={electionStatus === ElectionRoundStatus.Archived}
                onClick={async () => {
                  if (
                    await confirm({
                      title: `Delete form ${row.original.code}?`,
                      body:
                        row.original.status === FormStatus.Published ? (
                          <>
                            Please note that this form is published and may
                            contain associated data. Deleting this form could
                            result in the loss of any submitted answers from
                            your observers. Once deleted,
                            <b>the associated data cannot be retrieved</b>
                          </>
                        ) : (
                          'This action is permanent and cannot be undone. Once deleted, this form cannot be retrieved.'
                        ),
                      actionButton: 'Delete',
                      actionButtonClass: buttonVariants({
                        variant: 'destructive',
                      }),
                      cancelButton: 'Cancel',
                    })
                  ) {
                    deleteFormMutation.mutate({
                      electionRoundId: currentElectionRoundId,
                      formId: row.original.id,
                    })
                  }
                }}
              >
                Delete form
              </DropdownMenuItem>
            ) : (
              <DropdownMenuItem
                className='text-red-600'
                disabled={electionStatus === ElectionRoundStatus.Archived}
                onClick={async () => {
                  const languageCode = row.original.defaultLanguage
                  const language = appLanguages?.find(
                    (l) => languageCode === l.code
                  )
                  const fullName = language
                    ? `${language.name} / ${language.nativeName}`
                    : ''

                  if (
                    await confirm({
                      title: `Delete translation ${fullName}?`,
                      body: 'This action is permanent and cannot be undone. Once deleted, this translation cannot be retrieved.',
                      actionButton: 'Delete',
                      actionButtonClass: buttonVariants({
                        variant: 'destructive',
                      }),
                      cancelButton: 'Cancel',
                    })
                  ) {
                    deleteTranslationMutation.mutate({
                      electionRoundId: currentElectionRoundId,
                      formId: row.original.id,
                      languageCode,
                    })
                  }
                }}
              >
                Delete translation
              </DropdownMenuItem>
            )} */}
          </DropdownMenuContent>
        </DropdownMenu>
      ),
    },
  ]
}
