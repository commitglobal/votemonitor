import { zodResolver } from '@hookform/resolvers/zod';
import { useForm } from 'react-hook-form';
import { z } from 'zod';

import { Button } from '@/components/ui/button';
import { Form, FormControl, FormField, FormItem, FormLabel, FormMessage } from '@/components/ui/form';
import { Input } from '@/components/ui/input';
import { useNavigate } from '@tanstack/react-router';
import { Card, CardContent, CardDescription, CardFooter, CardHeader, CardTitle } from '@/components/ui/card';
import Logo from '@/components/layout/Header/Logo';
import { Route as AcceptInviteRoute } from '@/routes/accept-invite/index';
import { useMutation } from '@tanstack/react-query';
import { noAuthApi } from '@/common/no-auth-api';
import { toast } from '@/components/ui/use-toast';

const formSchema = z
  .object({
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

interface AcceptInviteRequest {
  password: string;
  confirmPassword: string;
  invitationToken: string;
}
function AcceptInvite() {
  const navigate = useNavigate();
  const { invitationToken } = AcceptInviteRoute.useSearch();

  const form = useForm<z.infer<typeof formSchema>>({
    resolver: zodResolver(formSchema),
    mode: 'all',
    defaultValues: {
      password: '',
      confirmPassword: '',
    },
  });

  const acceptInviteMutation = useMutation({
    mutationFn: async (obj: AcceptInviteRequest) => {
      return await noAuthApi.post<AcceptInviteRequest>(`/auth/accept-invite`, obj);
    },

    onSuccess: () => {
      toast({
        title: 'Success',
        description: 'Password was set successfully',
      });

      navigate({ to: '/accept-invite/success' });
    },

    onError: () => {
      toast({
        title: 'Error accepting invite',
        description: 'Please contact tech support',
        variant: 'destructive',
      });
    },
  });

  const onSubmit = async (values: z.infer<typeof formSchema>) => {
    acceptInviteMutation.mutate({
      password: values.password,
      confirmPassword: values.confirmPassword,
      invitationToken: invitationToken,
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
                  <CardTitle className='text-2xl'>Accept invite</CardTitle>
                  <CardDescription>Set up your password for the new account.</CardDescription>
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
                      <Input type='password' autoCorrect='off' autoCapitalize='none' autoComplete='off' {...field} />
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
                    <FormLabel>Confirm password</FormLabel>
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
                Accept invite
              </Button>
            </CardFooter>
          </Card>
        </form>
      </Form>
    </div>
  );
}

export default AcceptInvite;
