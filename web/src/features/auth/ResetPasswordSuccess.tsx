import type { FunctionComponent } from '@/common/types';
import { SuccessIcon } from './SuccessIcon';

export function ResetPasswordSuccess(): FunctionComponent {
  return (
    <div className='max-w-sm m-auto  flex flex-col justify-center items-center gap-6 text-center text-lg'>
      <SuccessIcon />
      <p className='text-2xl font-bold'>Password changed!</p>

      <div className='flex gap-4 flex-col'>
        <p>
          Your password has been successfully reset. You can now go back to the Vote Monitor app and use your new
          password.
        </p>
      </div>
    </div>
  );
}
