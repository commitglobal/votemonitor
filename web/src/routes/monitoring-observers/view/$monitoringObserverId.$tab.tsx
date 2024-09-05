import { monitoringObserverDetailsQueryOptions } from '@/common/queryOptions';
import type { FunctionComponent } from '@/common/types';
import MonitoringObserverDetails from '@/features/monitoring-observers/components/MonitoringObserverDetails/MonitoringObserverDetails';
import {
  monitoringObserverDetailsRouteSearchSchema
} from '@/features/monitoring-observers/models/monitoring-observer';
import { redirectIfNotAuth } from '@/lib/utils';
import { createFileRoute } from '@tanstack/react-router';

export const Route = createFileRoute('/monitoring-observers/view/$monitoringObserverId/$tab')({
  beforeLoad: () => {
    redirectIfNotAuth();

  },
  component: Details,
  loader: ({ context: { queryClient, currentElectionRoundContext }, params: { monitoringObserverId } }) => {
    const electionRoundId = currentElectionRoundContext.getState().currentElectionRoundId;

    return queryClient.ensureQueryData(monitoringObserverDetailsQueryOptions(electionRoundId, monitoringObserverId));
  },
  validateSearch: monitoringObserverDetailsRouteSearchSchema
});

function Details(): FunctionComponent {
  return (
    <div className='p-2'>
      <MonitoringObserverDetails />
    </div>
  );
}