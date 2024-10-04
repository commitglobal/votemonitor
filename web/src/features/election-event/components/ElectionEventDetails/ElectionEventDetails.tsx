import { Badge } from '@/components/ui/badge';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Separator } from '@/components/ui/separator';
import { cn } from '@/lib/utils';

import { useElectionRoundDetails } from '../../hooks/election-event-hooks';
import { useCurrentElectionRoundStore } from '@/context/election-round.store';
import { useTranslation } from 'react-i18next';

export default function ElectionEventDetails() {
  const { t } = useTranslation();
  const currentElectionRoundId = useCurrentElectionRoundStore(s => s.currentElectionRoundId);
  const { data: electionEvent } = useElectionRoundDetails(currentElectionRoundId);

  return (
    <Card className='w-[800px] pt-0'>
      <CardHeader className='flex gap-2 flex-column'>
        <div className='flex flex-row items-center justify-between'>
          <CardTitle className='text-xl'>{t('electionEvent.eventDetails.cardTitle')}</CardTitle>
        </div>
        <Separator />
      </CardHeader>
      <CardContent className='flex flex-col items-baseline gap-6'>
        <div className='flex flex-col gap-1'>
          <p className='font-bold text-gray-700'>{t('electionEvent.eventDetails.title')}</p>
          <p className='font-normal text-gray-900'>
            {electionEvent?.title}
          </p>
        </div>
        <div className='flex flex-col gap-1'>
          <p className='font-bold text-gray-700'>{t('electionEvent.eventDetails.englishTitle')}</p>
          <p className='font-normal text-gray-900'>
            {electionEvent?.englishTitle}
          </p>
        </div>
        <div className='flex flex-col gap-1'>
          <p className='font-bold text-gray-700'>{t('electionEvent.eventDetails.country')}</p>
          <p className='font-normal text-gray-900'>{electionEvent?.countryFullName}</p>
        </div>
        <div className='flex flex-col gap-1'>
          <p className='font-bold text-gray-700'>{t('electionEvent.eventDetails.startDate')}</p>
          <p className='font-normal text-gray-900'>{electionEvent?.startDate}</p>
        </div>

        <div className='flex flex-col gap-1'>
          <p className='font-bold text-gray-700'>{t('electionEvent.eventDetails.status')}</p>
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
