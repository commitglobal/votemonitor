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
import { NgoStatusBadge } from '@/features/ngos/components/NgoStatusBadges';
import { EllipsisVerticalIcon } from '@heroicons/react/24/solid';
import type { ColumnDef } from '@tanstack/react-table';
import { useTranslation } from 'react-i18next';
import { MonitoringNgoModel } from '../../models/types';
import { useMonitoringNgos } from './queries';
export interface MonitoringNgosDashboardProps {
  electionRoundId: string;
}
function MonitoringNgosDashboard({ electionRoundId }: MonitoringNgosDashboardProps) {
  const { t } = useTranslation();

  const ngoAdminsColDefs: ColumnDef<MonitoringNgoModel>[] = [
    {
      accessorKey: 'name',
      enableSorting: true,
      header: ({ column }) => <DataTableColumnHeader title='Name' column={column} />,
    },

    {
      accessorKey: 'status',
      enableSorting: false,
      header: ({ column }) => <DataTableColumnHeader title='Status' column={column} />,
      cell: ({
        row: {
          original: { status },
        },
      }) => {
        return <NgoStatusBadge status={status} />;
      },
    },
    {
      id: 'actions',
      cell: ({ row }) => {
        const ngoId = row.original.id;

        return (
          <div className='text-right'>
            <DropdownMenu>
              <DropdownMenuTrigger asChild>
                <Button variant='ghost-primary' size='icon'>
                  <span className='sr-only'>Actions</span>
                  <EllipsisVerticalIcon className='w-6 h-6' />
                </Button>
              </DropdownMenuTrigger>
              <DropdownMenuContent align='end'>
                <DropdownMenuItem
                  className='text-red-600'
                  onClick={async (e) => {
                    e.stopPropagation();
                  }}>
                  Delete
                </DropdownMenuItem>
              </DropdownMenuContent>
            </DropdownMenu>
          </div>
        );
      },
    },
  ];

  return (
    <Card>
      <CardHeader className='flex gap-2 flex-column'>
        <div className='flex flex-row items-center justify-between'>
          <CardTitle className='text-2xl font-semibold leading-none tracking-tight'>
            {t('electionEvent.monitoringNgos.cardTitle')}
          </CardTitle>
        </div>
        <Separator />
      </CardHeader>
      <CardContent className='flex flex-col items-baseline gap-6'>
        <QueryParamsDataTable columns={ngoAdminsColDefs} useQuery={() => useMonitoringNgos(electionRoundId)} />
      </CardContent>
    </Card>
  );
}

export default MonitoringNgosDashboard;
