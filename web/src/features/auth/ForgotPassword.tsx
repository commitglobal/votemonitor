import { zodResolver } from '@hookform/resolvers/zod';
import { useForm } from 'react-hook-form';
import { z } from 'zod';

import Logo from '@/components/layout/Header/Logo';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardDescription, CardFooter, CardHeader, CardTitle } from '@/components/ui/card';
import { Form, FormControl, FormField, FormItem, FormLabel, FormMessage } from '@/components/ui/form';
import { Input } from '@/components/ui/input';
import API from '@/services/api';
import { useMutation } from '@tanstack/react-query';
import { toast } from 'sonner';

const formSchema = z.object({
  email: z
    .string()
    .min(1, {
      message: 'Email is mandatory',
    })
    .email({ message: 'Email format is not correct' }),
});

interface ForgotPasswordRequest {
  email: string;
}

function ForgotPassword() {
  const form = useForm<z.infer<typeof formSchema>>({
    resolver: zodResolver(formSchema),
    mode: 'onChange',
    defaultValues: {
      email: '',
    },
  });

  const forgotPasswordMutation = useMutation({
    mutationFn: async (obj: ForgotPasswordRequest) => {
      return await API.post<ForgotPasswordRequest>(`auth/forgot-password`, obj);
    },

    onSuccess: () => {
      toast.success('An email was sent with reset password instructions');
    },
  });

  const onSubmit = async (values: z.infer<typeof formSchema>) => {
    forgotPasswordMutation.mutate({
      email: values.email,
    });
  };

  return (
    <div className='w-screen h-screen flex flex-col justify-center items-center gap-8'>
      <Form {...form}>
        <form onSubmit={form.handleSubmit(onSubmit)} className='space-y-8'>
          <Card className='w-full max-w-sm'>
            <CardHeader>
              <div className='flex'>
                <div>
                  <CardTitle className='text-2xl'>Forgot password?</CardTitle>
                  <CardDescription>Request a reset password link</CardDescription>
                </div>
                <Logo width={56} height={56} />
              </div>
            </CardHeader>
            <CardContent className='grid gap-4'>
              <FormField
                control={form.control}
                name='email'
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Email</FormLabel>
                    <FormControl>
                      <Input type='email' {...field} />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />
            </CardContent>
            <CardFooter>
              <Button type='submit' className='w-full'>
                Request password reset
              </Button>
            </CardFooter>
          </Card>
        </form>
      </Form>
    </div>
  );
}

export default ForgotPassword;
