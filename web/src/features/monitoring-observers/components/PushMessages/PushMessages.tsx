import { Button } from '@/components/ui/button';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { DataTableColumnHeader } from '@/components/ui/DataTable/DataTableColumnHeader';
import { QueryParamsDataTable } from '@/components/ui/DataTable/QueryParamsDataTable';
import { Separator } from '@/components/ui/separator';
import { ChevronRightIcon } from '@heroicons/react/24/outline';
import { Link, useNavigate } from '@tanstack/react-router';
import type { CellContext, ColumnDef } from '@tanstack/react-table';
import { Plus } from 'lucide-react';

import { DateTimeFormat } from '@/common/formats';
import { ElectionRoundStatus, type FunctionComponent } from '@/common/types';
import type { TableCellProps } from '@/components/ui/DataTable/DataTable';
import { useCurrentElectionRoundStore } from '@/context/election-round.store';
import { format } from 'date-fns';
import { useCallback } from 'react';
import { usePushMessages } from '../../hooks/push-messages-queries';
import type { PushMessageModel } from '../../models/push-message';
import { useElectionRoundDetails } from '@/features/election-event/hooks/election-event-hooks';

function PushMessages(): FunctionComponent {
  const pushMessagesColDefs: ColumnDef<PushMessageModel>[] = [
    {
      header: ({ column }) => <DataTableColumnHeader title='Sent at' column={column} />,
      accessorKey: 'sentAt',
      enableSorting: false,
      enableGlobalFilter: false,
      cell: ({ row }) => <div>{format(row.original.sentAt, DateTimeFormat)}</div>,
    },
    {
      header: ({ column }) => <DataTableColumnHeader title='Sender name' column={column} />,
      accessorKey: 'sender',
      enableSorting: false,
      enableGlobalFilter: false,
    },
    {
      header: ({ column }) => <DataTableColumnHeader title='Targeted Observers' column={column} />,
      accessorKey: 'numberOfTargetedObservers',
      enableSorting: false,
      enableGlobalFilter: false,
    },
    {
      header: ({ column }) => <DataTableColumnHeader title='Read notifications' column={column} />,
      accessorKey: 'numberOfReadNotifications',
      enableSorting: false,
      enableGlobalFilter: false,
    },
    {
      header: ({ column }) => <DataTableColumnHeader title='Title' column={column} />,
      accessorKey: 'title',
      enableSorting: false,
      enableGlobalFilter: false,
    },
    {
      header: '',
      accessorKey: 'action',
      enableSorting: false,
      cell: ({ row }) => (
        <div className='text-right'>
          <Link
            className='hover:bg-purple-100 inline-flex h-6 w-6 rounded-full items-center justify-center'
            params={{ id: row.original.id }}
            to='/monitoring-observers/push-messages/$id/view'>
            <ChevronRightIcon className='w-4 text-purple-600' />
          </Link>
        </div>
      ),
    },
  ];

  const getCellProps = (context: CellContext<PushMessageModel, unknown>): TableCellProps | void => {
    if (context.column.id === 'body' || context.column.id === 'title') {
      return {
        className: 'truncate hover:text-clip',
      };
    }
  };

  const navigate = useNavigate();
  const currentElectionRoundId = useCurrentElectionRoundStore((s) => s.currentElectionRoundId);
  const { data: electionRound } = useElectionRoundDetails(currentElectionRoundId);

  const navigateToPushMessage = useCallback(
    (id: string) => {
      navigate({ to: '/monitoring-observers/push-messages/$id/view', params: { id } });
    },
    [navigate]
  );

  return (
    <Card className='w-full pt-0'>
      <CardHeader className='flex flex-column gap-2'>
        <div className='flex flex-row justify-between items-center pr-6'>
          <CardTitle className='text-2xl font-semibold leading-none tracking-tight'>Push messages</CardTitle>
          <div className='table-actions flex flex-row-reverse flex-row- gap-4'>
            <Link
              to='/monitoring-observers/create-new-message'
              search={{ pageSize: 25 }}
              disabled={electionRound?.status === ElectionRoundStatus.Archived}>
              <Button disabled={electionRound?.status === ElectionRoundStatus.Archived}>
                <Plus className='mr-2' width={18} height={18} />
                Create new message
              </Button>
            </Link>
          </div>
        </div>
        <Separator />
      </CardHeader>
      <CardContent>
        <QueryParamsDataTable
          columns={pushMessagesColDefs}
          useQuery={(params) => usePushMessages(currentElectionRoundId, params)}
          onRowClick={navigateToPushMessage}
          getCellProps={getCellProps}
          emptySubtitle='Communicate instantly with observers by creating and sending push messages directly to their mobile app.'
          emptyTitle='No push messages created yet '
        />
      </CardContent>
    </Card>
  );
}

export default PushMessages;
