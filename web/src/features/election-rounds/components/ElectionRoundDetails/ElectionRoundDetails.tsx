import Layout from '@/components/layout/Layout';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Separator } from '@/components/ui/separator';
import { Route } from '@/routes/election-rounds/$electionRoundId';
import { PencilIcon } from '@heroicons/react/24/outline';
import { useSuspenseQuery } from '@tanstack/react-query';
import { Link } from '@tanstack/react-router';
import { electionRoundDetailsQueryOptions } from '../../queries';
import ElectionRoundStatusBadge from '../ElectionRoundStatusBadge/ElectionRoundStatusBadge';

function ElectionRoundDetails() {
  const { electionRoundId } = Route.useParams();
  const { data: electionRound } = useSuspenseQuery(electionRoundDetailsQueryOptions(electionRoundId));

  return (
    <Layout title={electionRound.title} subtitle={electionRound.englishTitle} enableBreadcrumbs={false}>
      <Card>
        <CardHeader>
          <CardTitle>Event details</CardTitle>
          <div className='flex justify-end gap-4 px-6'>
            <Link to='/election-rounds/$electionRoundId/edit' params={{ electionRoundId }}>
              <Button variant='ghost-primary'>
                <PencilIcon className='w-[18px] mr-2 text-purple-900' />
                <span className='text-base text-purple-900'>Edit</span>
              </Button>
            </Link>
          </div>

          <Separator />
        </CardHeader>
        <CardContent>
          <div className='flex flex-col gap-1'>
            <p className='font-bold text-gray-700'>Title</p>
            <p className='font-normal text-gray-900'>{electionRound.title}</p>
          </div>
          <div className='flex flex-col gap-1'>
            <p className='font-bold text-gray-700'>English title</p>
            <p className='font-normal text-gray-900'>{electionRound.englishTitle}</p>
          </div>
          <div className='flex flex-col gap-1'>
            <p className='font-bold text-gray-700'>Start date</p>
            <p className='font-normal text-gray-900'>{electionRound.startDate}</p>
          </div>
          <div className='flex flex-col gap-1'>
            <p className='font-bold text-gray-700'>Status</p>
            <ElectionRoundStatusBadge status={electionRound.status} />
          </div>
        </CardContent>
      </Card>
    </Layout>
  );
}

export default ElectionRoundDetails;
