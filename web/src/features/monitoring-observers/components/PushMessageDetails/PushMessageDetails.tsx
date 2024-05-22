import Layout from '@/components/layout/Layout';
import { Card, CardContent, CardFooter, CardHeader, CardTitle } from '@/components/ui/card';
import { Separator } from '@/components/ui/separator';
import { useLoaderData } from '@tanstack/react-router';
import { format } from 'date-fns';

import { DateTimeFormat } from '@/common/formats';
import type { FunctionComponent } from '@/common/types';
export default function PushMessageDetails(): FunctionComponent {
  const pushMessage = useLoaderData({ from: '/monitoring-observers/push-messages/$id/view' });

  return (
    <Layout title=''>
      <Card className='w-[800px] pt-0'>
        <CardHeader className='flex flex-column gap-2'>
          <div className='flex flex-row justify-between items-center'>
            <CardTitle className='text-xl'>Push message details</CardTitle>
          </div>
          <Separator />
        </CardHeader>
        <CardContent className='grid grid-cols-2 h-96'>
          <div className='flex flex-col gap-1'>
            <div className='flex flex-col gap-1'>
              <p className='text-gray-700 font-bold'>Title</p>
              <p className='text-gray-900 font-normal'>
                {pushMessage.title}
              </p>
            </div>
            <div className='flex flex-col gap-1'>
              <p className='text-gray-700 font-bold'>Body</p>
              <p className='text-gray-900 font-normal'>
                {pushMessage.body}
              </p>
            </div>
            <div className='flex flex-col gap-1'>
              <p className='text-gray-700 font-bold'>Sent at</p>
              <p className='text-gray-900 font-normal'>
                {format(pushMessage.sentAt, DateTimeFormat)}
              </p>
            </div>
            <div className='flex flex-col gap-1'>
              <p className='text-gray-700 font-bold'>Senter name</p>
              <p className='text-gray-900 font-normal'>
                {pushMessage.sender}
              </p>
            </div>
          </div>
          <div className='flex flex-col gap-1 overflow-y-auto'>
            <div className='flex flex-col gap-1'>
              <p className='text-gray-700 font-bold'>Total targeted observers {pushMessage?.receivers?.length ?? 0}</p>
              {pushMessage?.receivers?.map((receiver) => (
                <p className='text-gray-900 font-normal' key={receiver.id}>{receiver.name}</p>
              ))}
            </div>
          </div>
        </CardContent>
        <CardFooter className='flex justify-between'></CardFooter>
      </Card>
    </Layout>
  );
}
