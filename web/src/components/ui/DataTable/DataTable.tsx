import { SortOrder, type DataTableParameters, type PageResponse } from '@/common/types';
import { EmptyCollectionIcon } from '@/components/icons/EmptyCollectionIcon';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '@/components/ui/table';
import type { UseQueryResult } from '@tanstack/react-query';
import {
  flexRender,
  getCoreRowModel,
  getExpandedRowModel,
  getSortedRowModel,
  useReactTable,
  type CellContext,
  type ColumnDef,
  type PaginationState,
  type Row,
  type SortingState,
  type VisibilityState,
} from '@tanstack/react-table';
import { useEffect, useState, type ReactElement } from 'react';
import { Skeleton } from '../skeleton';
import { DataTablePagination } from './DataTablePagination';

export interface RowData {
  id: string;
  defaultLanguage?: string;
}

export interface TableCellProps {
  className: string;
}

declare module '@tanstack/react-table' {
  interface ColumnMeta<TData, TValue> {
    // Your additional properties here
    getCellContext: (context: CellContext<TData, TValue>) => TableCellProps | void;
  }
}

function isArrayResult<TData>(result: PageResponse<TData> | TData[]): result is TData[] {
  return Array.isArray(result); // data will be an array for TData[]
}

function isEmpty<TData>(result?: PageResponse<TData> | TData[]): boolean | undefined{
  if(result === undefined) return undefined;

 if(isArrayResult(result)){
  return result.length === 0;

 }

 return result.isEmpty;
}
export interface DataTableProps<TData extends RowData, TValue, TQueryParams = object> {
  /**
   * Tanstack table column definitions.
   */
  columns: ColumnDef<TData, TValue>[];

  /**
   * Tanstack query for paginated data.
   */
  useQuery: (
    params: DataTableParameters<TQueryParams>
  ) => UseQueryResult<PageResponse<TData>, Error> | UseQueryResult<TData[], Error>;

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

  /**
   * Externalize sorting state to the parent component.
   * Used by QueryParamsDataTable.
   */
  sortingExt?: SortingState;

  /**
   * Externalize sorting state to the parent component.
   * Used by QueryParamsDataTable.
   */
  setSortingExt?: (p: SortingState) => void;

  /**
   * Externalize column visibility state to the parent component.
   * Used by QueryParamsDataTable
   */
  columnVisibility?: VisibilityState;

  /**
   * Externalize query params to the parent component.
   * Used by QueryParamsDataTable
   */
  queryParams?: TQueryParams;

  /**
   * Externalize subrows creation
   * Used by DataTable
   * @param originalRow
   * @param index
   * @returns
   */
  getSubrows?: (originalRow: TData, index: number) => undefined | TData[];

  /**
   * Externalize row styling
   * Used by DataTable
   * @param originalRow
   * @param index
   * @returns
   */
  getRowClassName?: (row: Row<TData>) => string | undefined;

  onDataFetchingSucceed?: (pageSize: number, currentPage: number, totalCount: number) => void;

  onRowClick?: (id: string, defaultLanguage?: string) => void;

  getCellProps?: (context: CellContext<TData, unknown>) => TableCellProps | void;

  emptyTitle?: string;

  emptySubtitle?: string;
}

export function DataTable<TData extends RowData, TValue, TQueryParams = object>({
  columnVisibility,
  columns,
  paginationExt,
  setPaginationExt,
  setSortingExt,
  sortingExt,
  useQuery,
  queryParams,
  getSubrows,
  getRowClassName,
  onDataFetchingSucceed,
  onRowClick,
  getCellProps,
  emptyTitle,
  emptySubtitle,
}: DataTableProps<TData, TValue, TQueryParams>): ReactElement {
  let [pagination, setPagination]: [PaginationState, (p: PaginationState) => void] = useState({
    pageIndex: 0,
    pageSize: 10,
  });

  if (paginationExt) {
    if (!setPaginationExt) throw new Error('setPaginationExt is required when paginationExt is provided.');
    [pagination, setPagination] = [paginationExt, setPaginationExt];
  }

  let [sorting, setSorting]: [SortingState, (s: SortingState) => void] = useState<SortingState>([]);

  if (sortingExt) {
    if (!setSortingExt) throw new Error('setSortingExt is required when sortingExt is provided.');
    [sorting, setSorting] = [sortingExt, setSortingExt];
  }

  const { data: result, isFetching, isSuccess } = useQuery({
    pageNumber: pagination.pageIndex + 1,
    pageSize: pagination.pageSize,
    sortColumnName: sorting[0]?.id || 'id',
    sortOrder: sorting[0]?.desc ? SortOrder.desc : SortOrder.asc,
    otherParams: queryParams,
  });

  useEffect(() => {
    if (isSuccess && onDataFetchingSucceed) {
      if (isArrayResult(result)) {
        onDataFetchingSucceed(result.length, 1, result.length);
      } else {
        onDataFetchingSucceed(result.pageSize, result.currentPage, result.totalCount);
      }
    }
  }, [isSuccess, queryParams, result]);

  const table = useReactTable({
    data: !!result ? (isArrayResult(result) ? result : result.items ?? []) : [],
    columns,
    manualPagination: true,
    manualSorting: true,
    enableSorting: true,
    enableFilters: true,
    pageCount: result ? (isArrayResult(result) ? result.length : Math.ceil(result.totalCount / result.pageSize)) : 0,
    columnResizeMode: 'onChange',
    enableColumnResizing: true,
    getCoreRowModel: getCoreRowModel(),
    getSortedRowModel: getSortedRowModel(),
    getExpandedRowModel: getExpandedRowModel(),
    getSubRows: getSubrows,
    onPaginationChange: (updater) => {
      setPagination(updater instanceof Function ? updater(pagination) : updater);
    },
    onSortingChange: (updater) => {
      setSorting(updater instanceof Function ? updater(sorting) : updater);
    },
    state: {
      sorting,
      pagination,
      columnVisibility,
    },
    defaultColumn: {
      size: 165,
    },
  });

  return (
    <div className='bg-white border border-gray-300 shadow-sm filament-tables-container rounded-xl'>
      {isEmpty(result) ? (
        <div className='flex flex-col items-center justify-center max-w-lg gap-2 m-auto py-52'>
          <EmptyCollectionIcon />

          <p className='text-2xl font-bold'>{emptyTitle ?? 'No data'}</p>
          {emptySubtitle && <p className='text-lg text-[#606674] text-center'>{emptySubtitle}</p>}
        </div>
      ) : (
        <>
          <div>
            <Table>
              <TableHeader>
                {table.getHeaderGroups().map((headerGroup) => (
                  <TableRow key={headerGroup.id}>
                    {headerGroup.headers.map((header) => {
                      return (
                        <TableHead key={header.id} style={{ maxWidth: header.getSize() }}>
                          {header.isPlaceholder
                            ? null
                            : flexRender(header.column.columnDef.header, header.getContext())}
                        </TableHead>
                      );
                    })}
                  </TableRow>
                ))}
              </TableHeader>
              <TableBody>
                {isFetching ? (
                  Array.from({ length: pagination.pageSize }).map((_, index) => (
                    <TableRow key={index}>
                      {table.getVisibleLeafColumns().map((_, index) => (
                        <TableCell key={index}>
                          <Skeleton className='w-[100px] h-[20px] rounded-full' />
                        </TableCell>
                      ))}
                    </TableRow>
                  ))
                ) : table.getRowModel().rows?.length ? (
                  table.getRowModel().rows.map((row) => (
                    <TableRow
                      key={row.id}
                      data-state={row.getIsSelected() && 'selected'}
                      className={getRowClassName ? getRowClassName(row) : ''}
                      onClick={() => {
                        onRowClick?.(row.original.id, row.original.defaultLanguage);
                      }}
                      style={{ cursor: onRowClick ? 'pointer' : undefined }}>
                      {row.getVisibleCells().map((cell) => (
                        <TableCell
                          title={cell.getValue() as string}
                          key={cell.id}
                          style={{ maxWidth: cell.column.getSize() }}
                          {...(getCellProps ? getCellProps(cell.getContext()) : {})}>
                          {flexRender(cell.column.columnDef.cell, cell.getContext())}
                        </TableCell>
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

          {!!result && !isArrayResult(result) && <DataTablePagination table={table} totalCount={result?.totalCount} />}
        </>
      )}
    </div>
  );
}
