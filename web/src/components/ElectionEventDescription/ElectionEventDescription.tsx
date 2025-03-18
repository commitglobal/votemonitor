import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Separator } from '@/components/ui/separator';

import { ElectionRoundStatus } from '@/common/types';
import { AuthContext } from '@/context/auth.context';
import { useCurrentElectionRoundStore } from '@/context/election-round.store';
import {
  useArchiveElectionRound,
  useStartElectionRound,
  useUnarchiveElectionRound,
  useUnstartElectionRound,
} from '@/features/election-rounds/hooks';
import { PencilIcon } from '@heroicons/react/24/outline';
import { Link, useRouter } from '@tanstack/react-router';
import { ArchiveIcon, FileEdit, PlayIcon } from 'lucide-react';
import { useCallback, useContext } from 'react';
import { useTranslation } from 'react-i18next';
import { useElectionRoundDetails } from '../../features/election-event/hooks/election-event-hooks';
import CoalitionDescription from '../CoalitionDescription/CoalitionDescription';
import ElectionRoundStatusBadge from '../ElectionRoundStatusBadge/ElectionRoundStatusBadge';
import { useConfirm } from '../ui/alert-dialog-provider';
import { Button } from '../ui/button';
import { useToast } from '../ui/use-toast';

export default function ElectionEventDescription() {
  const { t } = useTranslation();
  const currentElectionRoundId = useCurrentElectionRoundStore((s) => s.currentElectionRoundId);
  const { data: electionEvent } = useElectionRoundDetails(currentElectionRoundId);
  const { userRole } = useContext(AuthContext);

  const { toast } = useToast();
  const confirm = useConfirm();

  const router = useRouter();

  const { mutate: unstartElectionRound } = useUnstartElectionRound();
  const { mutate: startElectionRound } = useStartElectionRound();
  const { mutate: archiveElectionRound } = useArchiveElectionRound();
  const { mutate: unarchiveElectionRound } = useUnarchiveElectionRound();

  const handleArchiveElectionRound = useCallback(async () => {
    if (
      await confirm({
        title: `Archive election round: ${electionEvent!.englishTitle}?`,
        body: 'Are you sure you want to archive this election round?',
      })
    ) {
      archiveElectionRound({
        electionRoundId: electionEvent!.id,
        onSuccess: () => {
          router.invalidate();

          toast({
            title: 'Election round archived successfully',
          });
        },
        onError: () =>
          toast({
            title: 'Error archiving election round',
            description: 'Please contact tech support',
            variant: 'destructive',
          }),
      });
    }
  }, [electionEvent, confirm]);

  const handleUnstartElectionRound = useCallback(async () => {
    if (
      await confirm({
        title: `Draft election round: ${electionEvent!.englishTitle}?`,
        body: 'Are you sure you want to draft this election round?',
      })
    ) {
      unstartElectionRound({
        electionRoundId: electionEvent!.id,
        onSuccess: () => {
          router.invalidate();

          toast({
            title: 'Election round drafted successfully',
          });
        },
        onError: () => {
          router.invalidate();
          toast({
            title: 'Error drafting election round',
            description: 'Please contact tech support',
            variant: 'destructive',
          });
        },
      });
    }
  }, [electionEvent, confirm]);

  const handleUnarchiveElectionRound = useCallback(async () => {
    if (
      await confirm({
        title: `Unarchive election round: ${electionEvent!.englishTitle}?`,
        body: 'Are you sure you want to unarchive this election round?',
      })
    ) {
      unarchiveElectionRound({
        electionRoundId: electionEvent!.id,
        onSuccess: () => {
          router.invalidate();
          toast({
            title: 'Election round unarchived successfully',
          });
        },
        onError: () =>
          toast({
            title: 'Error unarchiving election round',
            description: 'Please contact tech support',
            variant: 'destructive',
          }),
      });
    }
  }, [electionEvent, confirm]);

  const handleStartElectionRound = useCallback(async () => {
    if (
      await confirm({
        title: `Start election round: ${electionEvent!.englishTitle}?`,
        body: 'Are you sure you want to start this election round?',
      })
    ) {
      startElectionRound({
        electionRoundId: electionEvent!.id,
        onSuccess: () => {
          router.invalidate();
          toast({
            title: 'Election round started successfully',
          });
        },
        onError: () =>
          toast({
            title: 'Error starting election round',
            description: 'Please contact tech support',
            variant: 'destructive',
          }),
      });
    }
  }, [electionEvent, confirm]);

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
                {!(electionEvent!.status === ElectionRoundStatus.Archived) ? (
                  <Button onClick={handleArchiveElectionRound} variant='ghost-primary' className='text-yellow-900'>
                    <ArchiveIcon className='mr-2 h-4 w-4' />
                    Archive
                  </Button>
                ) : (
                  <Button onClick={handleUnarchiveElectionRound} variant='ghost-primary' className='text-green-900'>
                    <ArchiveIcon className='mr-2 h-4 w-4' />
                    Unarchive
                  </Button>
                )}

                {!(electionEvent!.status === ElectionRoundStatus.Started) ? (
                  <Button onClick={handleStartElectionRound} variant='ghost-primary' className='text-green-900'>
                    <PlayIcon className='mr-2 h-4 w-4' />
                    Start
                  </Button>
                ) : (
                  <Button onClick={handleUnstartElectionRound} variant='ghost-primary' className='text-slate-700'>
                    <FileEdit className='mr-2 h-4 w-4' />
                    Draft
                  </Button>
                )}
                <Link to='/election-rounds/$electionRoundId/edit' params={{ electionRoundId: currentElectionRoundId }}>
                  <Button variant='ghost-primary'>
                    <PencilIcon className='h-4 w-4 mr-2 text-purple-900' />
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
