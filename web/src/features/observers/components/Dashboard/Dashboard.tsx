import { observerColDefs, type Observer } from '../../models/Observer';
import type { ReactElement } from 'react';
import { DataTable } from '@/components/ui/DataTable/DataTable';
import { type UseQueryResult, useQuery } from '@tanstack/react-query';
import type { PageParameters, PageResponse } from '@/common/types';
import { authApi } from '@/common/auth-api';
import { ObserverDashboardRoute } from '../../ObserverRoutes';

function useObservers({ pageNumber, pageSize }: PageParameters): UseQueryResult<PageResponse<Observer>, Error> {
  return useQuery({
    queryKey: ['observers', pageNumber, pageSize],
    queryFn: async () => {
      const response = await authApi.get<PageResponse<Observer>>('/observers', {
        params: {
          PageNumber: pageNumber,
          PageSize: pageSize,
          Status: 'Active',
        },
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
  // TODO - use search parameters to filter observers
  // TODO - update search parameters when filters change

  return (
    <>
      <header>
        <div className='mx-auto max-w-7xl px-4 py-6 sm:px-6 lg:px-8'>
          <h1 className='text-3xl font-bold tracking-tight text-gray-900'>Observers</h1>
        </div>
      </header>
      <main>
        <div className='bg-white mx-auto max-w-7xl py-6 sm:px-6 lg:px-8 '>
          <DataTable columns={observerColDefs} pagedQuery={useObservers} />
        </div>
      </main>
    </>
  );
}
