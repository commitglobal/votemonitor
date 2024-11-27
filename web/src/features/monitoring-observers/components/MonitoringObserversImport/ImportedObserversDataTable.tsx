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
import { ImportObserverRow } from './MonitoringObserversImport';

import { DataTablePagination } from '@/components/ui/DataTable/DataTablePagination';
import { ColumnDef } from '@tanstack/react-table';

import { DataTableColumnHeader } from '@/components/ui/DataTable/DataTableColumnHeader';
import { Tooltip, TooltipContent, TooltipProvider, TooltipTrigger } from '@/components/ui/tooltip';

import { ExclamationTriangleIcon } from '@heroicons/react/24/solid';
import { useMemo, useState } from 'react';
import { ImportObserverDataTableRowActions } from './ImportObserverDataTableRowActions';
import { Input } from '@/components/ui/input';

type ImportedObserversDataTableProps = {
  data: ImportObserverRow[];
  updateObserver: (observer: ImportObserverRow) => void;
  deleteObserver: (observer: ImportObserverRow) => void;
};

export function ImportedObserversDataTable({
  data,
  updateObserver,
  deleteObserver,
}: ImportedObserversDataTableProps): FunctionComponent {
  const tableCols: ColumnDef<ImportObserverRow>[] = useMemo(
    () => [
      {
        accessorKey: 'firstName',
        header: ({ column }) => <DataTableColumnHeader title='First name' column={column} />,
        cell: ({ row }) =>
          row.original.errors?.some((er) => er.path.some((path) => path === 'firstName')) ? (
            <div className='border-b-2 border-red-500 h-full w-full flex justify-between'>
              <span>{row.original.firstName}</span>

              <TooltipProvider delayDuration={100}>
                <Tooltip>
                  <TooltipTrigger asChild>
                    <span className='underline cursor-pointer decoration-dashed hover:decoration-solid'>
                      <ExclamationTriangleIcon className='h-5 w-5 cursor-pointer text-red-500' />
                    </span>
                  </TooltipTrigger>
                  <TooltipContent>
                    <div className='flex flex-col'>
                      {row.original.errors
                        ?.filter((error) => error.path.some((path) => path === 'firstName'))
                        .map((error) => <div key={error.message}>{error.message}</div>)}
                    </div>
                  </TooltipContent>
                </Tooltip>
              </TooltipProvider>
            </div>
          ) : (
            <div>{row.original.firstName}</div>
          ),
      },
      {
        accessorKey: 'lastName',
        header: ({ column }) => <DataTableColumnHeader title='Last name' column={column} />,
        cell: ({ row }) =>
          row.original.errors?.some((er) => er.path.some((path) => path === 'lastName')) ? (
            <div className='border-b-2 border-red-500 h-full w-full flex justify-between'>
              <span>{row.original.lastName}</span>

              <TooltipProvider delayDuration={100}>
                <Tooltip>
                  <TooltipTrigger asChild>
                    <span className='underline cursor-pointer decoration-dashed hover:decoration-solid'>
                      <ExclamationTriangleIcon className='h-5 w-5 cursor-pointer text-red-500' />
                    </span>
                  </TooltipTrigger>
                  <TooltipContent>
                    <div className='flex flex-col'>
                      {row.original.errors
                        ?.filter((error) => error.path.some((path) => path === 'lastName'))
                        .map((error) => <div key={error.message}>{error.message}</div>)}
                    </div>
                  </TooltipContent>
                </Tooltip>
              </TooltipProvider>
            </div>
          ) : (
            <div>{row.original.lastName}</div>
          ),
      },
      {
        accessorKey: 'email',
        header: ({ column }) => <DataTableColumnHeader title='Email' column={column} />,
        cell: ({ row }) =>
          row.original.errors?.some((er) => er.path.some((path) => path === 'email')) ? (
            <div className='border-b-2 border-red-500 h-full w-full flex justify-between'>
              <span>{row.original.email}</span>

              <TooltipProvider delayDuration={100}>
                <Tooltip>
                  <TooltipTrigger asChild>
                    <span className='underline cursor-pointer decoration-dashed hover:decoration-solid'>
                      <ExclamationTriangleIcon className='h-5 w-5 cursor-pointer text-red-500' />
                    </span>
                  </TooltipTrigger>
                  <TooltipContent>
                    <div className='flex flex-col'>
                      {row.original.errors
                        ?.filter((error) => error.path.some((path) => path === 'email'))
                        .map((error) => <div key={error.message}>{error.message}</div>)}
                    </div>
                  </TooltipContent>
                </Tooltip>
              </TooltipProvider>
            </div>
          ) : (
            <div>{row.original.email}</div>
          ),
      },
      {
        accessorKey: 'phoneNumber',
        header: ({ column }) => <DataTableColumnHeader title='Phone number' column={column} />,
        cell: ({ row }) => <div>{row.original.phoneNumber}</div>,
      },

      {
        accessorKey: 'errors',
        header: ({ column }) => <DataTableColumnHeader title='Number of errors' column={column} />,
        cell: ({ row }) =>
          row.original.errors?.length ? (
            <TooltipProvider delayDuration={100}>
              <Tooltip>
                <TooltipTrigger asChild>
                  <span className='underline cursor-pointer decoration-dashed hover:decoration-solid'>
                    {row.original.errors?.length}
                  </span>
                </TooltipTrigger>
                <TooltipContent>
                  <div className='flex flex-col'>
                    {row.original.errors?.map((error) => <div key={error.message}>{error.message}</div>)}
                  </div>
                </TooltipContent>
              </Tooltip>
            </TooltipProvider>
          ) : null,
      },

      {
        id: 'actions',
        cell: ({ row }) => (
          <ImportObserverDataTableRowActions
            row={row}
            updateObserver={updateObserver}
            deleteObserver={deleteObserver}
          />
        ),
      },
    ],
    [updateObserver, deleteObserver]
  );
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
