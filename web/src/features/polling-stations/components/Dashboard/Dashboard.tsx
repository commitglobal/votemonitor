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
import { useCurrentElectionRoundStore } from '@/context/election-round.store';
import { ExportDataButton } from '@/features/responses/components/ExportDataButton/ExportDataButton';
import { ExportedDataType } from '@/features/responses/models/data-export';
import { FunnelIcon } from '@heroicons/react/24/outline';
import { useNavigate, useSearch } from '@tanstack/react-router';
import { useDebounce } from '@uidotdev/usehooks';
import { useCallback, useMemo, useState, type ReactElement } from 'react';
import i18n from '@/i18n';

function usePollingStations(electionRoundId: string, queryParams: DataTableParameters): UseQueryResult<PageResponse<PollingStation>, Error> {
  return useQuery({
    queryKey: ['pollingStations', electionRoundId, queryParams],
    queryFn: async () => {
      const params = {
        ...queryParams.otherParams,
        PageNumber: String(queryParams.pageNumber),
        PageSize: String(queryParams.pageSize),
        SortColumnName: queryParams.sortColumnName,
        SortOrder: queryParams.sortOrder,
      };
      const searchParams = buildURLSearchParams(params);

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
    enabled: !!electionRoundId
  });
}

export const pollingStationColDefs: ColumnDef<PollingStation>[] = [
  {
    header: ({ column }) => <DataTableColumnHeader title={i18n.t('electionEvent.pollingStations.headers.level1')} column={column} />,
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
    header: ({ column }) => <DataTableColumnHeader title={i18n.t('electionEvent.pollingStations.headers.level2')} column={column} />,
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
    header: ({ column }) => <DataTableColumnHeader title={i18n.t('electionEvent.pollingStations.headers.level3')} column={column} />,
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
    header: ({ column }) => <DataTableColumnHeader title={i18n.t('electionEvent.pollingStations.headers.level4')} column={column} />,
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
    header: ({ column }) => <DataTableColumnHeader title={i18n.t('electionEvent.pollingStations.headers.level5')} column={column} />,
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
    header: ({ column }) => <DataTableColumnHeader title={i18n.t('electionEvent.pollingStations.headers.number')} column={column} />,
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
    header: ({ column }) => <DataTableColumnHeader title={i18n.t('translation:electionEvent.pollingStations.headers.address')} column={column} />,
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
    header: ({ column }) => <DataTableColumnHeader title={i18n.t('electionEvent.pollingStations.headers.tags')} column={column} />,
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
    pollingStationNumberFilter?: string;
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
  const currentElectionRoundId = useCurrentElectionRoundStore(s => s.currentElectionRoundId);

  const queryParams = useMemo(() => {
    const params = [
      ['level1Filter', debouncedSearch.level1Filter],
      ['level2Filter', debouncedSearch.level2Filter],
      ['level3Filter', debouncedSearch.level3Filter],
      ['level4Filter', debouncedSearch.level4Filter],
      ['level5Filter', debouncedSearch.level5Filter],
      ['pollingStationNumberFilter', debouncedSearch.pollingStationNumberFilter],
    ].filter(([_, value]) => value);

    return Object.fromEntries(params);
  }, [debouncedSearch]);

  return (
    <Card className='pt-0'>
      <CardHeader className='flex gap-2 flex-column'>
        <div className='flex items-center justify-between px-6'>
          <CardTitle>{i18n.t('electionEvent.pollingStations.cardTitle')}</CardTitle>

          <div className='flex items-center gap-4'>
            <ExportDataButton exportedDataType={ExportedDataType.PollingStations} />
          </div>

        </div>
        <Separator />
        <div className='flex flex-row justify-end gap-4 px-6 filters'>
          <>
            <FunnelIcon
              onClick={changeIsFiltering}
              className='w-[20px] text-purple-900 cursor-pointer'
              fill={isFiltering ? '#5F288D' : 'rgba(0,0,0,0)'}
            />
          </>
        </div>
        <Separator />
        {isFiltering && (<div className='grid items-center grid-cols-6 gap-4 mb-4'>

          <PollingStationsFilters />

          <Button
            onClick={() => {
              void navigate({});
            }}
            variant='ghost-primary'>
            {i18n.t('electionEvent.pollingStations.resetFilters')}
          </Button>
        </div>)}
        {Object.entries(search).length > 0 && (
          <div className='flex flex-wrap gap-2 col-span-full'>


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
      <CardContent className='flex flex-col items-baseline gap-6'>
        <QueryParamsDataTable columns={pollingStationColDefs} useQuery={(params) => usePollingStations(currentElectionRoundId, params)} queryParams={queryParams} />
      </CardContent>
    </Card>
  );
}