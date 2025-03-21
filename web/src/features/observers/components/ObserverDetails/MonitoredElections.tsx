import { ColumnDef, flexRender, getCoreRowModel, getSortedRowModel, useReactTable } from '@tanstack/react-table';
import { FC } from 'react';
import { Observer, ObserversMonitoredElectionModel } from '../../models/observer';
import { DataTableColumnHeader } from '@/components/ui/DataTable/DataTableColumnHeader';
import ElectionRoundStatusBadge from '@/components/ElectionRoundStatusBadge/ElectionRoundStatusBadge';
import { useNavigate } from '@tanstack/react-router';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { TableHeader, TableRow, TableHead, TableBody, TableCell, Table } from '@/components/ui/table';
import { Separator } from '@/components/ui/separator';

interface ObserversProps {
  observer: Observer;
}

const observersMonitoredElectionsColDefs: ColumnDef<ObserversMonitoredElectionModel>[] = [
  {
    accessorKey: 'title',
    header: ({ column }) => <DataTableColumnHeader title='Election title' column={column} />,
  },
  {
    accessorKey: 'englishTitle',
    header: ({ column }) => <DataTableColumnHeader title='Election English title' column={column} />,
  },
  {
    accessorKey: 'startDate',
    header: ({ column }) => <DataTableColumnHeader title='Election date' column={column} />,
  },
  {
    accessorKey: 'status',
    header: ({ column }) => <DataTableColumnHeader title='Status' column={column} />,
    cell: ({
      row: {
        original: { status },
      },
    }) => <ElectionRoundStatusBadge status={status} />,
  },
];

export const ObserversMonitoredElectionView: FC<ObserversProps> = ({ observer }: ObserversProps) => {
  const table = useReactTable({
    columns: observersMonitoredElectionsColDefs,
    data: observer.monitoredElections,
    getCoreRowModel: getCoreRowModel(),
    getSortedRowModel: getSortedRowModel(),
  });

  const rows = table.getRowModel().rows;
  const navigate = useNavigate();
  console.log(observer.monitoredElections);
  return (
    <Card className='w-[1400px] pt-0'>
      <CardHeader className='flex gap-2 flex-column'>
        <div className='flex flex-row items-center justify-between'>
          <CardTitle className='text-xl'>Monitored Elections</CardTitle>
        </div>
        <Separator />
      </CardHeader>
      <CardContent className='flex flex-col items-baseline gap-6'>
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
                <TableRow
                  className='cursor-pointer'
                  key={row.id}
                  onClick={() => {
                    navigate({
                      to: `/election-rounds/${row.original.id}/event-details`,
                    });
                  }}>
                  {row.getVisibleCells().map((cell) => (
                    <TableCell key={cell.id}>{flexRender(cell.column.columnDef.cell, cell.getContext())}</TableCell>
                  ))}
                </TableRow>
              ))
            ) : (
              <TableRow>
                <TableCell className='h-24 text-center' colSpan={observersMonitoredElectionsColDefs.length}>
                  No elections monitored.
                </TableCell>
              </TableRow>
            )}
          </TableBody>
        </Table>
      </CardContent>
    </Card>
  );
};
