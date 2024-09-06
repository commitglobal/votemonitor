import { Button } from '@/components/ui/button';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { DataTableColumnHeader } from '@/components/ui/DataTable/DataTableColumnHeader';
import { QueryParamsDataTable } from '@/components/ui/DataTable/QueryParamsDataTable';
import { DropdownMenu, DropdownMenuContent, DropdownMenuItem, DropdownMenuTrigger } from '@/components/ui/dropdown-menu';
import { Separator } from '@/components/ui/separator';
import { useDialog } from '@/components/ui/use-dialog';
import { EllipsisVerticalIcon, LinkIcon, PaperClipIcon } from '@heroicons/react/24/outline';
import { ColumnDef } from '@tanstack/react-table';
import { format } from 'date-fns';
import { useState } from 'react';

import { authApi } from '@/common/auth-api';
import { DateTimeFormat } from '@/common/formats';
import { toast } from '@/components/ui/use-toast';
import { queryClient } from '@/main';
import { useMutation } from '@tanstack/react-query';
import { Link } from '@tanstack/react-router';
import { useObserverGuides } from '../../hooks/election-event-hooks';
import { ObserverGuide } from '../../models/observer-guide';
import ConfirmDeleteDialog from './ConfirmDeleteDialog';
import EditObserversGuideDialog from './EditObserversGuideDialog';
import UploadObserversGuideDialog from './UploadObserversGuideDialog';
import { useCurrentElectionRoundStore } from '@/context/election-round.store';
import { t } from 'i18next';

export default function ObserversGuides() {
  const uploadObserverGuideDialog = useDialog();
  const editObserverGuideDialog = useDialog();
  const confirmDeleteDialog = useDialog();

  const [guideId, setGuideId] = useState<string | undefined>(undefined);
  const [guideTitle, setGuideTitle] = useState<string | undefined>(undefined);
  const currentElectionRoundId = useCurrentElectionRoundStore(s => s.currentElectionRoundId);

  function handleDeleteGuide(guideId: string): void {
    setGuideId(guideId);
    confirmDeleteDialog.trigger();
  }

  function handleUpdateTitle(guideId: string, title: string): void {
    setGuideId(guideId);
    setGuideTitle(title);

    editObserverGuideDialog.trigger();
  }

  const deleteObserverGuideMutation = useMutation({
    mutationFn: ({ electionRoundId }: { electionRoundId: string }) => {

      return authApi.delete<void>(`/election-rounds/${electionRoundId}/observer-guide/${guideId}`);
    },

    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['observer-guides'] });

      toast({
        title: 'Success',
        description: 'Delete was successful',
      });
    },

    onError: () => {
      toast({
        title: 'Error deleting observer guide',
        description: 'Please contact Platform admins',
        variant: 'destructive'
      });
    }
  });


  const observerGuideColDefs: ColumnDef<ObserverGuide>[] = [
    {
      header: '',
      accessorKey: 'guideType',
      enableSorting: false,
      enableGlobalFilter: false,
      maxSize: 25,
      size: 10,
      cell: ({
        row: {
          original: { guideType },
        },
      }) => <div>{guideType === 'Document' ? <PaperClipIcon className='w-4 h-4 ml-auto opacity-50' /> : <LinkIcon className='w-4 h-4 ml-auto opacity-50' />}</div>,
    },
    {
      header: ({ column }) => <DataTableColumnHeader title={t('electionEvent.observerGuides.dataTableColumnHeaderTitle')} column={column} />,
      accessorKey: 'title',
      enableSorting: false,
      enableGlobalFilter: false,
      cell: ({
        row: {
          original: { title, websiteUrl, presignedUrl },
        },
      }) => (<Button type='button' variant='link'><Link
        to={websiteUrl || presignedUrl}
        target='_blank'
        preload={false}>{title}</Link>
      </Button>)
    },
    {
      header: ({ column }) => <DataTableColumnHeader title={t('electionEvent.observerGuides.dataTableColumnHeaderUploadedOn')} column={column} />,
      accessorKey: 'createdOn',
      enableSorting: false,
      enableGlobalFilter: false,
      cell: ({
        row: {
          original: { createdOn },
        },
      }) => <p>{format(createdOn, DateTimeFormat)}</p>
    },
    {
      header: ({ column }) => <DataTableColumnHeader title={t('electionEvent.observerGuides.dataTableColumnHeaderCreatedBy')} column={column} />,
      accessorKey: 'createBy',
      enableSorting: false,
      enableGlobalFilter: false,
      cell: ({
        row: {
          original: { createdBy },
        },
      }) => <p>{createdBy}</p>
    },
    {
      header: '',
      accessorKey: 'action',
      enableSorting: false,
      cell: ({ row }) => (
        <DropdownMenu>
          <DropdownMenuTrigger>
            <EllipsisVerticalIcon className='w-[24px] h-[24px] tex t-purple-600' />
          </DropdownMenuTrigger>
          <DropdownMenuContent>
            <DropdownMenuItem onClick={() => handleUpdateTitle(row.original.id, row.original.title)}>Update title</DropdownMenuItem>
            <DropdownMenuItem className='text-red-600' onClick={() => handleDeleteGuide(row.original.id)}>
              Delete guide
            </DropdownMenuItem>
          </DropdownMenuContent>
        </DropdownMenu>)
    }
  ];

  return (
    <Card className='w-full pt-0'>
      <CardHeader className='flex flex-column gap-2'>
        <div className='flex flex-row justify-between items-center px-6'>
          <CardTitle className='text-xl'>{t('electionEvent.observerGuides.cardTitle')}</CardTitle>
          <div className='table-actions flex flex-row-reverse flex-row- gap-4'>
            {!!guideTitle && !!guideId && <EditObserversGuideDialog guideId={guideId} title={guideTitle} {...editObserverGuideDialog.dialogProps} />}
            <UploadObserversGuideDialog {...uploadObserverGuideDialog.dialogProps} />
            {!!guideId && <ConfirmDeleteDialog
              alertTitle={'Delete ?'}
              alertDescription={'Are you sure you want to delete this resource ?'}
              cancelActionButtonText='Cancel'
              confirmActionButtonText='Delete guide'
              onConfirm={() => deleteObserverGuideMutation.mutate({ electionRoundId: currentElectionRoundId })}
              {...confirmDeleteDialog.dialogProps} />}
            <Button className='bg-purple-900 hover:bg-purple-600' onClick={() => uploadObserverGuideDialog.trigger()}>
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
              {t('electionEvent.observerGuides.buttonUploadObserverGuide')}
            </Button>

          </div>
        </div>
        <Separator />

      </CardHeader>
      <CardContent>
        <QueryParamsDataTable columns={observerGuideColDefs} useQuery={() => useObserverGuides(currentElectionRoundId)} />
      </CardContent>
    </Card>
  );
}

