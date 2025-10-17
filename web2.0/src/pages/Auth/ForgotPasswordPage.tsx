'use client'

import { useState } from 'react'
import { z } from 'zod'
import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { Link, useRouterState } from '@tanstack/react-router'
import { useResetPasswordMutation } from '@/mutations/auth-mutations'
import { Send } from 'lucide-react'
import { toast } from 'sonner'
import { Button } from '@/components/ui/button'
import {
  Card,
  CardContent,
  CardDescription,
  CardFooter,
  CardHeader,
  CardTitle,
} from '@/components/ui/card'
import {
  Form,
  FormControl,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from '@/components/ui/form'
import { Input } from '@/components/ui/input'

const formSchema = z.object({
  email: z
    .string()
    .min(1, { message: 'This field is required' })
    .pipe(z.email({ message: 'Invalid email address' })),
})

function ForgotPasswordPage() {
  const { mutate: resetPassword } = useResetPasswordMutation()
  const [displaySuccessMessage, setDisplaySuccessMessage] = useState(false)
  const form = useForm<z.infer<typeof formSchema>>({
    resolver: zodResolver(formSchema),
    defaultValues: {
      email: '',
    },
  })

  const isLoading = useRouterState({ select: (s) => s.isLoading })
  const isSendingEmail = isLoading || form.formState.isSubmitting

  const onSubmit = async (values: z.infer<typeof formSchema>) => {
    resetPassword(values.email, {
      onSuccess: () => {
        toast.success('Email sent successfully')
        setDisplaySuccessMessage(true)
      },
      onError: () => {
        toast.error('Failed to send email')
      },
    })
  }

  return (
    <div className='flex min-h-svh w-full items-center justify-center p-6 md:p-10'>
      <div className='w-full max-w-sm'>
        <div className='flex flex-col gap-6'>
          <Card>
            <CardHeader>
              <CardTitle className='text-2xl'>
                {displaySuccessMessage
                  ? 'Email sent sucessfully'
                  : 'Forgot your password?'}
              </CardTitle>
              <CardDescription>
                {displaySuccessMessage ? (
                  <div className='flex flex-col items-start'>
                    <span>
                      If there is an account associated with this email, you
                      should have received an email with a password reset link.
                    </span>
                    <Button
                      className='mt-2 h-auto p-0 text-sm underline underline-offset-4'
                      variant='link'
                      asChild
                    >
                      <Link to='/login'>Go back to the login page.</Link>
                    </Button>
                  </div>
                ) : (
                  'Enter your email below and we will send you a link to reset it.'
                )}
              </CardDescription>
            </CardHeader>
            {!displaySuccessMessage && (
              <Form {...form}>
                <form
                  id='forgot-password-form'
                  onSubmit={form.handleSubmit(onSubmit)}
                  className='flex flex-col gap-6'
                >
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
                  </CardContent>
                  <CardFooter>
                    <Button
                      type='submit'
                      className='flex w-full cursor-pointer items-center justify-center gap-2'
                      disabled={isSendingEmail}
                    >
                      <>
                        <Send className='h-4 w-4' />
                        Send reset link
                      </>
                    </Button>
                  </CardFooter>
                </form>
              </Form>
            )}
          </Card>
        </div>
      </div>
    </div>
  )
}

export default ForgotPasswordPage
