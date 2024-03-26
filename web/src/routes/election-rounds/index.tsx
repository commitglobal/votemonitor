import { createFileRoute } from '@tanstack/react-router';
import { electionRoundColDefs } from '@/features/election-round/models/ElectionRound';
import { QueryParamsDataTable } from '@/components/ui/DataTable/QueryParamsDataTable';
import Layout from '@/components/layout/Layout';
import type { ReactElement } from 'react';
import CreateElectionRound from '@/features/election-round/components/CreateElectionRound';
import { useElectionRounds } from '@/features/election-round/queries';

function ElectionRounds(): ReactElement {
  return (
    <Layout title={'Election Rounds'} actions={(
      <CreateElectionRound />
    )}>
      <QueryParamsDataTable columns={electionRoundColDefs} useQuery={useElectionRounds} />
    </Layout>
  );
}

export const Route = createFileRoute('/election-rounds/')({
  component: ElectionRounds,
});
