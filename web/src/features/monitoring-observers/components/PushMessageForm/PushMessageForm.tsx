import Layout from '@/components/layout/Layout';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardFooter } from '@/components/ui/card';
import { Form, FormControl, FormDescription, FormField, FormItem, FormLabel, FormMessage } from '@/components/ui/form';
import { Input } from '@/components/ui/input';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select';
import { Textarea } from '@/components/ui/textarea';
import { zodResolver } from '@hookform/resolvers/zod';
import { useForm } from 'react-hook-form';
import { z } from 'zod';

function PushMessageForm() {
  const createPushMessageSchema = z.object({
    title: z.string().min(1, { message: 'Your message must have a title before sending.' }),
    messageBody: z
      .string()
      .min(1, { message: 'Your message must have a detailed description before sending.' })
      .max(1000),
    recipients: z.string(),
    location: z.string(),
    location1: z.string(),
    location2: z.string(),
    location3: z.string(),
  });

  const form = useForm<z.infer<typeof createPushMessageSchema>>({
    resolver: zodResolver(createPushMessageSchema),
  });

  function onSubmit(values: z.infer<typeof createPushMessageSchema>) {
    console.log(values);
  }

  return (
    <Layout title='Create new message'>
      <Card className='w-[800px] py-6'>
        <CardContent className='flex flex-col gap-6 items-baseline'>
          <Form {...form}>
            <form onSubmit={form.handleSubmit(onSubmit)} className='space-y-8'>
              <FormField
                control={form.control}
                name='title'
                render={({ field }) => (
                  <FormItem className='w-[540px]'>
                    <FormLabel className='text-left'>
                      Title of message <span className='text-red-500'>*</span>
                    </FormLabel>
                    <FormControl>
                      <Input {...field} />
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
                  <FormItem className='w-[540px]'>
                    <FormLabel className='text-left'>
                      Message body <span className='text-red-500'>*</span>
                    </FormLabel>
                    <FormControl>
                      <Textarea rows={8} className='resize-none' {...field} />
                    </FormControl>
                    <FormDescription>1000 characters</FormDescription>
                    <FormMessage />
                  </FormItem>
                )}
              />

              <FormField
                control={form.control}
                name='recipients'
                render={({ field }) => (
                  <FormItem className='w-[540px]'>
                    <FormLabel>
                      Recipients <span className='text-red-500'>*</span>
                    </FormLabel>
                    <Select onValueChange={field.onChange} defaultValue={field.value}>
                      <FormControl>
                        <SelectTrigger>
                          <SelectValue placeholder='A subgroup of observers' />
                        </SelectTrigger>
                      </FormControl>
                      <SelectContent>
                        <SelectItem value='subgroup'>A subgroup of observers</SelectItem>
                      </SelectContent>
                    </Select>

                    <FormMessage />
                  </FormItem>
                )}
              />

              <FormField
                control={form.control}
                name='location'
                render={({ field }) => (
                  <FormItem className='w-[540px]'>
                    <FormLabel>
                      Filter recipients by <span className='text-red-500'>*</span>
                    </FormLabel>
                    <Select onValueChange={field.onChange} defaultValue={field.value}>
                      <FormControl>
                        <SelectTrigger>
                          <SelectValue placeholder='Location' />
                        </SelectTrigger>
                      </FormControl>
                      <SelectContent>
                        <SelectItem value='subgroup'>Location</SelectItem>
                      </SelectContent>
                    </Select>

                    <FormMessage />
                  </FormItem>
                )}
              />

              <FormField
                control={form.control}
                name='location1'
                render={({ field }) => (
                  <FormItem className='w-[540px]'>
                    <FormLabel>
                      Location (Level 1) <span className='text-red-500'>*</span>
                    </FormLabel>
                    <Select onValueChange={field.onChange} defaultValue={field.value}>
                      <FormControl>
                        <SelectTrigger>
                          <SelectValue placeholder='Location' />
                        </SelectTrigger>
                      </FormControl>
                      <SelectContent>
                        <SelectItem value='subgroup'>Location</SelectItem>
                      </SelectContent>
                    </Select>

                    <FormMessage />
                  </FormItem>
                )}
              />

              <FormField
                control={form.control}
                name='location2'
                render={({ field }) => (
                  <FormItem className='w-[540px]'>
                    <FormLabel>Location (Level 2)</FormLabel>
                    <Select onValueChange={field.onChange} defaultValue={field.value}>
                      <FormControl>
                        <SelectTrigger>
                          <SelectValue placeholder='Location' />
                        </SelectTrigger>
                      </FormControl>
                      <SelectContent>
                        <SelectItem value='subgroup'>Location</SelectItem>
                      </SelectContent>
                    </Select>

                    <FormMessage />
                  </FormItem>
                )}
              />

              <FormField
                control={form.control}
                name='location3'
                render={({ field }) => (
                  <FormItem className='w-[540px]'>
                    <FormLabel>Location (Level 3)</FormLabel>
                    <Select onValueChange={field.onChange} defaultValue={field.value}>
                      <FormControl>
                        <SelectTrigger>
                          <SelectValue placeholder='Location' />
                        </SelectTrigger>
                      </FormControl>
                      <SelectContent>
                        <SelectItem value='subgroup'>Location</SelectItem>
                      </SelectContent>
                    </Select>

                    <FormMessage />
                  </FormItem>
                )}
              />
              <div className='fixed bottom-0 left-0 bg-white py-4 px-12 flex justify-end w-screen'>
                <Button>Send message to 152 observers</Button>
              </div>
            </form>
          </Form>
        </CardContent>
      </Card>
    </Layout>
  );
}

export default PushMessageForm;
