import { observerColDefs, type Observer } from '../../models/Observer';
import type { ReactElement } from 'react';
import { type UseQueryResult, useQuery } from '@tanstack/react-query';
import type { PageParameters, PageResponse } from '@/common/types';
import { authApi } from '@/common/auth-api';
import { QueryParamsDataTable } from '@/components/ui/DataTable/QueryParamsDataTable';

function useObservers(p: PageParameters): UseQueryResult<PageResponse<Observer>, Error> {
  return useQuery({
    queryKey: ['observers', p.pageNumber, p.pageSize],
    queryFn: async () => {
      const response = await authApi.get<PageResponse<Observer>>('/observers', {
        params: {
          NameFilter: '',
          PageNumber: p.pageNumber,
          PageSize: p.pageSize,
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
  return (
    <>
      <header>
        <div className='mx-auto max-w-7xl px-4 py-6 sm:px-6 lg:px-8'>
          <h1 className='text-3xl font-bold tracking-tight text-gray-900'>Observers</h1>
        </div>
      </header>
      <main>
        <div className='bg-white mx-auto max-w-7xl py-6 sm:px-6 lg:px-8 '>
          <QueryParamsDataTable columns={observerColDefs} usePagedQuery={useObservers} />
        </div>
      </main>
    </>
  );
}
