import TableTagList from '@/components/table-tag-list/TableTagList';
import { Badge } from '@/components/ui/badge';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Separator } from '@/components/ui/separator';
import { PencilIcon } from '@heroicons/react/24/outline';
import { useNavigate } from '@tanstack/react-router';

import { DateTimeFormat } from '@/common/formats';
import { monitoringObserverDetailsQueryOptions } from '@/common/queryOptions';
import type { FunctionComponent } from '@/common/types';
import { Route } from '@/routes/monitoring-observers/view/$monitoringObserverId.$tab';
import { useSuspenseQuery } from '@tanstack/react-query';
import { format } from 'date-fns';
import { useCurrentElectionRoundStore } from '@/context/election-round.store';

export default function MonitoringObserverDetailsView(): FunctionComponent {
  const { monitoringObserverId } = Route.useParams();
    const currentElectionRoundId = useCurrentElectionRoundStore(s => s.currentElectionRoundId);
  const monitoringObserverQuery = useSuspenseQuery(monitoringObserverDetailsQueryOptions(currentElectionRoundId, monitoringObserverId));
  const monitoringObserver = monitoringObserverQuery.data;

  const navigate = useNavigate();
  const navigateToEdit = (): void => {
    void navigate({
      to: '/monitoring-observers/edit/$monitoringObserverId',
      params: { monitoringObserverId: monitoringObserver.id },
    });
  };

  return (
    <Card className='w-[800px] pt-0'>
      <CardHeader className='flex flex-column gap-2'>
        <div className='flex flex-row justify-between items-center'>
          <CardTitle className='text-xl'>Monitoring observer details</CardTitle>
          <Button onClick={navigateToEdit} variant='ghost-primary'>
            <PencilIcon className='w-[18px] mr-2 text-purple-900' />
            <span className='text-base text-purple-900'>Edit</span>
          </Button>
        </div>
        <Separator />
      </CardHeader>
      <CardContent className='flex flex-col gap-6 items-baseline'>
        <div className='flex flex-col gap-1'>
          <p className='text-gray-700 font-bold'>Name</p>
          <p className='text-gray-900 font-normal'>
            {monitoringObserver.firstName} {monitoringObserver.lastName}
          </p>
        </div>
        <div className='flex flex-col gap-1'>
          <p className='text-gray-700 font-bold'>Email</p>
          <p className='text-gray-900 font-normal'>{monitoringObserver.email}</p>
        </div>
        <div className='flex flex-col gap-1'>
          <p className='text-gray-700 font-bold'>Phone</p>
          <p className='text-gray-900 font-normal'>{monitoringObserver.phoneNumber}</p>
        </div>
        <div className='flex flex-col gap-1'>
          <p className='text-gray-700 font-bold'>Tags</p>
          <TableTagList tags={monitoringObserver.tags} />
        </div>
        <div className='flex flex-col gap-1'>
          <p className='text-gray-700 font-bold'>Last activity</p>
          <p className='text-gray-900 font-normal'> {monitoringObserver.latestActivityAt ? format(monitoringObserver.latestActivityAt, DateTimeFormat) : '-'}</p>
        </div>
        <div className='flex flex-col gap-1'>
          <p className='text-gray-700 font-bold'>Status</p>
          <Badge className={'badge-' + monitoringObserver.status}>{monitoringObserver.status}</Badge>
        </div>
      </CardContent>
    </Card>
  );
}
