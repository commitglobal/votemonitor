
import { authApi } from '@/common/auth-api'
import { DataTableParameters, PageResponse } from '@/common/types'
import Layout from '@/components/layout/Layout'
import { QueryParamsDataTable } from '@/components/ui/DataTable/QueryParamsDataTable'
import { ElectionRound, electionRoundColDefs } from '@/features/election-round/models/ElectionRound'
import { UseQueryResult, useQuery } from '@tanstack/react-query'
import { createFileRoute } from '@tanstack/react-router'

export const Route = createFileRoute('/election-rounds/')({
  component: ElectionEvents,
})

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


function ElectionEvents() {
  return <Layout title={'Election Rounds'}>
    <QueryParamsDataTable columns={electionRoundColDefs} useQuery={useElectionRounds} />
  </Layout>
}