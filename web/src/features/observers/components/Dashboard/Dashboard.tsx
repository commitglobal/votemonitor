import { observerColDefs, type Observer, type ObserverSearch } from '../../models/Observer';
import type { ReactElement } from 'react';
import { DataTable } from '@/components/ui/DataTable/DataTable';
import { type UseQueryResult, useQuery } from '@tanstack/react-query';
import type { PageResponse } from '@/common/types';
import { authApi } from '@/common/auth-api';
import { ObserverDashboardRoute } from '../../ObserverRoutes';
import { useNavigate } from '@tanstack/router';
import type { PaginationState } from '@tanstack/react-table';

function useObservers(search: ObserverSearch): UseQueryResult<PageResponse<Observer>, Error> {
  return useQuery({
    queryKey: ['observers', search.PageNumber, search.PageSize],
    queryFn: async () => {
      const response = await authApi.get<PageResponse<Observer>>('/observers', {
        params: search,
      });

      if (response.status !== 200) {
        throw new Error('Failed to fetch observers');
      }

      return response.data;
    },
  });
}

export default function ObserversDashboard(): ReactElement {
  const searchParameters = ObserverDashboardRoute.useSearch();
  const pagination: PaginationState = {
    pageIndex: searchParameters.PageNumber - 1,
    pageSize: searchParameters.PageSize,
  };
  const navigate = useNavigate({ from: ObserverDashboardRoute.id });

  const updatePagination = (ps: PaginationState): void => {
    navigate({
      search: {
        PageNumber: ps.pageIndex + 1,
        PageSize: ps.pageSize,
      },
    }).catch(() => {});
  };
  const queryResult = useObservers(searchParameters);

  return (
    <>
      <header>
        <div className='mx-auto max-w-7xl px-4 py-6 sm:px-6 lg:px-8'>
          <h1 className='text-3xl font-bold tracking-tight text-gray-900'>Observers</h1>
        </div>
      </header>
      <main>
        <div className='bg-white mx-auto max-w-7xl py-6 sm:px-6 lg:px-8 '>
          <DataTable
            columns={observerColDefs}
            queryResult={queryResult}
            pagination={pagination}
            updatePagination={updatePagination}
          />
        </div>
      </main>
    </>
  );
}
