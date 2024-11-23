import Layout from '@/components/layout/Layout';
import { Card, CardContent } from '@/components/ui/card';
import { Form, FormControl, FormDescription, FormField, FormItem, FormLabel, FormMessage } from '@/components/ui/form';
import { Input } from '@/components/ui/input';
import { zodResolver } from '@hookform/resolvers/zod';
import { useForm } from 'react-hook-form';
import { z } from 'zod';

import { authApi } from '@/common/auth-api';
import { ElectionRoundStatus, type FunctionComponent } from '@/common/types';
import { RichTextEditor } from '@/components/rich-text-editor';
import { toast } from '@/components/ui/use-toast';
import { useCurrentElectionRoundStore } from '@/context/election-round.store';
import { useMutation, useQueryClient } from '@tanstack/react-query';
import { useNavigate, useRouter } from '@tanstack/react-router';
import { citizenNotificationsKeys } from '../hooks/citizen-notifications-queries';
import { Button } from '@/components/ui/button';
import { useElectionRoundDetails } from '@/features/election-event/hooks/election-event-hooks';

const createPushMessageSchema = z.object({
  title: z.string().min(1, { message: 'Your message must have a title before sending.' }),
  messageBody: z
    .string()
    .min(1, { message: 'Your message must have a detailed description before sending.' })
    .max(1000),
});

function CitizenNotificationMessageForm(): FunctionComponent {
  const navigate = useNavigate();
  const currentElectionRoundId = useCurrentElectionRoundStore((s) => s.currentElectionRoundId);
  const { data: electionRound } = useElectionRoundDetails(currentElectionRoundId);

  const router = useRouter();

  const queryClient = useQueryClient();

  const form = useForm<z.infer<typeof createPushMessageSchema>>({
    resolver: zodResolver(createPushMessageSchema),
    defaultValues: {
      title: '',
      messageBody: '',
    },
  });

  const sendNotificationMutation = useMutation({
    mutationFn: ({ electionRoundId, title, body }: { electionRoundId: string; title: string; body: string }) => {
      return authApi.post<{
        title: string;
        body: string;
      }>(`/election-rounds/${electionRoundId}/citizen-notifications:send`, { title, body });
    },

    onSuccess: async () => {
      queryClient.invalidateQueries({ queryKey: citizenNotificationsKeys.all(currentElectionRoundId) });
      toast({
        title: 'Success',
        description: 'Notification sent',
      });

      router.invalidate();
      await navigate({ to: '/election-event/$tab', params: { tab: 'citizen-notifications' } });
    },
  });

  function onSubmit(values: z.infer<typeof createPushMessageSchema>): void {
    sendNotificationMutation.mutate({
      electionRoundId: currentElectionRoundId,
      title: values.title,
      body: values.messageBody,
    });
  }

  return (
    <Layout title='Create new message'>
      <Card className='py-6'>
        <CardContent>
          <Form {...form}>
            {/* eslint-disable-next-line @typescript-eslint/no-misused-promises */}
            <form onSubmit={form.handleSubmit(onSubmit)}>
              <h2 className='mb-10 text-xl font-medium'>1. Compose your message</h2>

              <div className='flex flex-col gap-6 mb-20'>
                <FormField
                  control={form.control}
                  name='title'
                  render={({ field }) => (
                    <FormItem className='w-[540px]'>
                      <FormLabel className='text-left'>
                        Title of message <span className='text-red-500'>*</span>
                      </FormLabel>
                      <FormControl>
                        <Input {...field} maxLength={256} />
                      </FormControl>
                      <FormDescription>
                        Create a short title for your message that will appear in the push notification.
                      </FormDescription>
                      <FormMessage />
                    </FormItem>
                  )}
                />

                <FormField
                  control={form.control}
                  name='messageBody'
                  render={({ field }) => (
                    <FormItem className='w-full'>
                      <FormLabel className='text-left'>
                        Message body <span className='text-red-500'>*</span>
                      </FormLabel>
                      <FormControl>
                        <RichTextEditor {...field} onValueChange={field.onChange} />
                      </FormControl>
                      <FormMessage />
                    </FormItem>
                  )}
                />
              </div>
              <div className='flex flex-row-reverse w-full'>
                <Button disabled={electionRound?.status === ElectionRoundStatus.Archived}>Send notification</Button>
              </div>
            </form>
          </Form>
        </CardContent>
      </Card>
    </Layout>
  );
}

export default CitizenNotificationMessageForm;
