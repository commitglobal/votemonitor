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
import { ImportLocationRow } from './LocationsImport';

import { DataTablePagination } from '@/components/ui/DataTable/DataTablePagination';
import { ColumnDef } from '@tanstack/react-table';

import { DataTableColumnHeader } from '@/components/ui/DataTable/DataTableColumnHeader';
import { Tooltip, TooltipContent, TooltipProvider, TooltipTrigger } from '@/components/ui/tooltip';

import { LocationDataTableRowActions } from '@/components/LocationDataTableRowActions/LocationDataTableRowActions';
import { Input } from '@/components/ui/input';
import { ExclamationTriangleIcon } from '@heroicons/react/24/solid';
import { useMemo, useState } from 'react';

type ImportedLocationsDataTableProps = {
  data: ImportLocationRow[];
  updateLocation: (location: ImportLocationRow) => void;
  deleteLocation: (location: ImportLocationRow) => void;
};

export function ImportedLocationsDataTable({
  data,
  updateLocation,
  deleteLocation,
}: ImportedLocationsDataTableProps): FunctionComponent {
  const tableCols: ColumnDef<ImportLocationRow>[] = useMemo(
    () => [
      {
        accessorKey: 'level1',
        header: ({ column }) => <DataTableColumnHeader title='Level1' column={column} />,
        cell: ({ row }) =>
          row.original.errors?.some((er) => er.path.some((path) => path === 'level1')) ? (
            <div className='border-b-2 border-red-500 h-full w-full flex justify-between'>
              <span>{row.original.level1}</span>

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
                        ?.filter((error) => error.path.some((path) => path === 'level1'))
                        .map((error, idx) => <div key={error.message}>{error.message}</div>)}
                    </div>
                  </TooltipContent>
                </Tooltip>
              </TooltipProvider>
            </div>
          ) : (
            <div>{row.original.level1}</div>
          ),
      },

      {
        accessorKey: 'level2',
        header: ({ column }) => <DataTableColumnHeader title='Level2' column={column} />,
        cell: ({ row }) =>
          row.original.errors?.some((er) => er.path.some((path) => path === 'level2')) ? (
            <div className='border-b-2 border-red-500 h-full w-full flex justify-between'>
              <span>{row.original.level2}</span>

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
                        ?.filter((error) => error.path.some((path) => path === 'level2'))
                        .map((error, idx) => <div key={error.message}>{error.message}</div>)}
                    </div>
                  </TooltipContent>
                </Tooltip>
              </TooltipProvider>
            </div>
          ) : (
            <div>{row.original.level2}</div>
          ),
      },

      {
        accessorKey: 'level3',
        header: ({ column }) => <DataTableColumnHeader title='Level3' column={column} />,
        cell: ({ row }) =>
          row.original.errors?.some((er) => er.path.some((path) => path === 'level3')) ? (
            <div className='border-b-2 border-red-500 h-full w-full flex justify-between'>
              <span>{row.original.level3}</span>

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
                        ?.filter((error) => error.path.some((path) => path === 'level3'))
                        .map((error, idx) => <div key={error.message}>{error.message}</div>)}
                    </div>
                  </TooltipContent>
                </Tooltip>
              </TooltipProvider>
            </div>
          ) : (
            <div>{row.original.level3}</div>
          ),
      },

      {
        accessorKey: 'level4',
        header: ({ column }) => <DataTableColumnHeader title='Level4' column={column} />,
        cell: ({ row }) =>
          row.original.errors?.some((er) => er.path.some((path) => path === 'level4')) ? (
            <div className='border-b-2 border-red-500 h-full w-full flex justify-between'>
              <span>{row.original.level4}</span>

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
                        ?.filter((error) => error.path.some((path) => path === 'level4'))
                        .map((error, idx) => <div key={error.message}>{error.message}</div>)}
                    </div>
                  </TooltipContent>
                </Tooltip>
              </TooltipProvider>
            </div>
          ) : (
            <div>{row.original.level4}</div>
          ),
      },

      {
        accessorKey: 'level5',
        header: ({ column }) => <DataTableColumnHeader title='Level5' column={column} />,
        cell: ({ row }) =>
          row.original.errors?.some((er) => er.path.some((path) => path === 'level5')) ? (
            <div className='border-b-2 border-red-500 h-full w-full flex justify-between'>
              <span>{row.original.level5}</span>

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
                        ?.filter((error) => error.path.some((path) => path === 'level5'))
                        .map((error, idx) => <div key={error.message}>{error.message}</div>)}
                    </div>
                  </TooltipContent>
                </Tooltip>
              </TooltipProvider>
            </div>
          ) : (
            <div>{row.original.level5}</div>
          ),
      },

      {
        accessorKey: 'displayOrder',
        header: ({ column }) => <DataTableColumnHeader title='Display order' column={column} />,
        cell: ({ row }) =>
          row.original.errors?.some((er) => er.path.some((path) => path === 'displayOrder')) ? (
            <div className='border-b-2 border-red-500 h-full w-full flex justify-between'>
              <span>{row.original.displayOrder}</span>

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
                        ?.filter((error) => error.path.some((path) => path === 'displayOrder'))
                        .map((error, idx) => <div key={error.message}>{error.message}</div>)}
                    </div>
                  </TooltipContent>
                </Tooltip>
              </TooltipProvider>
            </div>
          ) : (
            <div>{row.original.displayOrder}</div>
          ),
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
          <LocationDataTableRowActions
            location={row.original}
            updateLocation={updateLocation}
            deleteLocation={deleteLocation}
          />
        ),
      },
    ],
    [updateLocation, deleteLocation]
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
