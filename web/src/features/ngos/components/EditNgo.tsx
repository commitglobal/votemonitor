import { BackButtonIcon } from '@/components/layout/Breadcrumbs/BackButton';
import Layout from '@/components/layout/Layout';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Form, FormControl, FormField, FormItem, FormLabel, FormMessage } from '@/components/ui/form';
import { Input } from '@/components/ui/input';
import { Separator } from '@/components/ui/separator';
import { TrashIcon } from '@heroicons/react/24/outline';
import { zodResolver } from '@hookform/resolvers/zod';
import { Link, useBlocker, useNavigate } from '@tanstack/react-router';
import { FC, useEffect } from 'react';
import { useForm } from 'react-hook-form';
import { useNgoMutations } from '../hooks/ngos-queries';
import { EditNgoFormData, editNgoSchema, NGO } from '../models/NGO';
import { useConfirm } from '@/components/ui/alert-dialog-provider';
import { DevTool } from '@hookform/devtools';

interface EditNgoAdminProps {
  existingData: NGO;
  ngoId: string;
}

export const EditNgo: FC<EditNgoAdminProps> = ({ existingData, ngoId }) => {
  const navigate = useNavigate();
  const confirm = useConfirm();

  const { editNgoMutation, deleteNgoWithConfirmation } = useNgoMutations();

  const form = useForm<EditNgoFormData>({
    resolver: zodResolver(editNgoSchema),
    mode: 'all',
    reValidateMode: 'onChange',
    defaultValues: {
      name: existingData.name ?? '',
    },
  });

  useEffect(() => {
    if (form.formState.isSubmitSuccessful) {
      form.reset({}, { keepValues: true });
    }
  }, [form.formState.isSubmitSuccessful, form.reset]);

  function onSubmit(values: EditNgoFormData) {
    editNgoMutation.mutate({ ngoId, values });
  }

  const handleDelete = async () => {
    await deleteNgoWithConfirmation({
      ngoId,
      name: existingData.name,
      onMutationSuccess: () => {
        navigate({ to: '/ngos' });
      },
    });
  };

  useBlocker({
    shouldBlockFn: async () => {
      if (!form.formState.isDirty || form.formState.isSubmitting) {
        return false;
      }

      return !(await confirm({
        title: `Unsaved Changes Detected`,
        body: 'You have unsaved changes. If you leave this page, your changes will be lost. Are you sure you want to continue?',
        actionButton: 'Leave',
        cancelButton: 'Stay',
      }));
    },
  });

  return (
    <Layout
      title={`Edit ${existingData.name}`}
      backButton={
        <Link title='Go back' to='/ngos/view/$ngoId/$tab' params={{ ngoId, tab: 'details' }} preload='intent'>
          <BackButtonIcon />
        </Link>
      }
      breadcrumbs={<></>}>
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
                render={({ field, fieldState }) => (
                  <FormItem className='w-[540px]'>
                    <FormLabel>
                      Name <span className='text-red-500'>*</span>
                    </FormLabel>
                    <FormControl>
                      <Input placeholder='Name' {...field} {...fieldState} />
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
            <DevTool control={form.control} /> {/* set up the dev tool */}
          </Form>
        </CardContent>
      </Card>
    </Layout>
  );
};
