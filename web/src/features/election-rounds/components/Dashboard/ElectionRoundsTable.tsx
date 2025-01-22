import { QueryParamsDataTable } from '@/components/ui/DataTable/QueryParamsDataTable';
import { useNavigate } from '@tanstack/react-router';
import { useElectionRounds } from '../../queries';
import { electionRoundColDefs } from './columns-defs';

function ElectionRoundsTable() {
  const navigate = useNavigate();

  return (
    <QueryParamsDataTable
      columns={electionRoundColDefs}
      useQuery={useElectionRounds}
      onRowClick={(electionRoundId: string) =>
        navigate({ to: `/election-rounds/$electionRoundId`, params: { electionRoundId } })
      }
    />
  );
}

export default ElectionRoundsTable;
