import { authApi } from '@/common/auth-api';
import Layout from '@/components/layout/Layout';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Form, FormControl, FormField, FormItem, FormLabel, FormMessage } from '@/components/ui/form';
import { Input } from '@/components/ui/input';
import { Separator } from '@/components/ui/separator';
import TagsSelectFormField from '@/components/ui/tag-selector';
import { toast } from '@/components/ui/use-toast';
import { useCurrentElectionRoundStore } from '@/context/election-round.store';
import { useMonitoringObserversTags } from '@/hooks/tags-queries';
import { zodResolver } from '@hookform/resolvers/zod';
import { useMutation, useQueryClient } from '@tanstack/react-query';
import { useNavigate } from '@tanstack/react-router';
import { useForm } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import { z } from 'zod';
import { MonitorObserverBackButton } from './MonitoringObserverBackButton';
import { monitoringObserversKeys } from '../hooks/monitoring-observers-queries';

export default function CreateMonitoringObserver() {
  const { t } = useTranslation('translation', { keyPrefix: 'observers.addObserver' });
  const currentElectionRoundId = useCurrentElectionRoundStore((s) => s.currentElectionRoundId);
  const { data: availableTags } = useMonitoringObserversTags(currentElectionRoundId);
  const navigate = useNavigate();
  const queryClient = useQueryClient();
  const newObserverSchema = z.object({
    firstName: z.string(),
    lastName: z.string(),
    email: z.string().email(),
    phoneNumber: z.string(),
    tags: z.any(),
  });

  type ObserverFormData = z.infer<typeof newObserverSchema>;

  const form = useForm<ObserverFormData>({
    resolver: zodResolver(newObserverSchema),
  });

  function onSubmit(values: ObserverFormData) {
    newObserverMutation.mutate({ electionRoundId: currentElectionRoundId, values });
  }

  const newObserverMutation = useMutation({
    mutationFn: ({ electionRoundId, values }: { electionRoundId: string; values: ObserverFormData }) => {
      return authApi.post(`/election-rounds/${electionRoundId}/monitoring-observers`, { observers: [values] });
    },

    onSuccess: (_, {electionRoundId}) => {
      toast({
        title: 'Success',
        description: t('onSuccess'),
      });

      queryClient.invalidateQueries({ queryKey: monitoringObserversKeys.all(electionRoundId) });

      navigate({
        to: '/monitoring-observers/$tab',
        params: { tab: 'list' },
      });
    },
    onError: () => {
      toast({
        title: t('onError'),
        description: 'Please contact tech support',
        variant: 'destructive',
      });
    },
  });
  return (
    <Layout title={''} backButton={<MonitorObserverBackButton />} enableBreadcrumbs={false}>
      <Card className='w-[800px] pt-0'>
        <CardHeader className='flex gap-2 flex-column'>
          <div className='flex flex-row items-center justify-between'>
            <CardTitle className='text-xl'>{t('title')}</CardTitle>
          </div>
          <Separator />
        </CardHeader>
        <CardContent>
          <Form {...form}>
            <form onSubmit={form.handleSubmit(onSubmit)} className='space-y-4'>
              <FormField
                control={form.control}
                name='firstName'
                render={({ field, fieldState }) => (
                  <FormItem>
                    <FormLabel>{t('firstName')}</FormLabel>
                    <Input placeholder={t('firstName')} {...field} {...fieldState} />
                    <FormMessage />
                  </FormItem>
                )}
              />

              <FormField
                control={form.control}
                name='lastName'
                render={({ field, fieldState }) => (
                  <FormItem>
                    <FormLabel>{t('lastName')}</FormLabel>
                    <Input placeholder={t('lastName')} {...field} {...fieldState} />
                    <FormMessage />
                  </FormItem>
                )}
              />

              <FormField
                control={form.control}
                name='email'
                render={({ field, fieldState }) => (
                  <FormItem>
                    <FormLabel>{t('email')}</FormLabel>
                    <Input placeholder={t('email')} {...field} {...fieldState} />
                    <FormMessage />
                  </FormItem>
                )}
              />

              <FormField
                control={form.control}
                name='phoneNumber'
                render={({ field, fieldState }) => (
                  <FormItem>
                    <FormLabel>{t('phone')}</FormLabel>
                    <Input placeholder={t('phone')} {...field} {...fieldState} />
                    <FormMessage />
                  </FormItem>
                )}
              />

              <FormField
                control={form.control}
                name='tags'
                render={({ field }) => (
                  <FormItem>
                    <FormLabel className='text-left'>Tags</FormLabel>
                    <FormControl>
                      <TagsSelectFormField
                        options={availableTags?.filter((tag) => !field.value?.includes(tag)) ?? []}
                        defaultValue={field.value}
                        onValueChange={field.onChange}
                        placeholder='Observer tags'
                      />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />

              <div className='flex justify-between'>
                <div className='flex gap-2'>
                  <Button
                    variant='outline'
                    type='button'
                    onClick={() => {
                      void navigate({
                        to: '/monitoring-observers/$tab',
                        params: { tab: 'list' },
                      });
                    }}>
                    Cancel
                  </Button>

                  <Button title={t('addBtnText')} type='submit' className='px-6'>
                    {t('addBtnText')}
                  </Button>
                </div>
              </div>
            </form>
          </Form>
        </CardContent>
      </Card>
    </Layout>
  );
}
