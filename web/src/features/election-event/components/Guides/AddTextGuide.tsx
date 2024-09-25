import { FunctionComponent } from '@/common/types';
import { BackButtonIcon } from '@/components/layout/Breadcrumbs/BackButton';
import Layout from '@/components/layout/Layout';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Separator } from '@/components/ui/separator';
import { Link, useNavigate } from '@tanstack/react-router';
import { GuidePageType, GuideType } from '../../models/guide';
import AddGuideForm from './AddGuideForm';

export interface AddTextGuideProps {
  guidePageType: GuidePageType;
}

export default function AddTextGuide({ guidePageType }: AddTextGuideProps): FunctionComponent {
  const navigate = useNavigate();

  return (
    <Layout
      title={`New text guide`}
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
          <AddGuideForm
            guidePageType={guidePageType}
            onSuccess={(guide) =>
              void navigate({
                to:
                  guidePageType === GuidePageType.Observer
                    ? '/observer-guides/edit/$guideId'
                    : '/citizen-guides/edit/$guideId',
                params: { guideId: guide.id },
              })
            }
            guideType={GuideType.Text}>
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
          </AddGuideForm>
        </CardContent>
      </Card>
    </Layout>
  );
}
