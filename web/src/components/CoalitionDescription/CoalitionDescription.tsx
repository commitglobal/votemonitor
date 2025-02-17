import { useCurrentElectionRoundStore } from '@/context/election-round.store';
import { useCoalitionDetails } from '@/features/election-event/hooks/coalition-hooks';
import { useTranslation } from 'react-i18next';
import { Card, CardContent, CardHeader, CardTitle } from '../ui/card';
import { Separator } from '../ui/separator';

function CoalitionDescription() {
  const { t } = useTranslation();
  const currentElectionRoundId = useCurrentElectionRoundStore((s) => s.currentElectionRoundId);
  const { data: coalitionDetails } = useCoalitionDetails(currentElectionRoundId);

  return (
    <>
      {coalitionDetails?.isInCoalition ? (
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
      ) : null}
    </>
  );
}

export default CoalitionDescription;
