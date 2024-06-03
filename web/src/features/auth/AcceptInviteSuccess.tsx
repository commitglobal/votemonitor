import type { FunctionComponent } from '@/common/types';
import { SuccessIcon } from './SuccessIcon';

export function AcceptInviteSuccess(): FunctionComponent {
  return (
    <div className='max-w-sm m-auto  flex flex-col justify-center items-center gap-6 text-center text-lg'>
      <SuccessIcon />
      <p className='text-2xl font-bold'>Success! Your Account is Set Up</p>

      <div className='flex gap-4 flex-col'>
        <p>Congratulations! Your account has been set up successfully.</p>
        <p>
          Download the Vote Monitor app below to start monitoring the elections to monitor the elections you've been
          assigned to by your organization.
        </p>
        <p>
          <a
            href='https://play.google.com/store/apps/details?id=org.commitglobal.votemonitor.app&pcampaignid=web_share'
            className='hover:underline text-purple-500'
            target='_blank'
            rel='noreferrer'>
            Google Play Store
          </a>{' '}
          |{' '}
          <a
            href='https://apps.apple.com/ro/app/vote-monitor/id6478601394'
            className='hover:underline text-purple-500'
            target='_blank'
            rel='noreferrer'>
            App Store
          </a>
        </p>
        <p>Thank you for your commitment to ensuring fair and transparent elections</p>
      </div>
    </div>
  );
}
