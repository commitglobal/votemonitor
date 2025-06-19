import { zodResolver } from '@hookform/resolvers/zod';
import { useForm } from 'react-hook-form';
import { z } from 'zod';

import Logo from '@/components/layout/Header/Logo';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardDescription, CardFooter, CardHeader, CardTitle } from '@/components/ui/card';
import { Form, FormControl, FormField, FormItem, FormLabel, FormMessage } from '@/components/ui/form';
import { Input } from '@/components/ui/input';

import type { FunctionComponent } from '@/common/types';
import { PasswordInput } from '@/components/ui/password-input';
import { Route as ResetPasswordRoute } from '@/routes/(auth)/reset-password/index';
import API from '@/services/api';
import { useMutation } from '@tanstack/react-query';
import { useNavigate } from '@tanstack/react-router';
import { toast } from 'sonner';

interface ResetPasswordRequest {
  password: string;
  token: string;
  email: string;
}

const formSchema = z
  .object({
    email: z
      .string()
      .min(1, {
        message: 'Email is mandatory',
      })
      .email({ message: 'Email format is not correct' }),
    password: z.string().min(8, { message: 'Password is mandatory and must be at least 8 characters long' }),
    confirmPassword: z.string().min(8, { message: 'Password is mandatory and must be at least 8 characters long' }),
  })
  .refine(
    (values) => {
      return values.password === values.confirmPassword;
    },
    {
      message: 'Passwords must match!',
      path: ['confirmPassword'],
    }
  );

function ResetPassword(): FunctionComponent {
  const navigate = useNavigate();
  const { token } = ResetPasswordRoute.useSearch();

  const form = useForm<z.infer<typeof formSchema>>({
    resolver: zodResolver(formSchema),
    mode: 'onChange',
    defaultValues: {
      email: '',
      password: '',
      confirmPassword: '',
    },
  });

  const resetPasswordMutation = useMutation({
    mutationFn: (obj: ResetPasswordRequest) => {
      return API.post<ResetPasswordRequest>(`auth/reset-password`, obj);
    },

    onSuccess: () => {
      toast.success('Password was reset successfully');
      navigate({ to: '/reset-password/success' });
    },
  });

  const onSubmit = async (values: z.infer<typeof formSchema>) => {
    resetPasswordMutation.mutate({
      email: values.email,
      password: values.password,
      token: token,
    });
  };

  return (
    <div className='flex flex-col items-center justify-center w-screen h-screen gap-8'>
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
              <FormField
                control={form.control}
                name='password'
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Password</FormLabel>
                    <FormControl>
                      <PasswordInput
                        autoCorrect='off'
                        autoCapitalize='off'
                        autoComplete='off'
                        spellCheck='false'
                        {...field}
                      />
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
                      <PasswordInput
                        autoCorrect='off'
                        autoCapitalize='off'
                        autoComplete='off'
                        spellCheck='false'
                        {...field}
                      />
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
