import { useLoaderData, useNavigate } from '@tanstack/react-router';
import { Observer } from '../../models/Observer';
import Layout from '@/components/layout/Layout';
import { Card, CardContent, CardDescription, CardFooter, CardHeader, CardTitle } from '@/components/ui/card';
import { Tabs, TabsContent, TabsList, TabsTrigger } from '@/components/ui/tabs';
import { Badge } from '@/components/ui/badge';
import { Separator } from '@/components/ui/separator';
import { Button } from '@/components/ui/button';
import { PencilIcon } from '@heroicons/react/24/outline';

export default function ObserverDetails() {
  const observer: Observer = useLoaderData({ strict: false });
  const navigate = useNavigate();
  const navigateToEdit = () => {
    navigate({ to: `/observers/$observerId/edit`, params: { observerId: observer.id } });
  };

  return (
    <Layout title={observer.name}>
      <Tabs defaultValue='observer-details'>
        <TabsList className='grid grid-cols-2 bg-gray-200 w-[400px] mb-4'>
          <TabsTrigger value='observer-details'>Observer details</TabsTrigger>
          <TabsTrigger value='responses'>Responses/forms</TabsTrigger>
        </TabsList>
        <TabsContent value='observer-details'>
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
                <p className='text-gray-700 font-bold'>Name</p>
                <p className='text-gray-900 font-normal'>{observer.name}</p>
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
                <p className='text-gray-700 font-bold'>Last activity</p>
                <p className='text-gray-900 font-normal'>Oct 16, 2023, 4:32:53 AM</p>
              </div>
              <div className='flex flex-col gap-1'>
                <p className='text-gray-700 font-bold'>Status</p>
                <Badge className={'badge-' + observer.status}>{observer.status}</Badge>
              </div>
            </CardContent>
            <CardFooter className='flex justify-between'></CardFooter>
          </Card>
        </TabsContent>
        <TabsContent value='responses'>Change your password here.</TabsContent>
      </Tabs>
    </Layout>
  );
}
