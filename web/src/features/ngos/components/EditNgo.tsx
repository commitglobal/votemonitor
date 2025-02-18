import Layout from '@/components/layout/Layout';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Form, FormControl, FormField, FormItem, FormLabel, FormMessage } from '@/components/ui/form';
import { Input } from '@/components/ui/input';
import { Select, SelectContent, SelectGroup, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select';
import { Separator } from '@/components/ui/separator';
import { TrashIcon } from '@heroicons/react/24/outline';
import { zodResolver } from '@hookform/resolvers/zod';
import { useNavigate } from '@tanstack/react-router';
import { FC } from 'react';
import { useForm } from 'react-hook-form';
import { useNgoMutations } from '../hooks/ngos-queries';
import { EditNgoFormData, editNgoSchema, NGO, NGOStatus } from '../models/NGO';
import { NgoBackButton, NgoBreadcrumbs } from './NgoExtraComponents';

interface EditNgoAdminProps {
  existingData: NGO;
  ngoId: string;
}

export const EditNgo: FC<EditNgoAdminProps> = ({ existingData, ngoId }) => {
  const navigate = useNavigate();

  const { editNgoMutation, deleteNgoWithConfirmation } = useNgoMutations();

  const form = useForm<EditNgoFormData>({
    resolver: zodResolver(editNgoSchema),
    defaultValues: {
      name: existingData.name,
      status: existingData.status,
    },
  });

  function onSubmit(values: EditNgoFormData) {
    editNgoMutation.mutate({ ngoId, values });
  }

  const handleDelete = async () => {
    await deleteNgoWithConfirmation({
      ngoId,
      name: existingData.name,
      onMutationSuccess: () => {
        navigate({ to: '/ngos/view/$ngoId/$tab', params: { ngoId, tab: 'admins' } });
      },
    });
  };

  return (
    <Layout
      title={`Edit ${existingData.name}`}
      backButton={<NgoBackButton ngoId={ngoId} />}
      breadcrumbs={<NgoBreadcrumbs ngoData={{ id: ngoId, name: existingData.name }} />}>
      <Card className='w-[800px] pt-0'>
        <CardHeader className='flex flex-column gap-2'>
          <div className='flex flex-row justify-between items-center'>
            <CardTitle className='text-xl'>Edit NGO</CardTitle>
          </div>
          <Separator />
        </CardHeader>
        <CardContent className='flex flex-col gap-6 items-baseline'>
          <Form {...form}>
            <form onSubmit={form.handleSubmit(onSubmit)} className='space-y-4'>
              <FormField
                control={form.control}
                name='name'
                render={({ field }) => (
                  <FormItem className='w-[540px]'>
                    <FormLabel>
                      Name <span className='text-red-500'>*</span>
                    </FormLabel>
                    <FormControl>
                      <Input placeholder='Name' {...field} />
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
                          <SelectValue placeholder='NGO Status' />
                        </SelectTrigger>
                        <SelectContent>
                          <SelectGroup>
                            <SelectItem value={NGOStatus.Activated}>{NGOStatus.Activated}</SelectItem>
                            <SelectItem value={NGOStatus.Pending}>{NGOStatus.Pending}</SelectItem>
                            <SelectItem value={NGOStatus.Deactivated}>{NGOStatus.Deactivated}</SelectItem>
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
                  Delete organization
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
