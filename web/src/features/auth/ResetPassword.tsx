import { zodResolver } from '@hookform/resolvers/zod';
import { useForm } from 'react-hook-form';
import { z } from 'zod';

import { Button } from '@/components/ui/button';
import { Form, FormControl, FormField, FormItem, FormLabel, FormMessage } from '@/components/ui/form';
import { Input } from '@/components/ui/input';
import { Card, CardContent, CardDescription, CardFooter, CardHeader, CardTitle } from '@/components/ui/card';
import Logo from '@/components/layout/Header/Logo';

import { Route as ResetPasswordRoute } from '@/routes/reset-password/index'
import { useMutation } from '@tanstack/react-query';
import { noAuthApi } from '@/common/no-auth-api';


const formSchema = z.object({
  password: z.string().min(6, { message: 'Password is mandatory and must bt at least 6 characters long' }),
  confirmPassword: z.string().min(6, { message: 'Password is mandatory and must bt at least 6 characters long' }),
}).refine(
  (values) => {
    return values.password === values.confirmPassword;
  },
  {
    message: "Passwords must match!",
    path: ["confirmPassword"],
  }
);

function ResetPassword() {
  const { token } = ResetPasswordRoute.useSearch();

  const form = useForm<z.infer<typeof formSchema>>({
    resolver: zodResolver(formSchema),
    defaultValues: {
      password: '',
      confirmPassword: '',
    },
  });

  const resetPasswordMutation = useMutation<ResetPasswordRequest>({
    mutationFn: (obj) => {
      const electionRoundId: string | null = localStorage.getItem('electionRoundId');
      const monitoringNgoId: string | null = localStorage.getItem('monitoringNgoId');

      return noAuthApi.post<void>(
        `/election-rounds/${electionRoundId}/monitoring-ngos/${monitoringNgoId}/monitoring-observers/${observer.id}`,
        obj
      );
    },

    onSuccess: () => {
      toast({
        title: 'Success',
        description: 'Observer successfully updated',
      });
    },
  });


  const onSubmit = async (values: z.infer<typeof formSchema>) => {
    
  };

  return (
    <div className='w-screen h-screen flex flex-col justify-center items-center gap-8'>
      <Form {...form}>
        <form onSubmit={form.handleSubmit(onSubmit)} className='space-y-8'>
          <Card className='w-full max-w-sm'>
            <CardHeader>
              <div className='flex'>
                <div>
                  <CardTitle className='text-2xl'>Reset your password</CardTitle>
                  <CardDescription>Setup your new password</CardDescription>
                </div>
                <Logo width={56} height={56} />
              </div>
            </CardHeader>
            <CardContent className='grid gap-4'>
              <FormField
                control={form.control}
                name='password'
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Password</FormLabel>
                    <FormControl>
                      <Input type='password' {...field} />

                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />

              <FormField
                control={form.control}
                name='confirmPassword'
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Confirm your password</FormLabel>
                    <FormControl>
                      <Input type='password' {...field} />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />
            </CardContent>
            <CardFooter>
              <Button type='submit' className='w-full'>
                Reset password
              </Button>
            </CardFooter>
          </Card>
        </form>
      </Form>
    </div>
  );
}

export default ResetPassword;
