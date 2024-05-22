import { createFileRoute, useNavigate } from '@tanstack/react-router';
import { electionRoundColDefs } from '@/features/election-round/models/ElectionRound';
import { QueryParamsDataTable } from '@/components/ui/DataTable/QueryParamsDataTable';
import Layout from '@/components/layout/Layout';
import { useCallback, type ReactElement } from 'react';
import CreateElectionRound from '@/features/election-round/components/CreateElectionRound';
import { useElectionRounds } from '@/features/election-round/queries';

function ElectionRounds(): ReactElement {
  const navigate = useNavigate();

  const navigateToElectionRound = useCallback(
    (electionRoundId: string) => {
      void navigate({ to: '/election-rounds/$electionRoundId', params: { electionRoundId } });
    },
    [navigate]
  );

  return (
    <Layout title={'Election Rounds'} actions={<CreateElectionRound />}>
      <QueryParamsDataTable
        columns={electionRoundColDefs}
        useQuery={useElectionRounds}
        onRowClick={navigateToElectionRound}
      />
    </Layout>
  );
}

export const Route = createFileRoute('/election-rounds/')({
  component: ElectionRounds,
});
