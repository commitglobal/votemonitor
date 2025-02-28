import { Button } from '@/components/ui/button';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Separator } from '@/components/ui/separator';
import { PencilIcon } from '@heroicons/react/24/outline';
import { Link, useNavigate } from '@tanstack/react-router';

import { DateTimeFormat } from '@/common/formats';
import { BackButtonIcon } from '@/components/layout/Breadcrumbs/BackButton';
import Layout from '@/components/layout/Layout';
import { format } from 'date-fns';
import { FC } from 'react';
import { NgoAdmin } from '../../models/NgoAdmin';
import { NgoAdminStatusBadge } from '../NgoStatusBadges';

interface NgoAdminDetailsViewProps {
  ngoId: string;
  ngoAdmin: NgoAdmin;
}

export const NgoAdminDetailsView: FC<NgoAdminDetailsViewProps> = ({ ngoId, ngoAdmin }) => {
  const navigate = useNavigate();
  const displayName = `${ngoAdmin.firstName} ${ngoAdmin.lastName}`;
  const navigateToEdit = (): void => {
    void navigate({
      to: '/ngos/admin/$ngoId/$adminId/edit',
      params: { adminId: ngoAdmin.id, ngoId },
    });
  };

  return (
    <Layout
      title={displayName}
      backButton={
        <Link title='Go back' to='/ngos/view/$ngoId/$tab' params={{ ngoId, tab: 'admins' }} preload='intent'>
          <BackButtonIcon />
        </Link>
      }
      breadcrumbs={<></>}>
      <Card className='w-[600px] pt-0'>
        <CardHeader className='flex gap-2 flex-column'>
          <div className='flex flex-row items-center justify-between'>
            <CardTitle className='text-xl'>NGO admin details</CardTitle>
            <Button onClick={navigateToEdit} variant='ghost-primary'>
              <PencilIcon className='w-[18px] mr-2 text-purple-900' />
              <span className='text-base text-purple-900'>Edit</span>
            </Button>
          </div>
          <Separator />
        </CardHeader>
        <CardContent className='flex flex-col items-baseline gap-6'>
          <div className='flex flex-col gap-1'>
            <p className='font-bold text-gray-700'>First name</p>
            <p className='font-normal text-gray-900'>{ngoAdmin.firstName}</p>
          </div>
          <div className='flex flex-col gap-1'>
            <p className='font-bold text-gray-700'>Last name</p>
            <p className='font-normal text-gray-900'>{ngoAdmin.lastName}</p>
          </div>
          <div className='flex flex-col gap-1'>
            <p className='font-bold text-gray-700'>Email</p>
            <p className='font-normal text-gray-900'>{ngoAdmin.email}</p>
          </div>
          <div className='flex flex-col gap-1'>
            <p className='font-bold text-gray-700'>Phone</p>
            <p className='font-normal text-gray-900'>{ngoAdmin.phoneNumber}</p>
          </div>

          <div className='flex flex-col gap-1'>
            <p className='font-bold text-gray-700'>Last activity</p>
            <p className='font-normal text-gray-900'>
              {' '}
              {ngoAdmin.latestActivityAt ? format(ngoAdmin.latestActivityAt, DateTimeFormat) : '-'}
            </p>
          </div>
          <div className='flex flex-col gap-1'>
            <p className='font-bold text-gray-700'>Status</p>
            <NgoAdminStatusBadge status={ngoAdmin.status} />
          </div>
        </CardContent>
      </Card>
    </Layout>
  );
};
