import TableTagList from '@/components/table-tag-list/TableTagList';
import { Button } from '@/components/ui/button';
import { DataTableColumnHeader } from '@/components/ui/DataTable/DataTableColumnHeader';
import { Tooltip, TooltipContent, TooltipProvider, TooltipTrigger } from '@/components/ui/tooltip';
import { ExclamationTriangleIcon } from '@heroicons/react/24/solid';
import { ColumnDef } from '@tanstack/react-table';
import { ImportPollingStationRow } from './PollingStationsImport';

export const columnDefinitions: ColumnDef<ImportPollingStationRow>[] = [
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
    accessorKey: 'number',
    header: ({ column }) => <DataTableColumnHeader title='Number' column={column} />,
    cell: ({ row }) =>
      row.original.errors?.some((er) => er.path.some((path) => path === 'number')) ? (
        <div className='border-b-2 border-red-500 h-full w-full flex justify-between'>
          <span>{row.original.number}</span>

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
                    ?.filter((error) => error.path.some((path) => path === 'number'))
                    .map((error, idx) => <div key={error.message}>{error.message}</div>)}
                </div>
              </TooltipContent>
            </Tooltip>
          </TooltipProvider>
        </div>
      ) : (
        <div>{row.original.number}</div>
      ),
  },

  {
    accessorKey: 'address',
    header: ({ column }) => <DataTableColumnHeader title='Address' column={column} />,
    cell: ({ row }) =>
      row.original.errors?.some((er) => er.path.some((path) => path === 'address')) ? (
        <div className='border-b-2 border-red-500 h-full w-full flex justify-between'>
          <span>{row.original.address}</span>

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
                    ?.filter((error) => error.path.some((path) => path === 'address'))
                    .map((error, idx) => <div key={error.message}>{error.message}</div>)}
                </div>
              </TooltipContent>
            </Tooltip>
          </TooltipProvider>
        </div>
      ) : (
        <div>{row.original.address}</div>
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
    header: ({ column }) => <DataTableColumnHeader title={'Coordinates'} column={column} />,
    accessorKey: 'coordinates',
    enableSorting: false,
    enableGlobalFilter: true,
    cell: ({
      row: {
        original: { latitude, longitude },
      },
    }) =>
      latitude && longitude ? (
        <Button asChild variant={'link'}>
          <a href={`https://www.google.com/maps?q=${latitude},${longitude}`} target='_blank' rel='noopener noreferrer'>
            {latitude},{longitude}
          </a>
        </Button>
      ) : (
        '-'
      ),
  },
  {
    header: ({ column }) => <DataTableColumnHeader title={'Tags'} column={column} />,
    accessorKey: 'tags',
    enableSorting: false,
    enableGlobalFilter: true,
    cell: ({
      row: {
        original: { tags },
      },
    }) => <TableTagList tags={Object.entries(tags ?? {}).map(([key, value]) => `${key} : ${value}`)} />,
  },
  {
    accessorKey: 'errors',
    header: ({ column }) => <DataTableColumnHeader title='Errors' column={column} />,
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
                {row.original.errors?.map((error) => (
                  <div key={error.message}>
                    {error.path}: {error.message}
                  </div>
                ))}
              </div>
            </TooltipContent>
          </Tooltip>
        </TooltipProvider>
      ) : (
        '-'
      ),
  },
];
