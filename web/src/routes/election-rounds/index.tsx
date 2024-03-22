import { authApi } from '@/common/auth-api';
import { createFileRoute } from '@tanstack/react-router';
import { electionRoundColDefs, type ElectionRound } from '@/features/election-round/models/ElectionRound';
import { QueryParamsDataTable } from '@/components/ui/DataTable/QueryParamsDataTable';
import { useQuery, type UseQueryResult } from '@tanstack/react-query';
import Layout from '@/components/layout/Layout';
import type { DataTableParameters, PageResponse } from '@/common/types';
import type { ReactElement } from 'react';
import CreateElectionRoundButton from '@/features/election-round/components/CreateElectionRoundButton';

function useElectionRounds(p: DataTableParameters): UseQueryResult<PageResponse<ElectionRound>, Error> {
  return useQuery({
    queryKey: ['electionRounds', p.pageNumber, p.pageSize, p.sortColumnName, p.sortOrder],
    queryFn: async () => {
      const response = await authApi.get<PageResponse<ElectionRound>>('/election-rounds', {
        params: {
          PageNumber: p.pageNumber,
          PageSize: p.pageSize,
          SortColumnName: p.sortColumnName,
          SortOrder: p.sortOrder,
        },
      });

      if (response.status !== 200) {
        throw new Error('Failed to fetch electionRounds');
      }

      return response.data;
    },
  });
}

function ElectionRounds(): ReactElement {
  return (
    <Layout title={'Election Rounds'} actions={(
      <CreateElectionRoundButton />
    )}>
      <QueryParamsDataTable columns={electionRoundColDefs} useQuery={useElectionRounds} />
    </Layout>
  );
}

export const Route = createFileRoute('/election-rounds/')({
  component: ElectionRounds,
});
