import { authApi } from '@/common/auth-api';
import TableTagList from '@/components/table-tag-list/TableTagList';
import { DataTableColumnHeader } from '@/components/ui/DataTable/DataTableColumnHeader';
import { QueryParamsDataTable } from '@/components/ui/DataTable/QueryParamsDataTable';
import { buildURLSearchParams } from '@/lib/utils';
import { useQuery, UseQueryResult } from '@tanstack/react-query';
import { ColumnDef } from '@tanstack/react-table';

import { PollingStation } from '../../models/PollingStation';

import type { ReactElement } from 'react';
import type { DataTableParameters, PageParameters, PageResponse } from '@/common/types';
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
    header: ({ column }) => <DataTableColumnHeader title='Tags' column={column} />,
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
  return (
    <>
      <div className='mt-6'>
        <QueryParamsDataTable columns={pollingStationColDefs} useQuery={usePollingStations} />
      </div>
    </>
  );
}
