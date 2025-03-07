import { zodResolver } from '@hookform/resolvers/zod';
import { useForm } from 'react-hook-form';
import { z } from 'zod';

import { Button } from '@/components/ui/button';
import { Form, FormControl, FormField, FormItem, FormLabel, FormMessage } from '@/components/ui/form';
import { Input } from '@/components/ui/input';
import { useContext } from 'react';
import { AuthContext } from '@/context/auth.context';
import { LoginDTO } from '@/common/auth-api';
import { useNavigate } from '@tanstack/react-router';
import { Card, CardContent, CardDescription, CardFooter, CardHeader, CardTitle } from '@/components/ui/card';
import Logo from '@/components/layout/Header/Logo';

const formSchema = z.object({
  email: z
    .string()
    .min(1, {
      message: 'Email is mandatory',
    })
    .email({ message: 'Email format is not correct' }),
  password: z.string().min(1, { message: 'Password is mandatory' }),
});

function Login() {
  const { signIn } = useContext(AuthContext);
  const navigate = useNavigate();
  const form = useForm<z.infer<typeof formSchema>>({
    resolver: zodResolver(formSchema),
    mode: 'all',
    defaultValues: {
      email: '',
      password: '',
    },
  });

  const onSubmit = async (values: z.infer<typeof formSchema>) => {
    const signedIn: boolean = await signIn(values as LoginDTO);
    if (signedIn) {
      navigate({ to: '/' });
    }
  };

  return (
    <div className='w-screen h-screen flex flex-col justify-center items-center gap-8'>
      <Form {...form}>
        <form onSubmit={form.handleSubmit(onSubmit)} className='space-y-8'>
          <Card className='w-full max-w-sm'>
            <CardHeader>
              <div className='flex'>
                <div>
                  <CardTitle className='text-2xl'>Login</CardTitle>
                  <CardDescription>Enter your email below to login to your account.</CardDescription>
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
                      <Input {...field} />
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
                      <Input type='password' {...field} />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />
            </CardContent>
            <CardFooter>
              <Button type='submit' className='w-full'>
                Sign in
              </Button>
            </CardFooter>
          </Card>
        </form>
      </Form>
    </div>
  );
}

export default Login;
