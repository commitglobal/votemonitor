import { ngoColDefs, type NGO } from '../../models/NGO';
import { useCallback, type ReactElement } from 'react';
import { type UseQueryResult, useQuery } from '@tanstack/react-query';
import type { DataTableParameters, PageResponse } from '@/common/types';
import { authApi } from '@/common/auth-api';
import { QueryParamsDataTable } from '@/components/ui/DataTable/QueryParamsDataTable';
import Layout from '@/components/layout/Layout';
import { useNavigate } from '@tanstack/react-router';

function useNGOs(p: DataTableParameters): UseQueryResult<PageResponse<NGO>, Error> {
  return useQuery({
    queryKey: ['ngos', p.pageNumber, p.pageSize, p.sortColumnName, p.sortOrder],
    queryFn: async () => {
      const response = await authApi.get<PageResponse<NGO>>('/ngos', {
        params: {
          PageNumber: p.pageNumber,
          PageSize: p.pageSize,
          SortColumnName: p.sortColumnName,
          SortOrder: p.sortOrder,
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
  const navigate = useNavigate();

  const navigateToNgo = useCallback(
    (ngoId: string) => {
      navigate({ to: '/ngos/$ngoId', params: { ngoId } });
    },
    [navigate]
  );

  return (
    <Layout title={'NGOs'}>
      <QueryParamsDataTable columns={ngoColDefs} useQuery={useNGOs} onRowClick={navigateToNgo} />
    </Layout>
  );
}
