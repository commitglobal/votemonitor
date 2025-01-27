import { Button } from '@/components/ui/button';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Separator } from '@/components/ui/separator';
import { PencilIcon } from '@heroicons/react/24/outline';
import { useNavigate } from '@tanstack/react-router';

import { DateTimeFormat } from '@/common/formats';
import Layout from '@/components/layout/Layout';
import { format } from 'date-fns';
import { FC } from 'react';
import { NgoAdmin } from '../../models/NgoAdmin';
import { NgoBackButton, NgoBreadcrumbs } from '../NgoExtraComponents';
import { NgoAdminStatusBadge } from '../NgoStatusBadges';

interface NgoAdminDetailsViewProps {
  ngoId: string;
  ngoName: string;
  ngoAdmin: NgoAdmin;
}

export const NgoAdminDetailsView: FC<NgoAdminDetailsViewProps> = ({ ngoId, ngoName, ngoAdmin }) => {
  const navigate = useNavigate();
  const displayName = `${ngoAdmin.firstName} ${ngoAdmin.lastName}`;
  //TODO: Fix navigate to edit
  const navigateToEdit = (): void => {
    void navigate({
      //to: '/monitoring-observers/edit/$monitoringObserverId',
      params: { monitoringObserverId: ngoAdmin.id },
    });
  };

  return (
    <Layout
      title={displayName}
      backButton={<NgoBackButton ngoId={ngoId} />}
      breadcrumbs={
        <NgoBreadcrumbs ngoData={{ id: ngoId, name: ngoName }} adminData={{ id: ngoAdmin.id, name: displayName }} />
      }>
      <Card className='w-[800px] pt-0'>
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
            <p className='font-bold text-gray-700'>Name</p>
            <p className='font-normal text-gray-900'>{displayName}</p>
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
