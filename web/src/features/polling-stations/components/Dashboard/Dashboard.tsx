import { pollingStationColDefs, type PollingStation } from '../../models/PollingStation';
import { useState, type ReactElement } from 'react';
import { DataTable } from '@/components/ui/DataTable/DataTable';
import { type UseQueryResult, useQuery } from '@tanstack/react-query';
import type { PageParameters, PageResponse } from '@/common/types';
import { authApi } from '@/common/auth-api';
import type { PaginationState } from '@tanstack/react-table';

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
  const [pagination, setPagination] = useState<PaginationState>({ pageIndex: 0, pageSize: 10 });
  const updatePagination = (ps: PaginationState): void => {
    setPagination(ps);
  };
  const queryResult = usePollingStations({
    pageNumber: pagination.pageIndex + 1,
    pageSize: pagination.pageSize,
  });

  return (
    <>
      <div className='px-4 sm:px-0'>
        <h3 className='text-base font-semibold leading-7 text-gray-900'>Polling stations</h3>
      </div>
      <div className='mt-6'>
        <DataTable
          columns={pollingStationColDefs}
          queryResult={queryResult}
          pagination={pagination}
          updatePagination={updatePagination}
        />
      </div>
    </>
  );
}
