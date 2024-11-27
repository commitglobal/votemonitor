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
import { useCitizenNotifications } from '../hooks/citizen-notifications-queries';
import { CitizenNotificationModel } from '../models/citizen-notification';
import { useElectionRoundDetails } from '@/features/election-event/hooks/election-event-hooks';

function CitizenNotificationsDashboard(): FunctionComponent {
  const pushMessagesColDefs: ColumnDef<CitizenNotificationModel>[] = [
    {
      header: ({ column }) => <DataTableColumnHeader title='Sent at' column={column} />,
      accessorFn: (cn) => cn.sentAt,
      id: 'sentAt',
      enableSorting: false,
      enableGlobalFilter: false,
      cell: ({ row }) => <div>{format(row.original.sentAt, DateTimeFormat)}</div>,
    },
    {
      header: ({ column }) => <DataTableColumnHeader title='Sender name' column={column} />,
      accessorFn: (cn) => cn.sender,
      id: 'sender',
      enableSorting: false,
      enableGlobalFilter: false,
    },
    {
      header: ({ column }) => <DataTableColumnHeader title='Title' column={column} />,
      accessorFn: (cn) => cn.title,
      id: 'title',
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
            className='inline-flex items-center justify-center w-6 h-6 rounded-full hover:bg-purple-100'
            params={{ notificationId: row.original.id }}
            to='/citizen-notifications/view/$notificationId'>
            <ChevronRightIcon className='w-4 text-purple-600' />
          </Link>
        </div>
      ),
    },
  ];

  const getCellProps = (context: CellContext<CitizenNotificationModel, unknown>): TableCellProps | void => {
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
    (notificationId: string) => {
      void navigate({ to: '/citizen-notifications/view/$notificationId', params: { notificationId } });
    },
    [navigate]
  );

  return (
    <Card className='w-full pt-0'>
      <CardHeader className='flex gap-2 flex-column'>
        <div className='flex flex-row items-center justify-between px-6'>
          <CardTitle className='text-xl'>Push messages</CardTitle>
          <div className='flex flex-row-reverse gap-4 table-actions flex-row-'>
            <Link to='/citizen-notifications/new' disabled={electionRound?.status === ElectionRoundStatus.Archived}>
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
          useQuery={(params) => useCitizenNotifications(currentElectionRoundId, params)}
          onRowClick={navigateToPushMessage}
          getCellProps={getCellProps}
          emptySubtitle='Communicate instantly with citizens by creating and sending push messages directly to their mobile app.'
          emptyTitle='No push messages created yet '
        />
      </CardContent>
    </Card>
  );
}

export default CitizenNotificationsDashboard;
