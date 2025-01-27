import Layout from '@/components/layout/Layout';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Form, FormControl, FormField, FormItem, FormLabel, FormMessage } from '@/components/ui/form';
import { Input } from '@/components/ui/input';
import { Select, SelectContent, SelectGroup, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select';
import { Separator } from '@/components/ui/separator';
import { Route } from '@/routes/ngos/admin.$ngoId.$adminId.edit';
import { TrashIcon } from '@heroicons/react/24/outline';
import { zodResolver } from '@hookform/resolvers/zod';
import { useNavigate } from '@tanstack/react-router';
import { FC } from 'react';
import { useForm } from 'react-hook-form';
import { z } from 'zod';
import { useNgoAdminDeleteWithConfirmation } from '../../hooks/ngos-queriess';
import { NgoAdminStatus } from '../../models/NGO';
import { editNgoAdminSchema, NgoAdmin } from '../../models/NgoAdmin';
import { NgoBackButton, NgoBreadcrumbs } from '../NgoExtraComponents';

interface EditNgoAdminProps {
  existingData: NgoAdmin;
  id: string;
}

export const EditNgoAdmin: FC<EditNgoAdminProps> = ({ id, existingData }) => {
  const navigate = useNavigate();
  const { ngoId, adminId } = Route.useParams();
  const { deleteNgoAdminWithConfirmation } = useNgoAdminDeleteWithConfirmation(ngoId);
  const displayName = `${existingData.firstName} ${existingData.lastName}`;

  const form = useForm<z.infer<typeof editNgoAdminSchema>>({
    resolver: zodResolver(editNgoAdminSchema),
    defaultValues: {
      firstName: existingData.firstName,
      lastName: existingData.lastName,
      email: existingData.email,
      phoneNumber: existingData.phoneNumber,
      status: existingData.status,
    },
  });

  function onSubmit(values: z.infer<typeof editNgoAdminSchema>) {
    // editMutation.mutate({
    //   observerId: observer.id,
    //   obj: values,
    // });
  }

  // const deleteMutation = useMutation({
  //   mutationFn: (observerId: string) => {
  //     return authApi.delete<void>(`/observers/${observerId}`);
  //   },
  //   onSuccess: () => {
  //     navigate({ to: '/observers' });
  //   },
  // });

  // const editMutation = useMutation({
  //   mutationFn: ({ observerId, obj }: any) => {
  //     return authApi.put<void>(`/observers/${observerId}`, obj);
  //   },
  // });

  const handleDelete = async () => {
    await deleteNgoAdminWithConfirmation({
      ngoId,
      adminId,
      name: displayName,
      onMutationSuccess: () => {
        navigate({ to: '/ngos/view/$ngoId/$tab', params: { ngoId, tab: 'admins' } });
      },
    });
  };

  return (
    <Layout
      title={`Edit ${displayName}`}
      backButton={<NgoBackButton ngoId={ngoId} />}
      breadcrumbs={
        <NgoBreadcrumbs ngoData={{ id: ngoId, name: '' }} adminData={{ id: existingData.id, name: displayName }} />
      }>
      <Card className='w-[800px] pt-0'>
        <CardHeader className='flex flex-column gap-2'>
          <div className='flex flex-row justify-between items-center'>
            <CardTitle className='text-xl'>Edit NGO admin</CardTitle>
          </div>
          <Separator />
        </CardHeader>
        <CardContent className='flex flex-col gap-6 items-baseline'>
          <Form {...form}>
            <form onSubmit={form.handleSubmit(onSubmit)} className='space-y-4'>
              <FormField
                control={form.control}
                name='firstName'
                render={({ field }) => (
                  <FormItem className='w-[540px]'>
                    <FormLabel>
                      First name <span className='text-red-500'>*</span>
                    </FormLabel>
                    <FormControl>
                      <Input placeholder='First name' {...field} />
                    </FormControl>
                    <FormMessage className='mt-2' />
                  </FormItem>
                )}
              />

              <FormField
                control={form.control}
                name='lastName'
                render={({ field }) => (
                  <FormItem className='w-[540px]'>
                    <FormLabel>
                      Last name <span className='text-red-500'>*</span>
                    </FormLabel>
                    <FormControl>
                      <Input placeholder='Last name' {...field} />
                    </FormControl>
                    <FormMessage className='mt-2' />
                  </FormItem>
                )}
              />
              <FormField
                control={form.control}
                name='email'
                render={({ field }) => (
                  <FormItem className='w-[540px]'>
                    <FormLabel>
                      Email <span className='text-red-500'>*</span>
                    </FormLabel>
                    <FormControl>
                      <Input placeholder='Email address' {...field} />
                    </FormControl>
                    <FormMessage className='mt-2' />
                  </FormItem>
                )}
              />
              <FormField
                control={form.control}
                name='phoneNumber'
                render={({ field }) => (
                  <FormItem className='w-[540px]'>
                    <FormLabel>
                      Phone number <span className='text-red-500'>*</span>
                    </FormLabel>
                    <FormControl>
                      <Input placeholder='Phone number' {...field} />
                    </FormControl>
                    <FormMessage className='mt-2' />
                  </FormItem>
                )}
              />
              <FormField
                control={form.control}
                name='status'
                render={({ field }) => (
                  <FormItem className='w-[540px]'>
                    <FormLabel>
                      Status <span className='text-red-500'>*</span>
                    </FormLabel>
                    <FormControl>
                      <Select onValueChange={field.onChange} defaultValue={field.value} value={field.value}>
                        <SelectTrigger>
                          <SelectValue placeholder='Admin status' />
                        </SelectTrigger>
                        <SelectContent>
                          <SelectGroup>
                            <SelectItem value={NgoAdminStatus.Active}>{NgoAdminStatus.Active}</SelectItem>
                            <SelectItem value={NgoAdminStatus.Deactivated}>{NgoAdminStatus.Deactivated}</SelectItem>
                          </SelectGroup>
                        </SelectContent>
                      </Select>
                    </FormControl>
                    <FormMessage className='mt-2' />
                  </FormItem>
                )}
              />
              <div className='flex justify-between'>
                <Button
                  onClick={handleDelete}
                  variant='ghost'
                  className='text-destructive hover:text-destructive hover:bg-background px-0'>
                  <TrashIcon className='w-[18px] mr-2' />
                  Delete admin
                </Button>
                <div className='flex gap-2'>
                  <Button variant='outline' type='submit'>
                    Cancel
                  </Button>
                  <Button type='submit' className='px-6'>
                    Save
                  </Button>
                </div>
              </div>
            </form>
          </Form>
        </CardContent>
      </Card>
    </Layout>
  );
};
