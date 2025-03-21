import { authApi } from '@/common/auth-api';
import type { FunctionComponent } from '@/common/types';
import { useConfirm } from '@/components/ui/alert-dialog-provider';
import { Button, buttonVariants } from '@/components/ui/button';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { DataTableColumnHeader } from '@/components/ui/DataTable/DataTableColumnHeader';
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from '@/components/ui/dropdown-menu';
import { Separator } from '@/components/ui/separator';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '@/components/ui/table';
import { useDialog } from '@/components/ui/use-dialog';
import { toast } from '@/components/ui/use-toast';
import { NgoStatusBadge } from '@/features/ngos/components/NgoStatusBadges';
import { EllipsisVerticalIcon } from '@heroicons/react/24/solid';
import { useMutation, useQueryClient } from '@tanstack/react-query';
import type { ColumnDef } from '@tanstack/react-table';
import { flexRender, getCoreRowModel, getSortedRowModel, useReactTable } from '@tanstack/react-table';
import { Plus } from 'lucide-react';
import { useTranslation } from 'react-i18next';
import { MonitoringNgoModel } from '../../models/types';
import AddMonitoringNgoDialog from './AddMonitoringNgoDialog';
import { monitoringNgoKeys, useMonitoringNgos } from './queries';

type MonitoringNgosTableProps = {
  columns: ColumnDef<MonitoringNgoModel>[];
  data: MonitoringNgoModel[];
};

function MonitoringNgosTable({ columns, data }: MonitoringNgosTableProps): FunctionComponent {
  const table = useReactTable({
    columns,
    data,
    getCoreRowModel: getCoreRowModel(),
    getSortedRowModel: getSortedRowModel(),
  });

  const rows = table.getRowModel().rows;

  return (
    <>
      <Table>
        <TableHeader>
          {table.getHeaderGroups().map((headerGroup) => (
            <TableRow key={headerGroup.id}>
              {headerGroup.headers.map((header) => {
                return (
                  <TableHead key={header.id} style={{ width: header.getSize() }}>
                    {header.isPlaceholder ? null : flexRender(header.column.columnDef.header, header.getContext())}
                  </TableHead>
                );
              })}
            </TableRow>
          ))}
        </TableHeader>

        <TableBody>
          {rows.length > 0 ? (
            table.getRowModel().rows.map((row) => (
              <TableRow key={row.id}>
                {row.getVisibleCells().map((cell) => (
                  <TableCell key={cell.id}>{flexRender(cell.column.columnDef.cell, cell.getContext())}</TableCell>
                ))}
              </TableRow>
            ))
          ) : (
            <TableRow>
              <TableCell className='h-24 text-center' colSpan={columns.length}>
                No results.
              </TableCell>
            </TableRow>
          )}
        </TableBody>
      </Table>
    </>
  );
}

export interface MonitoringNgosDashboardProps {
  electionRoundId: string;
}
function MonitoringNgosDashboard({ electionRoundId }: MonitoringNgosDashboardProps) {
  const { t } = useTranslation();
  const addMonitoringNgoDialog = useDialog();
  const { data } = useMonitoringNgos(electionRoundId);
  const queryClient = useQueryClient();
  const confirm = useConfirm();
  const deleteMonitoringNgoMutation = useMutation({
    mutationFn: async (ngoId: string) => {
      return await authApi.delete(`election-rounds/${electionRoundId}/monitoring-ngos/${ngoId}`);
    },

    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: monitoringNgoKeys.all(electionRoundId) });
      toast({
        title: 'Success',
        description: 'Removed monitoring NGO',
      });
    },
    //TODO Add error handling
  });

  const monitoringNgosColDefs: ColumnDef<MonitoringNgoModel>[] = [
    {
      accessorKey: 'name',
      enableSorting: true,
      header: ({ column }) => <DataTableColumnHeader title='Name' column={column} />,
    },

    {
      accessorKey: 'ngoStatus',
      enableSorting: false,
      header: ({ column }) => <DataTableColumnHeader title='Status' column={column} />,
      cell: ({
        row: {
          original: { ngoStatus },
        },
      }) => {
        return <NgoStatusBadge status={ngoStatus} />;
      },
    },
    {
      id: 'actions',
      cell: ({ row }) => {
        const ngoId = row.original.ngoId;

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

                    if (
                      await confirm({
                        title: `Delete ${row.original.name}?`,
                        body: 'This action is permanent and cannot be undone. Once deleted, this NGO admin cannot be retrieved.',
                        actionButton: 'Delete',
                        actionButtonClass: buttonVariants({ variant: 'destructive' }),
                        cancelButton: 'Cancel',
                      })
                    ) {
                      deleteMonitoringNgoMutation.mutate(ngoId);
                    }
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
    <>
      <Card>
        <CardHeader className='flex gap-2 flex-column'>
          <div className='flex flex-row items-center justify-between'>
            <CardTitle className='text-2xl font-semibold leading-none tracking-tight'>
              {t('electionEvent.monitoringNgos.cardTitle')}
            </CardTitle>
            <div className='flex md:flex-row-reverse gap-4 table-actions'>
              <Button title='Add admin' onClick={() => addMonitoringNgoDialog.trigger()}>
                <Plus className='mr-2' width={18} height={18} />
                Add monitoring NGO
              </Button>
            </div>
          </div>
          <Separator />
        </CardHeader>
        <CardContent className='flex flex-col items-baseline gap-6'>
          <MonitoringNgosTable columns={monitoringNgosColDefs} data={data?.monitoringNgos ?? []} />
        </CardContent>
      </Card>
      {addMonitoringNgoDialog.dialogProps.open && (
        <AddMonitoringNgoDialog electionRoundId={electionRoundId} {...addMonitoringNgoDialog.dialogProps} />
      )}
    </>
  );
}

export default MonitoringNgosDashboard;
