import {
  type ColumnDef,
  flexRender,
  getCoreRowModel,
  useReactTable,
  type PaginationState,
} from '@tanstack/react-table';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '@/components/ui/table';
import type { ReactElement } from 'react';
import type { PageResponse } from '@/common/types';
import { DataTablePagination } from './DataTablePagination';
import type { UseQueryResult } from '@tanstack/react-query';
import { Skeleton } from '../skeleton';

interface DataTableProps<TData, TValue> {
  columns: ColumnDef<TData, TValue>[];
  queryResult: UseQueryResult<PageResponse<TData>, Error>;
  pagination: PaginationState;
  updatePagination: (ps: PaginationState) => void;
}

export function DataTable<TData, TValue>({
  columns,
  queryResult,
  pagination,
  updatePagination,
}: DataTableProps<TData, TValue>): ReactElement {
  const { data, isFetching } = queryResult;

  const table = useReactTable({
    data: data?.items || [],
    columns,
    manualPagination: true,
    pageCount: data ? Math.ceil(data.totalCount / data.pageSize) : 0,
    getCoreRowModel: getCoreRowModel(),
    onPaginationChange: (updater) => {
      updatePagination(updater instanceof Function ? updater(pagination) : updater);
    },
    state: { pagination },
  });

  return (
    <div>
      <div className='rounded-md border'>
        <Table>
          <TableHeader>
            {table.getHeaderGroups().map((headerGroup) => (
              <TableRow key={headerGroup.id}>
                {headerGroup.headers.map((header) => {
                  return (
                    <TableHead key={header.id}>
                      {header.isPlaceholder ? null : flexRender(header.column.columnDef.header, header.getContext())}
                    </TableHead>
                  );
                })}
              </TableRow>
            ))}
          </TableHeader>
          <TableBody>
            {isFetching ? (
              Array.from({ length: 5 }).map((_, index) => (
                <TableRow key={index}>
                  {columns.map((_, index) => (
                    <TableCell key={index}>
                      <Skeleton className='w-[100px] h-[20px] rounded-full' />
                    </TableCell>
                  ))}
                </TableRow>
              ))
            ) : table.getRowModel().rows?.length ? (
              table.getRowModel().rows.map((row) => (
                <TableRow key={row.id} data-state={row.getIsSelected() && 'selected'}>
                  {row.getVisibleCells().map((cell) => (
                    <TableCell key={cell.id}>{flexRender(cell.column.columnDef.cell, cell.getContext())}</TableCell>
                  ))}
                </TableRow>
              ))
            ) : (
              <TableRow>
                <TableCell colSpan={columns.length} className='h-24 text-center'>
                  No results.
                </TableCell>
              </TableRow>
            )}
          </TableBody>
        </Table>
      </div>
      <div className='mt-4'>
        <DataTablePagination table={table} />
      </div>
    </div>
  );
}
