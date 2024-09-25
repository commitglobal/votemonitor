import { FunctionComponent } from '@/common/types';
import { BackButtonIcon } from '@/components/layout/Breadcrumbs/BackButton';
import Layout from '@/components/layout/Layout';
import { useConfirm } from '@/components/ui/alert-dialog-provider';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Separator } from '@/components/ui/separator';
import { useToast } from '@/components/ui/use-toast';
import { useCurrentElectionRoundStore } from '@/context/election-round.store';
import { useQueryClient, useSuspenseQuery } from '@tanstack/react-query';
import { Link, useNavigate } from '@tanstack/react-router';
import { citizenGuideDetailsQueryOptions } from '../../hooks/citizen-guides-hooks';
import { observerGuideDetailsQueryOptions } from '../../hooks/observer-guides-hooks';
import { GuidePageType, GuideType } from '../../models/guide';
import EditGuideForm from './EditGuideForm';

export interface EditTextGuideProps {
  guidePageType: GuidePageType;
  guideId: string;
}

export default function EditTextGuide({ guidePageType, guideId }: EditTextGuideProps): FunctionComponent {
  const navigate = useNavigate();
  const currentElectionRoundId = useCurrentElectionRoundStore((s) => s.currentElectionRoundId);

  const { data: guide } =
    guidePageType === GuidePageType.Observer
      ? useSuspenseQuery(observerGuideDetailsQueryOptions(currentElectionRoundId, guideId))
      : useSuspenseQuery(citizenGuideDetailsQueryOptions(currentElectionRoundId, guideId));

  return (
    <Layout
      title={`Edit ${guide.title}`}
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
        <CardHeader className='flex gap-2 flex-column'>
          <div className='flex flex-row items-center justify-between'>
            <CardTitle className='text-xl'>Edit text guide</CardTitle>
          </div>
          <Separator />
        </CardHeader>
        <CardContent className='flex flex-col items-baseline gap-6'>
          <EditGuideForm guideId={guideId} guidePageType={guidePageType} guideType={GuideType.Text}>
            <div className='flex justify-between'>
              <div></div>
              <div className='flex gap-2'>
                <Button
                  variant='outline'
                  type='button'
                  onClick={() => {
                    void navigate({
                      to: '/election-event/$tab',
                      params: { tab: guidePageType === GuidePageType.Observer ? 'observer-guides' : 'citizen-guides' },
                    });
                  }}>
                  Cancel
                </Button>
                <Button type='submit' className='px-6'>
                  Save
                </Button>
              </div>
            </div>
          </EditGuideForm>
        </CardContent>
      </Card>
    </Layout>
  );
}
