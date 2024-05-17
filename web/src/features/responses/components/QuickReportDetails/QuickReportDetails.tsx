import { Link, useLoaderData } from '@tanstack/react-router';
import type { FunctionComponent } from '@/common/types';
import Layout from '@/components/layout/Layout';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Switch } from '@/components/ui/switch';
import { Separator } from '@/components/ui/separator';
import { format } from 'date-fns';
import { ArrowTopRightOnSquareIcon } from '@heroicons/react/24/outline';
import { Dialog, DialogContent, DialogTrigger } from '@/components/ui/dialog';

export default function QuickReportDetails(): FunctionComponent {
  const quickReport = useLoaderData({ from: '/responses/quick-reports/$quickReportId' });

  return (
    <Layout
      backButton={
        <Link to='/responses' preload='intent' search={{ tab: 'quick-reports' }}>
          <svg xmlns='http://www.w3.org/2000/svg' width='30' height='30' viewBox='0 0 30 30' fill='none'>
            <path
              fillRule='evenodd'
              clipRule='evenodd'
              d='M19.0607 7.93934C19.6464 8.52513 19.6464 9.47487 19.0607 10.0607L14.1213 15L19.0607 19.9393C19.6464 20.5251 19.6464 21.4749 19.0607 22.0607C18.4749 22.6464 17.5251 22.6464 16.9393 22.0607L10.9393 16.0607C10.3536 15.4749 10.3536 14.5251 10.9393 13.9393L16.9393 7.93934C17.5251 7.35355 18.4749 7.35355 19.0607 7.93934Z'
              fill='#7833B3'
            />
          </svg>
        </Link>
      }
      breadcrumbs={
        <div className='breadcrumbs flex flex-row gap-2 mb-4'>
          <Link className='crumb' to='/responses' preload='intent' search={{ tab: 'quick-reports' }}>
            responses
          </Link>
          <Link className='crumb'>{quickReport.id}</Link>
        </div>
      }
      title={quickReport.id}>
      <Card className='max-w-4xl'>
        <CardHeader>
          <CardTitle className='mb-4 flex justify-between'>
            <div>{quickReport.title}</div>
            <Switch id='needs-followup'>Needs follow-up</Switch>
          </CardTitle>
          <Separator />
        </CardHeader>

        <CardContent className='text-[#374151] flex flex-col gap-6'>
          <div>
            <p className='font-bold'>Time submitted</p>
            {quickReport.timestamp && <p>{format(quickReport.timestamp, 'u-MM-dd KK:mm')}</p>}
          </div>

          <div>
            <p className='font-bold'>Issue title</p>
            <p>{quickReport.title}</p>
          </div>

          <div>
            <p className='font-bold'>Description</p>
            <p>{quickReport.description}</p>
          </div>

          {quickReport.attachments.length > 0 && (
            <div>
              <p className='font-bold pb-2'>Media files</p>
              <div className='flex flex-wrap gap-4'>
                {quickReport.attachments.map((attachment) => (
                  <Dialog>
                    <DialogTrigger>
                      <button type='button'>
                        <img alt={attachment.fileName} className='rounded-lg h-44 w-44' src={attachment.presignedUrl} />
                      </button>
                    </DialogTrigger>
                    <DialogContent className='max-w-5xl'>
                      <div className='flex justify-center'>
                        <img alt={attachment.fileName} src={attachment.presignedUrl} />
                      </div>
                    </DialogContent>
                  </Dialog>
                ))}
              </div>
            </div>
          )}

          <div>
            <p className='font-bold'>Observer</p>
            <Link
              search
              className='text-purple-500 flex gap-1'
              to='/monitoring-observers/$monitoringObserverId/view/$tab'
              params={{ monitoringObserverId: quickReport.monitoringObserverId, tab: 'details' }}
              target='_blank'
              preload={false}>
              {quickReport.firstName} {quickReport.lastName}
              <ArrowTopRightOnSquareIcon className='w-4' />
            </Link>
          </div>

          <div>
            <p className='font-bold'>Location type</p>
            <p>{quickReport.quickReportLocationType}</p>
          </div>

          {quickReport.pollingStationDetails && (
            <div>
              <p className='font-bold'>Polling station details</p>
              <p>{quickReport.pollingStationDetails}</p>
            </div>)}

          {quickReport.level1 && (
            <div className='flex gap-4'>
              <div>
                <p className='font-bold'>Location - L1</p>
                {quickReport.level1}
              </div>
              {quickReport.level2 && (
                <div>
                  <p className='font-bold'>Location - L2</p>
                  {quickReport.level2}
                </div>
              )}
              {quickReport.level3 && (
                <div>
                  <p className='font-bold'>Location - L3</p>
                  {quickReport.level3}
                </div>
              )}
              {quickReport.level4 && (
                <div>
                  <p className='font-bold'>Location - L4</p>
                  {quickReport.level4}
                </div>
              )}
              {quickReport.level5 && (
                <div>
                  <p className='font-bold'>Location - L5</p>
                  {quickReport.level5}
                </div>
              )}
              <div>
                <p className='font-bold'>Station</p>
                <p>{quickReport.number}</p>
              </div>
            </div>
          )}
        </CardContent>
      </Card>
    </Layout>
  );
}
