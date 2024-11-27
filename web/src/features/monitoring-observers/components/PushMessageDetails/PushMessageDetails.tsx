import Layout from '@/components/layout/Layout';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Separator } from '@/components/ui/separator';
import { format } from 'date-fns';

import { DateTimeFormat } from '@/common/formats';
import type { FunctionComponent } from '@/common/types';
import { NavigateBack } from '@/components/NavigateBack/NavigateBack';
import { useCurrentElectionRoundStore } from '@/context/election-round.store';
import { cn } from '@/lib/utils';
import { pushMessageDetailsQueryOptions, Route } from '@/routes/monitoring-observers/push-messages.$id_.view';
import { CheckIcon } from '@heroicons/react/24/outline';
import { useSuspenseQuery } from '@tanstack/react-query';

export default function PushMessageDetails(): FunctionComponent {
  const { id } = Route.useParams();
  const currentElectionRoundId = useCurrentElectionRoundStore((s) => s.currentElectionRoundId);
  const { data: pushMessage } = useSuspenseQuery(pushMessageDetailsQueryOptions(currentElectionRoundId, id));

  return (
    <Layout backButton={<NavigateBack to='/monitoring-observers/$tab' params={{ tab: 'push-messages' }} />} 
    breadcrumbs={<></>}
    title={id}>
      <Card className='w-[800px] pt-0'>
        <CardHeader className='flex gap-2 flex-column'>
          <div className='flex flex-row items-center justify-between'>
            <CardTitle className='text-xl'>Push message details</CardTitle>
          </div>
          <Separator />
        </CardHeader>
        <CardContent className='grid grid-cols-2'>
          <div className='flex flex-col gap-1'>
            <div className='flex flex-col gap-1'>
              <p className='font-bold text-gray-700'>Title</p>
              <p className='font-normal text-gray-900'>{pushMessage.title}</p>
            </div>

            <div className='flex flex-col gap-1'>
              <p className='font-bold text-gray-700'>Sent at</p>
              <p className='font-normal text-gray-900'>{format(pushMessage.sentAt, DateTimeFormat)}</p>
            </div>
            <div className='flex flex-col gap-1'>
              <p className='font-bold text-gray-700'>Sender name</p>
              <p className='font-normal text-gray-900'>{pushMessage.sender}</p>
            </div>
          </div>
          <div className='flex flex-col gap-1 overflow-y-auto'>
            <div className='flex flex-col gap-1 h-36'>
              <p className='font-bold text-gray-700'>Total targeted observers {pushMessage?.receivers?.length ?? 0}</p>
              {pushMessage?.receivers?.map((receiver) => (
                <div className='flex gap-1' key={receiver.id}>
                  <p className='font-normal text-gray-900'>
                    {receiver.name}
                  </p>
                  {receiver.hasReadNotification && (
                    <CheckIcon title='Notification read' className='self-center w-4 h-4' color='#147638' />
                  )}
                </div>
              ))}
            </div>
          </div>
          <div className='flex flex-col col-span-2 gap-2 mt-8'>
            <p className='font-bold text-gray-700'>Body</p>
            <div
              dangerouslySetInnerHTML={{ __html: pushMessage.body }}
              className={cn('p-3 border rounded-lg prose max-w-none [&_ol]:list-decimal [&_ul]:list-disc')}
            />
          </div>
        </CardContent>
      </Card>
    </Layout>
  );
}
