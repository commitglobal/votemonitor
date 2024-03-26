import { pollingStationColDefs, type PollingStation } from '../../models/PollingStation';
import type { ReactElement } from 'react';
import { DataTable } from '@/components/ui/DataTable/DataTable';
import { type UseQueryResult, useQuery } from '@tanstack/react-query';
import type { PageParameters, PageResponse } from '@/common/types';
import { authApi } from '@/common/auth-api';

function usePollingStations({
  pageNumber,
  pageSize,
}: PageParameters): UseQueryResult<PageResponse<PollingStation>, Error> {
  return useQuery({
    queryKey: ['pollingStations', pageNumber, pageSize],
    queryFn: async () => {
      const response = await authApi.post<PageResponse<PollingStation>>('/polling-stations:list', {
        pageNumber,
        pageSize,
      });

      if (response.status !== 200) {
        throw new Error('Failed to fetch polling stations');
      }

      return response.data;
    },
  });
}

export default function PollingStationsDashboard(): ReactElement {
  return (
    <>
      <div className='px-4 sm:px-0'>
        <h3 className='text-base font-semibold leading-7 text-gray-900'>Polling stations</h3>
      </div>
      <div className='mt-6'>
        <DataTable columns={pollingStationColDefs} useQuery={usePollingStations} />
      </div>
    </>
  );
}
