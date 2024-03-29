import {
  type ColumnDef,
  flexRender,
  getCoreRowModel,
  useReactTable,
  type PaginationState,
} from '@tanstack/react-table';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '@/components/ui/table';
import { useState, type ReactElement } from 'react';
import type { PageParameters, PageResponse } from '@/common/types';
import { DataTablePagination } from './DataTablePagination';
import type { UseQueryResult } from '@tanstack/react-query';
import { Skeleton } from '../skeleton';

export interface DataTableProps<TData, TValue> {
  /**
   * Tanstack table column definitions.
   */
  columns: ColumnDef<TData, TValue>[];

  /**
   * Tanstack query for paginated data.
   */
  usePagedQuery: (p: PageParameters) => UseQueryResult<PageResponse<TData>, Error>;

  /**
   * Externalize pagination state to the parent component.
   * Used by QueryParamsDataTable.
   */
  paginationExt?: PaginationState;

  /**
   * Externalize pagination state to the parent component.
   * Used by QueryParamsDataTable.
   */
  setPaginationExt?: (p: PaginationState) => void;
}

export function DataTable<TData, TValue>({
  columns,
  usePagedQuery,
  paginationExt,
  setPaginationExt
}: DataTableProps<TData, TValue>): ReactElement {
  let [pagination, setPagination]: [PaginationState, (p: PaginationState) => void] = useState({ pageIndex: 0, pageSize: 10 });
  if (paginationExt) {
    if (!setPaginationExt) throw new Error('setPaginationExt is required when paginationExt is provided.');
    [pagination, setPagination] = [paginationExt, setPaginationExt];
  }

  const { data, isFetching } = usePagedQuery({
    pageNumber: pagination.pageIndex + 1,
    pageSize: pagination.pageSize,
  });

  const table = useReactTable({
    data: data?.items || [],
    columns,
    manualPagination: true,
    enableFilters: true,
    pageCount: data ? Math.ceil(data.totalCount / data.pageSize) : 0,
    getCoreRowModel: getCoreRowModel(),
    onPaginationChange: (updater) => {
      setPagination(updater instanceof Function ? updater(pagination) : updater);
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
