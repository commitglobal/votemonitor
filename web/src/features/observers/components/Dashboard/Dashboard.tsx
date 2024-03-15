import { observerColDefs, type Observer } from '../../models/Observer';
import type { ReactElement } from 'react';
import { type UseQueryResult, useQuery } from '@tanstack/react-query';
import type { PageParameters, PageResponse } from '@/common/types';
import { authApi } from '@/common/auth-api';
import { QueryParamsDataTable } from '@/components/ui/DataTable/QueryParamsDataTable';
import Layout from '@/components/layout/Layout';
import Panel from '@/components/layout/Panel/Panel';

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
    <Layout title={'Observers'}>
      <Panel>
        <QueryParamsDataTable columns={observerColDefs} usePagedQuery={useObservers} />
      </Panel>
    </Layout>
  );
}
