import type { ColumnDef } from '@tanstack/react-table';
import type { PollingStation } from '../../models/PollingStation';
import type { ReactElement } from 'react';
import { DataTable } from '@/components/ui/DataTable/DataTable';
import { type UseQueryResult, useQuery } from '@tanstack/react-query';
import type { PageParameters, PageResponse } from '@/common/types';
import { authApi } from '@/common/auth-api';

const columns: ColumnDef<PollingStation>[] = [
  {
    header: 'Address',
    accessorKey: 'address',
  },
  {
    header: 'County',
    accessorKey: 'tags.county',
  },
  {
    header: 'Locality',
    accessorKey: 'tags.locality',
  },
  {
    header: 'Section Number',
    accessorKey: 'tags.sectionNumber',
  },
  {
    header: 'Section Name',
    accessorKey: 'tags.sectionName',
  },
];

const pollingStationsQuery: PageParameters = {
  pageNumber: 1,
  pageSize: 5,
};

function usePollingStations(): UseQueryResult<PageResponse<PollingStation>, Error> {
  return useQuery({
    queryKey: ['pollingStations'],
    queryFn: async () => {
      const response = await authApi.post<PageResponse<PollingStation>>('/polling-stations:list', pollingStationsQuery);

      if (response.status !== 200) {
        throw new Error('Failed to fetch polling stations');
      }

      return response.data;
    },
  });
}

export default function PollingStationsDashboard(): ReactElement {
  // const queryClient = useQueryClient();
  const { status, data, error } = usePollingStations();

  return (
    <>
      <div className='px-4 sm:px-0'>
        <h3 className='text-base font-semibold leading-7 text-gray-900'>Polling stations</h3>
      </div>
      {status === 'loading' ? (
        'Loading...'
      ) : status === 'error' ? (
        <span>Error: {error.message}</span>
      ) : (
        <>
          <div className='bg-white shadow rounded-md mt-6'>
            <DataTable columns={columns} data={data.items} />
          </div>
        </>
      )}
    </>
  );
}
