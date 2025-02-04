import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Separator } from '@/components/ui/separator';

import { ElectionRoundStatus } from '@/common/types';
import { AuthContext } from '@/context/auth.context';
import { useCurrentElectionRoundStore } from '@/context/election-round.store';
import { PencilIcon } from '@heroicons/react/24/outline';
import { Link } from '@tanstack/react-router';
import { useContext } from 'react';
import { useTranslation } from 'react-i18next';
import { useElectionRoundDetails } from '../../features/election-event/hooks/election-event-hooks';
import ElectionRoundStatusBadge from '../ElectionRoundStatusBadge/ElectionRoundStatusBadge';
import { Button } from '../ui/button';
import CoalitionDescription from '../CoalitionDescription/CoalitionDescription';

export default function ElectionEventDescription() {
  const { t } = useTranslation();
  const currentElectionRoundId = useCurrentElectionRoundStore((s) => s.currentElectionRoundId);
  const { data: electionEvent } = useElectionRoundDetails(currentElectionRoundId);
  const { userRole } = useContext(AuthContext);

  return (
    <div className='space-y-4'>
      <Card>
        <CardHeader className='flex gap-2 flex-column'>
          <div className='flex flex-row items-center justify-between'>
            <CardTitle className='text-2xl font-semibold leading-none tracking-tight'>
              {t('electionEvent.eventDetails.cardTitle')}
            </CardTitle>
            {userRole === 'PlatformAdmin' && (
              <div className='flex justify-end gap-4 px-6'>
                <Link to='/election-rounds/$electionRoundId/edit' params={{ electionRoundId: currentElectionRoundId }}>
                  <Button variant='ghost-primary'>
                    <PencilIcon className='w-[18px] mr-2 text-purple-900' />
                    <span className='text-base text-purple-900'>Edit</span>
                  </Button>
                </Link>
              </div>
            )}
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
            <ElectionRoundStatusBadge status={electionEvent?.status ?? ElectionRoundStatus.NotStarted} />
          </div>
        </CardContent>
      </Card>
      {userRole === 'NgoAdmin' && <CoalitionDescription />}
    </div>
  );
}
