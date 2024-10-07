import { FunctionComponent } from '@/common/types';
import { BackButtonIcon } from '@/components/layout/Breadcrumbs/BackButton';
import Layout from '@/components/layout/Layout';
import { Card, CardContent, CardHeader } from '@/components/ui/card';
import { useCurrentElectionRoundStore } from '@/context/election-round.store';
import { useSuspenseQuery } from '@tanstack/react-query';
import { Link, useNavigate } from '@tanstack/react-router';
import { citizenGuideDetailsQueryOptions } from '../../hooks/citizen-guides-hooks';
import { observerGuideDetailsQueryOptions } from '../../hooks/observer-guides-hooks';
import { GuidePageType } from '../../models/guide';

export interface ViewTextGuideProps {
  guidePageType: GuidePageType;
  guideId: string;
}

export default function ViewTextGuide({ guidePageType, guideId }: ViewTextGuideProps): FunctionComponent {
  const currentElectionRoundId = useCurrentElectionRoundStore((s) => s.currentElectionRoundId);

  const { data: guide } =
    guidePageType === GuidePageType.Observer
      ? useSuspenseQuery(observerGuideDetailsQueryOptions(currentElectionRoundId, guideId))
      : useSuspenseQuery(citizenGuideDetailsQueryOptions(currentElectionRoundId, guideId));

  return (
    <Layout
      title={`${guide.title}`}
      breadcrumbs={<></>}
      backButton={
        <Link
          title='Go back'
          to='/election-event/$tab'
          params={{ tab: guidePageType === GuidePageType.Observer ? 'observer-guides' : 'citizen-guides' }}>
          <BackButtonIcon />
        </Link>
      }>
      <Card className='w-full pt-0'>
        <CardHeader className='flex gap-2 flex-column'></CardHeader>
        <CardContent className='flex flex-col items-baseline gap-6'>
          <div className='flex flex-col gap-1'>
            <p className='font-bold text-gray-700'>Text</p>
            <p className='font-normal text-gray-900'>{guide.text}</p>
          </div>
        </CardContent>
      </Card>
    </Layout>
  );
}
