import { Badge } from '@/components/ui/badge';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Separator } from '@/components/ui/separator';
import { cn } from '@/lib/utils';

import { useCurrentElectionRoundStore } from '@/context/election-round.store';
import { useTranslation } from 'react-i18next';
import { useElectionRoundDetails } from '../../hooks/election-event-hooks';
import { useCoalitionDetails } from '../../hooks/coalition-hooks';

export default function ElectionEventDetails() {
  const { t } = useTranslation();
  const currentElectionRoundId = useCurrentElectionRoundStore((s) => s.currentElectionRoundId);
  const { data: electionEvent } = useElectionRoundDetails(currentElectionRoundId);
  const { data: coalitionDetails } = useCoalitionDetails(currentElectionRoundId);

  return (
    <div className='space-y-4'>
      <Card>
        <CardHeader className='flex gap-2 flex-column'>
          <div className='flex flex-row items-center justify-between'>
            <CardTitle className='text-2xl font-semibold leading-none tracking-tight'>
              {t('electionEvent.eventDetails.cardTitle')}
            </CardTitle>
          </div>
          <Separator />
        </CardHeader>
        <CardContent className='flex flex-col items-baseline gap-6'>
          <div className='flex flex-col gap-1'>
            <p className='font-bold text-gray-700'>{t('electionEvent.eventDetails.title')}</p>
            <p className='font-normal text-gray-900'>{electionEvent?.title}</p>
          </div>
          <div className='flex flex-col gap-1'>
            <p className='font-bold text-gray-700'>{t('electionEvent.eventDetails.englishTitle')}</p>
            <p className='font-normal text-gray-900'>{electionEvent?.englishTitle}</p>
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
                'text-yellow-700 bg-yellow-200': electionEvent?.status === 'Archived',
              })}>
              {electionEvent?.status}
            </Badge>
          </div>
        </CardContent>
      </Card>
      {coalitionDetails && (
        <Card>
          <CardHeader className='flex gap-2 flex-column'>
            <div className='flex flex-row items-center justify-between'>
              <CardTitle className='text-2xl font-semibold leading-none tracking-tight'>
                {t('electionEvent.eventDetails.coalition.cardTitle')}
              </CardTitle>
            </div>
            <Separator />
          </CardHeader>
          <CardContent className='flex flex-col items-baseline gap-6'>
            <div className='flex flex-col gap-1'>
              <p className='font-bold text-gray-700'>{t('electionEvent.eventDetails.coalition.coalitionName')}</p>
              <p className='font-normal text-gray-900'>{coalitionDetails.name}</p>
            </div>

            <div className='flex flex-col gap-1'>
              <p className='font-bold text-gray-700'>{t('electionEvent.eventDetails.coalition.leaderName')}</p>
              <p className='font-normal text-gray-900'>{coalitionDetails.leaderName}</p>
            </div>

            <div className='flex flex-col gap-1'>
              <p className='font-bold text-gray-700'>{t('electionEvent.eventDetails.coalition.members')}</p>
              <p className='font-normal text-gray-900'>{coalitionDetails?.members.map((m) => m.name).join(', ')}</p>
            </div>
          </CardContent>
        </Card>
      )}
    </div>
  );
}
