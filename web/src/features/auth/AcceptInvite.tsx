import { zodResolver } from '@hookform/resolvers/zod';
import { useForm } from 'react-hook-form';
import { z } from 'zod';
import API from '@/services/api';
import Logo from '@/components/layout/Header/Logo';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardDescription, CardFooter, CardHeader, CardTitle } from '@/components/ui/card';
import { Form, FormControl, FormField, FormItem, FormLabel, FormMessage } from '@/components/ui/form';
import { PasswordInput } from '@/components/ui/password-input';
import { Route as AcceptInviteRoute } from '@/routes/(auth)/accept-invite/index';
import { useMutation } from '@tanstack/react-query';
import { useNavigate } from '@tanstack/react-router';
import { toast } from 'sonner';

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
    mode: 'onChange',
    defaultValues: {
      password: '',
      confirmPassword: '',
    },
  });

  const acceptInviteMutation = useMutation({
    mutationFn: async (obj: AcceptInviteRequest) => {
      return await API.post<AcceptInviteRequest>(`/auth/accept-invite`, obj);
    },

    onSuccess: () => {
      toast.success('Password was set successfully');

      navigate({ to: '/accept-invite/success' });
    },

    onError: () => {
      toast.error('Error accepting invite', {
        description: 'Please contact tech support',
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
                    <FormLabel>Confirm password</FormLabel>
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
