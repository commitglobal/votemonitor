import { authApi } from '@/common/auth-api';
import TableTagList from '@/components/table-tag-list/TableTagList';
import { DataTableColumnHeader } from '@/components/ui/DataTable/DataTableColumnHeader';
import { QueryParamsDataTable } from '@/components/ui/DataTable/QueryParamsDataTable';
import { buildURLSearchParams } from '@/lib/utils';
import { useQuery, UseQueryResult } from '@tanstack/react-query';
import { ColumnDef } from '@tanstack/react-table';

import { PollingStation } from '../../models/PollingStation';

import type { DataTableParameters, PageResponse } from '@/common/types';
import { PollingStationsFilters } from '@/components/PollingStationsFilters/PollingStationsFilters';
import { FilterBadge } from '@/components/ui/badge';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Separator } from '@/components/ui/separator';
import { FunnelIcon } from '@heroicons/react/24/outline';
import { useNavigate, useSearch } from '@tanstack/react-router';
import { useDebounce } from '@uidotdev/usehooks';
import { useCallback, useMemo, useState, type ReactElement } from 'react';


function usePollingStations(queryParams: DataTableParameters): UseQueryResult<PageResponse<PollingStation>, Error> {
  return useQuery({
    queryKey: ['pollingStations', queryParams],
    queryFn: async () => {
      const params = {
        ...queryParams.otherParams,
        PageNumber: String(queryParams.pageNumber),
        PageSize: String(queryParams.pageSize),
        SortColumnName: queryParams.sortColumnName,
        SortOrder: queryParams.sortOrder,
      };
      const searchParams = buildURLSearchParams(params);

      const electionRoundId: string | null = localStorage.getItem('electionRoundId');

      const response = await authApi.get<PageResponse<PollingStation>>(`/election-rounds/${electionRoundId}/polling-stations:list`,
        {
          params: searchParams,
        }
      );

      if (response.status !== 200) {
        throw new Error('Failed to fetch polling stations');
      }

      return response.data;
    },
  });
}

export const pollingStationColDefs: ColumnDef<PollingStation>[] = [
  {
    header: ({ column }) => <DataTableColumnHeader title='Level 1' column={column} />,
    accessorKey: 'level1',
    enableSorting: true,
    enableGlobalFilter: true,
    cell: ({
      row: {
        original: { level1 },
      },
    }) => (
      <p>
        {level1}
      </p>
    ),
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Level 2' column={column} />,
    accessorKey: 'level2',
    enableSorting: true,
    enableGlobalFilter: true,
    cell: ({
      row: {
        original: { level2 },
      },
    }) => (
      <p>
        {level2}
      </p>
    ),
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Level 3' column={column} />,
    accessorKey: 'level3',
    enableSorting: true,
    enableGlobalFilter: true,
    cell: ({
      row: {
        original: { level3 },
      },
    }) => (
      <p>
        {level3}
      </p>
    ),
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Level 4' column={column} />,
    accessorKey: 'level4',
    enableSorting: true,
    enableGlobalFilter: true,
    cell: ({
      row: {
        original: { level4 },
      },
    }) => (
      <p>
        {level4}
      </p>
    ),
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Level 5' column={column} />,
    accessorKey: 'level5',
    enableSorting: true,
    enableGlobalFilter: true,
    cell: ({
      row: {
        original: { level5 },
      },
    }) => (
      <p>
        {level5}
      </p>
    ),
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Number' column={column} />,
    accessorKey: 'number',
    enableSorting: true,
    enableGlobalFilter: true,
    cell: ({
      row: {
        original: { number },
      },
    }) => (
      <p>
        {number}
      </p>
    ),
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Address' column={column} />,
    accessorKey: 'address',
    enableSorting: true,
    enableGlobalFilter: true,
    cell: ({
      row: {
        original: { address },
      },
    }) => (
      <p>
        {address}
      </p>
    ),
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Polling station tags' column={column} />,
    accessorKey: 'tags',
    enableSorting: false,
    enableGlobalFilter: true,
    cell: ({
      row: {
        original: { tags },
      },
    }) => <TableTagList tags={Object.entries(tags).map(([key, value]) => `${key} : ${value}`)} />,
  },
];


export default function PollingStationsDashboard(): ReactElement {
  const navigate = useNavigate();

  const search = useSearch({ strict: false }) as {
    level1Filter?: string;
    level2Filter?: string;
    level3Filter?: string;
    level4Filter?: string;
    level5Filter?: string;
  };

  const [isFiltering, setFiltering] = useState(Object.keys(search).some(k => k === 'level1Filter' || k === 'level2Filter' || k === 'level3Filter' || k === 'level4Filter' || k === 'level5Filter'));


  const changeIsFiltering = () => {
    setFiltering((prev) => {
      return !prev;
    });
  };

  const onClearFilter = useCallback(
    (filter: string | string[]) => () => {
      const filters = Array.isArray(filter)
        ? Object.fromEntries(filter.map((key) => [key, undefined]))
        : { [filter]: undefined };
      void navigate({ search: (prev) => ({ ...prev, ...filters }) });
    },
    [navigate]
  );


  const debouncedSearch = useDebounce(search, 300);

  const queryParams = useMemo(() => {
    const params = [
      ['level1Filter', debouncedSearch.level1Filter],
      ['level2Filter', debouncedSearch.level2Filter],
      ['level3Filter', debouncedSearch.level3Filter],
      ['level4Filter', debouncedSearch.level4Filter],
      ['level5Filter', debouncedSearch.level5Filter],
    ].filter(([_, value]) => value);

    return Object.fromEntries(params);
  }, [debouncedSearch]);

  return (
    <Card className='pt-0'>
      <CardHeader className='flex flex-column gap-2'>
        <div className='flex flex-row justify-between items-center'>
          <CardTitle className='text-xl'>Event details</CardTitle>
        </div>

        <Separator />
        <div className='filters px-6 flex flex-row justify-end gap-4'>
          <>
            <FunnelIcon
              onClick={changeIsFiltering}
              className='w-[20px] text-purple-900 cursor-pointer'
              fill={isFiltering ? '#5F288D' : 'rgba(0,0,0,0)'}
            />
          </>
        </div>
        <Separator />
        {isFiltering && (<div className='grid grid-cols-6 gap-4 items-center mb-4'>

          <PollingStationsFilters />

          <Button
            onClick={() => {
              void navigate({});
            }}
            variant='ghost-primary'>
            Reset filters
          </Button>
        </div>)}
        {Object.entries(search).length > 0 && (
          <div className='col-span-full flex gap-2 flex-wrap'>


            {search.level1Filter && (
              <FilterBadge label={`Location - L1: ${search.level1Filter}`} onClear={onClearFilter(['level1Filter', 'level2Filter', 'level3Filter', 'level4Filter', 'level5Filter'])} />
            )}

            {search.level2Filter && (
              <FilterBadge label={`Location - L2: ${search.level2Filter}`} onClear={onClearFilter(['level2Filter', 'level3Filter', 'level4Filter', 'level5Filter'])} />
            )}

            {search.level3Filter && (
              <FilterBadge label={`Location - L3: ${search.level3Filter}`} onClear={onClearFilter(['level3Filter', 'level4Filter', 'level5Filter'])} />
            )}

            {search.level4Filter && (
              <FilterBadge label={`Location - L4: ${search.level4Filter}`} onClear={onClearFilter(['level4Filter', 'level5Filter'])} />
            )}

            {search.level5Filter && (
              <FilterBadge label={`Location - L5: ${search.level5Filter}`} onClear={onClearFilter(['level5Filter'])} />
            )}
          </div>
        )}

      </CardHeader>
      <CardContent className='flex flex-col gap-6 items-baseline'>
        <QueryParamsDataTable columns={pollingStationColDefs} useQuery={usePollingStations}  queryParams={queryParams}/>
      </CardContent>
    </Card>
  );
}