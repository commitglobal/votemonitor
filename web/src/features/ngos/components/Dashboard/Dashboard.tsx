import { ngoColDefs, type NGO } from '../../models/NGO';
import type { ReactElement } from 'react';
import { type UseQueryResult, useQuery } from '@tanstack/react-query';
import type { PageParameters, PageResponse } from '@/common/types';
import { authApi } from '@/common/auth-api';
import { QueryParamsDataTable } from '@/components/ui/DataTable/QueryParamsDataTable';
import Layout from '@/components/layout/Layout';

function useNGOs(p: PageParameters): UseQueryResult<PageResponse<NGO>, Error> {
  return useQuery({
    queryKey: ['ngos', p.pageNumber, p.pageSize],
    queryFn: async () => {
      const response = await authApi.get<PageResponse<NGO>>('/ngos', {
        params: {
          NameFilter: '',
          PageNumber: p.pageNumber,
          PageSize: p.pageSize,
          Status: 'Active',
        },
      });

      if (response.status !== 200) {
        throw new Error('Failed to fetch ngos');
      }

      return response.data;
    },
  });
}

export default function NGOsDashboard(): ReactElement {
  return (
    <Layout title={'NGOs'}>
      <QueryParamsDataTable columns={ngoColDefs} usePagedQuery={useNGOs} />
    </Layout>
  );
}
