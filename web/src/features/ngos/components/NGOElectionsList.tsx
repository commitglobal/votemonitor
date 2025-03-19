import { FC } from 'react';
import { MonitoredElectionModel, NGO } from '../models/NGO';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Separator } from '@/components/ui/separator';
import { DataTableColumnHeader } from '@/components/ui/DataTable/DataTableColumnHeader';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '@/components/ui/table';
import { ColumnDef, flexRender, getCoreRowModel, getSortedRowModel, useReactTable } from '@tanstack/react-table';
import ElectionRoundStatusBadge from '@/components/ElectionRoundStatusBadge/ElectionRoundStatusBadge';
import { useNavigate } from '@tanstack/react-router';

interface NGODetailsProps {
  ngo: NGO;
}

const ngoElectionsColDefs: ColumnDef<MonitoredElectionModel>[] = [
  {
    accessorKey: 'title',
    header: ({ column }) => <DataTableColumnHeader title='Title' column={column} />,
  },
  {
    accessorKey: 'englishTitle',
    header: ({ column }) => <DataTableColumnHeader title='English title' column={column} />,
  },
  {
    accessorKey: 'startDate',
    header: ({ column }) => <DataTableColumnHeader title='Start date' column={column} />,
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

export const NGOElectionsListView: FC<NGODetailsProps> = ({ ngo }: NGODetailsProps) => {
  const table = useReactTable({
    columns: ngoElectionsColDefs,
    data: ngo.monitoredElections.sort((a, b) => b.startDate.localeCompare(a.startDate)),
    getCoreRowModel: getCoreRowModel(),
    getSortedRowModel: getSortedRowModel(),
  });

  const rows = table.getRowModel().rows;

  const navigate = useNavigate();
  const navigateToElectionRoundPage = (id: string) => {
    navigate({
      to: `/election-rounds/${id}/event-details`,
    });
  };

  return (
    <Card className='w-[1400px] pt-0'>
      <CardHeader className='flex gap-2 flex-column'>
        <div className='flex flex-row items-center justify-between'>
          <CardTitle className='text-xl'>Election events</CardTitle>
        </div>
        <Separator />
      </CardHeader>
      ``
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
                  key={row.id}
                  onClick={() => {
                    navigateToElectionRoundPage(row.original.id);
                  }}>
                  {row.getVisibleCells().map((cell) => (
                    <TableCell key={cell.id}>{flexRender(cell.column.columnDef.cell, cell.getContext())}</TableCell>
                  ))}
                </TableRow>
              ))
            ) : (
              <TableRow>
                <TableCell className='h-24 text-center' colSpan={ngoElectionsColDefs.length}>
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
