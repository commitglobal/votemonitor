import { Badge } from '@/components/ui/badge';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Separator } from '@/components/ui/separator';
import { cn } from '@/lib/utils';

import { useElectionRoundDetails } from '../../hooks/election-event-hooks';
import { useCurrentElectionRoundStore } from '@/context/election-round.store';

export default function ElectionEventDetails() {
    const currentElectionRoundId = useCurrentElectionRoundStore(s => s.currentElectionRoundId);
  const { data: electionEvent } = useElectionRoundDetails(currentElectionRoundId);

  return (
    <Card className='w-[800px] pt-0'>
      <CardHeader className='flex flex-column gap-2'>
        <div className='flex flex-row justify-between items-center'>
          <CardTitle className='text-xl'>Event details</CardTitle>
        </div>
        <Separator />
      </CardHeader>
      <CardContent className='flex flex-col gap-6 items-baseline'>
        <div className='flex flex-col gap-1'>
          <p className='text-gray-700 font-bold'>Election event title</p>
          <p className='text-gray-900 font-normal'>
            {electionEvent?.title}
          </p>
        </div>
        <div className='flex flex-col gap-1'>
          <p className='text-gray-700 font-bold'>Election event English title</p>
          <p className='text-gray-900 font-normal'>
            {electionEvent?.englishTitle}
          </p>
        </div>
        <div className='flex flex-col gap-1'>
          <p className='text-gray-700 font-bold'>Country</p>
          <p className='text-gray-900 font-normal'>{electionEvent?.country}</p>
        </div>
        <div className='flex flex-col gap-1'>
          <p className='text-gray-700 font-bold'>Start date</p>
          <p className='text-gray-900 font-normal'>{electionEvent?.startDate}</p>
        </div>

        <div className='flex flex-col gap-1'>
          <p className='text-gray-700 font-bold'>Status</p>
          <Badge
            className={cn({
              'text-slate-700 bg-slate-200': electionEvent?.status === 'NotStarted',
              'text-green-700 bg-green-200': electionEvent?.status === 'Started',
              'text-yellow-700 bg-yellow-200': electionEvent?.status === 'Archived'
            })}>
            {electionEvent?.status}
          </Badge>
        </div>
      </CardContent>
    </Card>
  );
}
