import { FunctionComponent } from '@/common/types';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '@/components/ui/table';
import {
  flexRender,
  getCoreRowModel,
  getFilteredRowModel,
  getPaginationRowModel,
  getSortedRowModel,
  useReactTable,
} from '@tanstack/react-table';
import { ImportPollingStationRow } from './PollingStationsImport';

import { DataTablePagination } from '@/components/ui/DataTable/DataTablePagination';
import { ColumnDef } from '@tanstack/react-table';

import { Input } from '@/components/ui/input';
import { useMemo, useState } from 'react';
import { columnDefinitions } from './column-defs';

type ImportedPollingStationsDataTableProps = {
  data: ImportPollingStationRow[];
};

export function ImportedPollingStationsDataTable({ data }: ImportedPollingStationsDataTableProps): FunctionComponent {
  const tableCols: ColumnDef<ImportPollingStationRow>[] = useMemo(() => columnDefinitions, []);
  const [globalFilter, setGlobalFilter] = useState('');

  const table = useReactTable({
    columns: tableCols,
    data,
    getCoreRowModel: getCoreRowModel(),
    getSortedRowModel: getSortedRowModel(),
    getFilteredRowModel: getFilteredRowModel(),
    getPaginationRowModel: getPaginationRowModel(),
    globalFilterFn: (row, columnId, filterValue) => {
      // Custom filter function
      return Object.values(row.original).some((value) =>
        String(value).toLowerCase().includes(filterValue.toLowerCase())
      );
    },
    state: { globalFilter },
  });

  const rows = table.getFilteredRowModel().rows;

  return (
    <div className='space-y-4 pr-4 pl-4'>
      <div className='h-8 w-[250px]'>
        <Input placeholder='Filter ...' value={globalFilter} onChange={(e) => setGlobalFilter(e.target.value)} />
      </div>
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
              <TableCell className='h-24 text-center' colSpan={tableCols.length}>
                No results.
              </TableCell>
            </TableRow>
          )}
        </TableBody>
      </Table>
      <DataTablePagination table={table} totalCount={table.getFilteredRowModel().rows.length} />
    </div>
  );
}
