import { Observer } from '../../models/observer';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { FC } from 'react';
import { PencilIcon } from '@heroicons/react/24/outline';
import { Badge } from '@/components/ui/badge';
import { Button } from '@/components/ui/button';
import { Separator } from '@/components/ui/separator';
import { useNavigate } from '@tanstack/react-router';

interface ObserverDetailsProps {
  observer: Observer;
}

export const ObserverPersonalDetails: FC<ObserverDetailsProps> = ({ observer }: ObserverDetailsProps) => {
  const observerId = observer.id;
  const navigate = useNavigate();
  const navigateToEdit = () => {
    navigate({ to: `/observers/$observerId/edit`, params: { observerId } });
  };

  return (
    <Card className='w-[800px] pt-0'>
      <CardHeader className='flex flex-column gap-2'>
        <div className='flex flex-row justify-between items-center'>
          <CardTitle className='text-xl'>Observers details</CardTitle>
          <Button onClick={navigateToEdit} variant='ghost-primary'>
            <PencilIcon className='w-[18px] mr-2 text-purple-900' />
            <span className='text-base text-purple-900'>Edit</span>
          </Button>
        </div>
        <Separator />
      </CardHeader>
      <CardContent className='flex flex-col gap-6 items-baseline'>
        <div className='flex flex-col gap-1'>
          <p className='text-gray-700 font-bold'>First name</p>
          <p className='text-gray-900 font-normal'>{observer.firstName}</p>
        </div>
        <div className='flex flex-col gap-1'>
          <p className='text-gray-700 font-bold'>Last name</p>
          <p className='text-gray-900 font-normal'>{observer.lastName}</p>
        </div>
        <div className='flex flex-col gap-1'>
          <p className='text-gray-700 font-bold'>Email</p>
          <p className='text-gray-900 font-normal'>{observer.email}</p>
        </div>
        <div className='flex flex-col gap-1'>
          <p className='text-gray-700 font-bold'>Phone</p>
          <p className='text-gray-900 font-normal'>{observer.phoneNumber}</p>
        </div>
        <div className='flex flex-col gap-1'>
          <p className='text-gray-700 font-bold'>Tags</p>
          <p className='text-gray-900 font-normal'>N/A</p>
        </div>

        <div className='flex flex-col gap-1'>
          <p className='text-gray-700 font-bold'>Status</p>
          <Badge className={'badge-' + observer.status}>{observer.status}</Badge>
        </div>
      </CardContent>
    </Card>
  );
};
