import { Button } from '@/components/ui/button';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { DataTableColumnHeader } from '@/components/ui/DataTable/DataTableColumnHeader';
import { QueryParamsDataTable } from '@/components/ui/DataTable/QueryParamsDataTable';
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from '@/components/ui/dropdown-menu';
import { Separator } from '@/components/ui/separator';
import { useDialog } from '@/components/ui/use-dialog';
import { DocumentTextIcon, EllipsisVerticalIcon, LinkIcon, PaperClipIcon } from '@heroicons/react/24/outline';
import { ColumnDef, createColumnHelper } from '@tanstack/react-table';
import { format } from 'date-fns';

import { DateTimeFormat } from '@/common/formats';
import { ElectionRoundStatus } from '@/common/types';
import { useConfirm } from '@/components/ui/alert-dialog-provider';
import { Tooltip, TooltipContent, TooltipProvider, TooltipTrigger } from '@/components/ui/tooltip';
import { useCurrentElectionRoundStore } from '@/context/election-round.store';
import i18n from '@/i18n';
import { queryClient } from '@/main';
import API from '@/services/api';
import { useMutation } from '@tanstack/react-query';
import { Link, useNavigate } from '@tanstack/react-router';
import { ChevronDown } from 'lucide-react';
import { useMemo, useState } from 'react';
import { toast } from 'sonner';
import { citizenGuidesKeys, useCitizenGuides } from '../../hooks/citizen-guides-hooks';
import { useElectionRoundDetails } from '../../hooks/election-event-hooks';
import { observerGuidesKeys, useObserverGuides } from '../../hooks/observer-guides-hooks';
import { GuideModel, GuidePageType, GuideType } from '../../models/guide';
import AddGuideDialog from './AddGuideDialog';
import EditGuideAccessDialog, { useEditGuideAccessDialog } from './EditGuideAccessDialog';
import EditGuideDialog from './EditGuideDialog';

export interface GuidesDashboardProps {
  guidePageType: GuidePageType;
}
export default function GuidesDashboard({ guidePageType }: GuidesDashboardProps) {
  const addGuideDialog = useDialog();
  const editGuideDialog = useDialog();
  const editGuideAccessDialog = useEditGuideAccessDialog();

  const navigate = useNavigate();

  const confirm = useConfirm();
  const [guide, setGuide] = useState<GuideModel | undefined>(undefined);
  const [newGuideType, setNewGuideType] = useState<GuideType | undefined>(undefined);

  const currentElectionRoundId = useCurrentElectionRoundStore((s) => s.currentElectionRoundId);
  const { data: electionRound } = useElectionRoundDetails(currentElectionRoundId);

  async function handleDeleteGuide(guideId: string, guideName: string): Promise<void> {
    if (
      await confirm({
        title: `Delete ${guideName} ?`,
        actionButton: 'Delete',
        cancelButton: 'Cancel',
        body: <>Are you sure you want to delete this guide? This action cannot be undone</>,
      })
    ) {
      deleteGuideMutation.mutate({ guidePageType, electionRoundId: currentElectionRoundId, guideId });
    }
  }

  const deleteGuideMutation = useMutation({
    mutationFn: ({
      guidePageType,
      electionRoundId,
      guideId,
    }: {
      guidePageType: GuidePageType;
      electionRoundId: string;
      guideId: string;
    }) => {
      const url =
        guidePageType === GuidePageType.Observer
          ? `/election-rounds/${electionRoundId}/observer-guide/${guideId}`
          : `/election-rounds/${electionRoundId}/citizen-guides/${guideId}`;
      return API.delete<void>(url);
    },

    onSuccess: (_, { electionRoundId, guidePageType }) => {
      if (guidePageType === GuidePageType.Observer) {
        queryClient.invalidateQueries({ queryKey: observerGuidesKeys.all(electionRoundId) });
      }

      queryClient.invalidateQueries({ queryKey: citizenGuidesKeys.all(electionRoundId) });

      toast.success('Delete was successful');
    },

    onError: () => {
      toast.error('Error deleting guide', {
        description: 'Please contact Platform admins',
      });
    },
  });
  const columnHelper = createColumnHelper<GuideModel>();

  const guidesColDefs: ColumnDef<GuideModel>[] = useMemo(() => {
    const defaultColumns: ColumnDef<GuideModel>[] = [
      {
        header: '',
        accessorKey: 'guideType',
        enableSorting: false,
        enableGlobalFilter: false,
        maxSize: 25,
        minSize: 25,
        size: 16,
        cell: ({
          row: {
            original: { guideType },
          },
        }) => (
          <div className='w-[30px]'>
            {guideType === GuideType.Document ? (
              <PaperClipIcon className='w-4 h-4 opacity-50' />
            ) : guideType === GuideType.Website ? (
              <LinkIcon className='w-4 h-4 opacity-50' />
            ) : (
              <DocumentTextIcon className='w-4 h-4 opacity-50' />
            )}
          </div>
        ),
      },
      {
        header: ({ column }) => (
          <DataTableColumnHeader title={i18n.t('electionEvent.guides.headers.title')} column={column} />
        ),
        accessorKey: 'title',
        enableSorting: false,
        enableGlobalFilter: false,
        cell: ({
          row: {
            original: { id, guideType, title, websiteUrl, presignedUrl },
          },
        }) =>
          guideType === GuideType.Website || guideType === GuideType.Document ? (
            <Button type='button' variant='link'>
              <Link to={websiteUrl || presignedUrl} target='_blank' preload={false}>
                {title}
              </Link>
            </Button>
          ) : (
            <Link
              to={
                guidePageType === GuidePageType.Observer
                  ? '/observer-guides/view/$guideId'
                  : '/citizen-guides/view/$guideId'
              }
              params={{ guideId: id }}
              preload={false}>
              {title}
            </Link>
          ),
      },
      {
        header: ({ column }) => (
          <DataTableColumnHeader title={i18n.t('electionEvent.guides.headers.uploadedOn')} column={column} />
        ),
        accessorKey: 'createdOn',
        enableSorting: false,
        enableGlobalFilter: false,
        cell: ({
          row: {
            original: { createdOn },
          },
        }) => <p>{format(createdOn, DateTimeFormat)}</p>,
      },
      {
        header: ({ column }) => (
          <DataTableColumnHeader title={i18n.t('electionEvent.guides.headers.createdBy')} column={column} />
        ),
        accessorKey: 'createdBy',
        enableSorting: false,
        enableGlobalFilter: false,
      },
    ];

    if (electionRound?.isCoalitionLeader) {
      if (guidePageType === GuidePageType.Observer) {
        defaultColumns.push(
          columnHelper.display({
            id: 'sharedWith',
            enableSorting: false,
            header: ({ column }) => <DataTableColumnHeader title='Shared with' column={column} />,
            cell: ({ row }) =>
              row.depth === 0 ? (
                row.original.guideAccess.length ? (
                  <TooltipProvider delayDuration={100}>
                    <Tooltip>
                      <TooltipTrigger asChild>
                        <span className='underline cursor-pointer decoration-dashed hover:decoration-solid'>
                          {row.original.guideAccess.length}
                        </span>
                      </TooltipTrigger>
                      <TooltipContent>
                        {row.original.guideAccess.map((ga) => (
                          <div key={ga.ngoId}>{ga.name}</div>
                        ))}
                      </TooltipContent>
                    </Tooltip>
                  </TooltipProvider>
                ) : (
                  <>None</>
                )
              ) : null,
          })
        );
      }

      defaultColumns.push({
        header: '',
        accessorKey: 'action',
        enableSorting: false,
        cell: ({ row }) => (
          <DropdownMenu>
            <DropdownMenuTrigger>
              <EllipsisVerticalIcon className='w-[24px] h-[24px] tex t-purple-600' />
            </DropdownMenuTrigger>
            <DropdownMenuContent>
              <DropdownMenuItem
                disabled={!row.original.isGuideOwner}
                onClick={() => {
                  if (row.original.guideType !== GuideType.Text) {
                    setGuide(row.original);
                    editGuideDialog.trigger();
                  } else {
                    navigate({
                      to:
                        guidePageType === GuidePageType.Observer
                          ? '/observer-guides/edit/$guideId'
                          : '/citizen-guides/edit/$guideId',
                      params: { guideId: row.original.id },
                    });
                  }
                }}>
                Update
              </DropdownMenuItem>
              {guidePageType === GuidePageType.Observer ? (
                <DropdownMenuItem
                  disabled={electionRound?.status === ElectionRoundStatus.Archived}
                  onClick={() => editGuideAccessDialog.trigger(row.original.id)}>
                  Guide access
                </DropdownMenuItem>
              ) : null}
              <DropdownMenuItem
                className='text-red-600'
                disabled={!row.original.isGuideOwner}
                onClick={async () => await handleDeleteGuide(row.original.id, row.original.title)}>
                Delete guide
              </DropdownMenuItem>
            </DropdownMenuContent>
          </DropdownMenu>
        ),
      });
    } else {
      defaultColumns.push({
        header: '',
        accessorKey: 'action',
        enableSorting: false,
        cell: ({ row }) => (
          <DropdownMenu>
            <DropdownMenuTrigger>
              <EllipsisVerticalIcon className='w-[24px] h-[24px] tex t-purple-600' />
            </DropdownMenuTrigger>
            <DropdownMenuContent>
              <DropdownMenuItem
                disabled={!row.original.isGuideOwner}
                onClick={() => {
                  if (row.original.guideType !== GuideType.Text) {
                    setGuide(row.original);
                    editGuideDialog.trigger();
                  } else {
                    navigate({
                      to:
                        guidePageType === GuidePageType.Observer
                          ? '/observer-guides/edit/$guideId'
                          : '/citizen-guides/edit/$guideId',
                      params: { guideId: row.original.id },
                    });
                  }
                }}>
                Update
              </DropdownMenuItem>
              <DropdownMenuItem
                className='text-red-600'
                disabled={!row.original.isGuideOwner}
                onClick={async () => await handleDeleteGuide(row.original.id, row.original.title)}>
                Delete guide
              </DropdownMenuItem>
            </DropdownMenuContent>
          </DropdownMenu>
        ),
      });
    }
    return defaultColumns;
  }, [currentElectionRoundId, electionRound?.isCoalitionLeader]);

  return (
    <Card className='w-full pt-0'>
      <CardHeader className='flex gap-2 flex-column'>
        <div className='flex flex-row items-center justify-between pr-6'>
          <CardTitle className='text-2xl font-semibold leading-none tracking-tight'>
            {guidePageType === GuidePageType.Observer
              ? i18n.t('electionEvent.guides.observerGuidesCardTitle')
              : i18n.t('electionEvent.guides.citizenGuidesCardTitle')}
          </CardTitle>
          <DropdownMenu>
            <DropdownMenuTrigger asChild disabled={electionRound?.status === ElectionRoundStatus.Archived}>
              <Button className='bg-purple-900 hover:bg-purple-600'>
                <svg
                  className='mr-1.5'
                  xmlns='http://www.w3.org/2000/svg'
                  width='18'
                  height='18'
                  viewBox='0 0 18 18'
                  fill='none'>
                  <path
                    d='M3 12L3 12.75C3 13.9926 4.00736 15 5.25 15L12.75 15C13.9926 15 15 13.9926 15 12.75L15 12M12 6L9 3M9 3L6 6M9 3L9 12'
                    stroke='white'
                    strokeWidth='2'
                    strokeLinecap='round'
                    strokeLinejoin='round'
                  />
                </svg>
                {guidePageType === GuidePageType.Observer
                  ? i18n.t('electionEvent.guides.buttonUploadObserverGuide')
                  : i18n.t('electionEvent.guides.buttonUploadCitizenGuide')}
                <ChevronDown className='w-4 h-4 opacity-50' />
              </Button>
            </DropdownMenuTrigger>

            <DropdownMenuContent className='w-56'>
              <DropdownMenuItem
                className='flex flex-row gap-2'
                onClick={() => {
                  setNewGuideType(GuideType.Document);
                  addGuideDialog.trigger();
                }}>
                <PaperClipIcon className='w-4 h-4 opacity-50' />
                {i18n.t('electionEvent.guides.documentGuide')}
              </DropdownMenuItem>
              <DropdownMenuItem
                className='flex flex-row gap-2'
                onClick={() => {
                  setNewGuideType(GuideType.Website);
                  addGuideDialog.trigger();
                }}>
                <LinkIcon className='w-4 h-4 opacity-50' />
                {i18n.t('electionEvent.guides.urlGuide')}
              </DropdownMenuItem>
              <DropdownMenuItem
                className='flex flex-row gap-2'
                onClick={() => {
                  navigate({
                    to: guidePageType === GuidePageType.Observer ? '/observer-guides/new' : '/citizen-guides/new',
                  });
                }}>
                <DocumentTextIcon className='w-4 h-4 opacity-50' />
                {i18n.t('electionEvent.guides.textGuide')}
              </DropdownMenuItem>
            </DropdownMenuContent>
          </DropdownMenu>
        </div>
        <Separator />
      </CardHeader>
      <CardContent>
        <QueryParamsDataTable
          columns={guidesColDefs}
          useQuery={() =>
            guidePageType === GuidePageType.Observer
              ? useObserverGuides(currentElectionRoundId)
              : useCitizenGuides(currentElectionRoundId)
          }
        />
        {addGuideDialog.dialogProps.open && !!newGuideType && (
          <AddGuideDialog {...addGuideDialog.dialogProps} guideType={newGuideType} guidePageType={guidePageType} />
        )}
        {editGuideDialog.dialogProps.open && !!guide && (
          <EditGuideDialog {...editGuideDialog.dialogProps} guide={guide} guidePageType={guidePageType} />
        )}

        <EditGuideAccessDialog />
      </CardContent>
    </Card>
  );
}
