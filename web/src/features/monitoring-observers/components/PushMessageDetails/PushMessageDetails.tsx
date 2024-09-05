import Layout from '@/components/layout/Layout';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Separator } from '@/components/ui/separator';
import { format } from 'date-fns';

import { DateTimeFormat } from '@/common/formats';
import type { FunctionComponent } from '@/common/types';
import { NavigateBack } from '@/components/NavigateBack/NavigateBack';
import { useCurrentElectionRoundStore } from '@/context/election-round.store';
import { Route } from '@/routes/monitoring-observers/push-messages.$id';
import { pushMessageDetailsQueryOptions } from '@/routes/monitoring-observers/push-messages.$id_.view';
import { useSuspenseQuery } from '@tanstack/react-query';

export default function PushMessageDetails(): FunctionComponent {
  const { id } = Route.useParams()
  const currentElectionRoundId = useCurrentElectionRoundStore(s => s.currentElectionRoundId);
  const { data: pushMessage } = useSuspenseQuery(pushMessageDetailsQueryOptions(currentElectionRoundId, id));

  return (
    <Layout backButton={<NavigateBack to='/monitoring-observers/$tab' params={{ tab: 'push-messages' }} />} title=''>
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
              <p className='text-gray-900 font-normal'>{pushMessage.title}</p>
            </div>
            <div className='flex flex-col gap-1'>
              <p className='text-gray-700 font-bold'>Body</p>
              <p className='text-gray-900 font-normal'>{pushMessage.body}</p>
            </div>
            <div className='flex flex-col gap-1'>
              <p className='text-gray-700 font-bold'>Sent at</p>
              <p className='text-gray-900 font-normal'>{format(pushMessage.sentAt, DateTimeFormat)}</p>
            </div>
            <div className='flex flex-col gap-1'>
              <p className='text-gray-700 font-bold'>Sender name</p>
              <p className='text-gray-900 font-normal'>{pushMessage.sender}</p>
            </div>
          </div>
          <div className='flex flex-col gap-1 overflow-y-auto'>
            <div className='flex flex-col gap-1'>
              <p className='text-gray-700 font-bold'>Total targeted observers {pushMessage?.receivers?.length ?? 0}</p>
              {pushMessage?.receivers?.map((receiver) => (
                <p className='text-gray-900 font-normal' key={receiver.id}>
                  {receiver.name}
                </p>
              ))}
            </div>
          </div>
        </CardContent>
      </Card>
    </Layout>
  );
}
