import { BackButtonIcon } from '@/components/layout/Breadcrumbs/BackButton';
import Layout from '@/components/layout/Layout';
import { useConfirm } from '@/components/ui/alert-dialog-provider';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Form, FormControl, FormField, FormItem, FormLabel, FormMessage } from '@/components/ui/form';
import { Input } from '@/components/ui/input';
import { Separator } from '@/components/ui/separator';
import { Route } from '@/routes/ngos/admin/$ngoId.$adminId.edit';
import { TrashIcon } from '@heroicons/react/24/outline';
import { zodResolver } from '@hookform/resolvers/zod';
import { Link, useBlocker, useNavigate } from '@tanstack/react-router';
import { FC, useEffect, useMemo } from 'react';
import { useForm } from 'react-hook-form';
import { useNgoAdminMutations } from '../../hooks/ngo-admin-queries';
import { EditNgoAdminFormData, editNgoAdminSchema, NgoAdmin } from '../../models/NgoAdmin';
import { NgoAdminStatusBadge } from '../NgoStatusBadges';

interface EditNgoAdminProps {
  ngoAdmin: NgoAdmin;
}

export const EditNgoAdmin: FC<EditNgoAdminProps> = ({ ngoAdmin }) => {
  const navigate = useNavigate();
  const { ngoId, adminId } = Route.useParams();
  const { editNgoAdminMutation, deleteNgoAdminWithConfirmation } = useNgoAdminMutations(ngoId);
  const confirm = useConfirm();

  const displayName = useMemo(
    () => `${ngoAdmin.firstName} ${ngoAdmin.lastName}`,
    [ngoAdmin.firstName, ngoAdmin.lastName]
  );

  const form = useForm<EditNgoAdminFormData>({
    resolver: zodResolver(editNgoAdminSchema),
    mode: 'all',
    reValidateMode: 'onChange',
    defaultValues: {
      firstName: ngoAdmin.firstName,
      lastName: ngoAdmin.lastName,
      phoneNumber: ngoAdmin.phoneNumber,
    },
  });

  useEffect(() => {
    if (form.formState.isSubmitSuccessful) {
      form.reset({}, { keepValues: true });
    }
  }, [form.formState.isSubmitSuccessful, form.reset]);

  useBlocker({
    shouldBlockFn: async () => {
      if (!form.formState.isDirty) {
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

  function onSubmit(values: EditNgoAdminFormData) {
    editNgoAdminMutation.mutate({ adminId, values });
  }

  const handleDelete = async () => {
    await deleteNgoAdminWithConfirmation({
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
      backButton={
        <Link title='Go back' to='/ngos/admin/$ngoId/$adminId/view' params={{ ngoId, adminId }} preload='intent'>
          <BackButtonIcon />
        </Link>
      }
      breadcrumbs={<></>}>
      <Card className='w-[600px] pt-0'>
        <CardHeader className='flex flex-column gap-2'>
          <div className='flex flex-row justify-between items-center'>
            <CardTitle className='text-xl'>Edit NGO admin</CardTitle>
          </div>
          <Separator />
        </CardHeader>
        <CardContent className='flex flex-col gap-6 items-baseline'>
          <Form {...form}>
            <form onSubmit={form.handleSubmit(onSubmit)} className='space-y-4'>
              <div className='flex flex-col gap-1'>
                <p className='font-bold text-gray-700'>Email</p>
                <p className='font-normal text-gray-900'>{ngoAdmin.email}</p>
              </div>
              <FormField
                control={form.control}
                name='firstName'
                render={({ field, fieldState }) => (
                  <FormItem className='w-[540px]'>
                    <FormLabel>
                      First name <span className='text-red-500'>*</span>
                    </FormLabel>
                    <FormControl>
                      <Input placeholder='First name' {...field} {...fieldState} />
                    </FormControl>
                    <FormMessage className='mt-2' />
                  </FormItem>
                )}
              />

              <FormField
                control={form.control}
                name='lastName'
                render={({ field, fieldState }) => (
                  <FormItem className='w-[540px]'>
                    <FormLabel>
                      Last name <span className='text-red-500'>*</span>
                    </FormLabel>
                    <FormControl>
                      <Input placeholder='Last name' {...field} {...fieldState} />
                    </FormControl>
                    <FormMessage className='mt-2' />
                  </FormItem>
                )}
              />

              <FormField
                control={form.control}
                name='phoneNumber'
                render={({ field, fieldState }) => (
                  <FormItem className='w-[540px]'>
                    <FormLabel>Phone number</FormLabel>
                    <FormControl>
                      <Input placeholder='Phone number' {...field} {...fieldState} />
                    </FormControl>
                    <FormMessage className='mt-2' />
                  </FormItem>
                )}
              />

              <div className='flex flex-col gap-1'>
                <p className='font-bold text-gray-700'>Status</p>
                <NgoAdminStatusBadge status={ngoAdmin.status} />
              </div>

              <div className='flex justify-between'>
                <Button
                  onClick={handleDelete}
                  variant='ghost'
                  className='text-destructive hover:text-destructive hover:bg-background px-0'>
                  <TrashIcon className='w-[18px] mr-2' />
                  Delete admin
                </Button>
                <div className='flex gap-2'>
                  <Link
                    title='Go back'
                    to='/ngos/admin/$ngoId/$adminId/view'
                    params={{ ngoId, adminId }}
                    preload='intent'>
                    <Button variant='outline' type='button'>
                      Cancel
                    </Button>
                  </Link>

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
