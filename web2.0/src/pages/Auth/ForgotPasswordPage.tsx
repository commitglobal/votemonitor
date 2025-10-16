'use client'

import { useEffect, useState } from 'react'
import { z } from 'zod'
import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { Link, useRouterState } from '@tanstack/react-router'
import { useAuth } from '@/contexts/auth.context'
import { Send } from 'lucide-react'
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
  Dialog,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
} from '@/components/ui/dialog'
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
    .min(1, { message: 'This field is required!' })
    .pipe(z.email({ message: 'Invalid email address' })),
})

type DialogType = 'success' | 'error' | null

function ForgotPasswordPage() {
  const {
    forgotPassword,
    forgotPasswordApiResponse,
    setForgotPasswordApiResponse,
  } = useAuth()
  const isLoading = useRouterState({ select: (s) => s.isLoading })
  const form = useForm<z.infer<typeof formSchema>>({
    resolver: zodResolver(formSchema),
    defaultValues: {
      email: '',
    },
  })

  const [dialogOpen, setDialogOpen] = useState(false)
  const [dialogType, setDialogType] = useState<DialogType>(null)

  const isSendingEmail = isLoading || form.formState.isSubmitting

  const onSubmit = async (values: z.infer<typeof formSchema>) => {
    await forgotPassword(values.email)
  }

  useEffect(() => {
    if (forgotPasswordApiResponse) {
      if (forgotPasswordApiResponse.success) {
        setDialogType('success')
      } else {
        setDialogType('error')
      }
      setDialogOpen(true)
    }
  }, [forgotPasswordApiResponse])

  return (
    <div className='flex min-h-svh w-full items-center justify-center p-6 md:p-10'>
      <div className='w-full max-w-sm'>
        <div className='flex flex-col gap-6'>
          <Card>
            <CardHeader>
              <CardTitle className='text-2xl'>Forgot your password?</CardTitle>
              <CardDescription>
                Enter your email below and we will send you a link to reset it.
              </CardDescription>
            </CardHeader>
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
          </Card>
        </div>
      </div>
      <Dialog
        open={dialogOpen}
        onOpenChange={() => {
          setDialogOpen(false)
          setForgotPasswordApiResponse && setForgotPasswordApiResponse(null)
          setDialogType(null)
        }}
      >
        <DialogContent className='max-w-lg sm:max-w-xl md:max-w-2xl'>
          <DialogHeader>
            {dialogType === 'success' ? (
              <>
                <DialogTitle>Success</DialogTitle>
                <DialogDescription>
                  If an email address is associated with your account, you will
                  receive a password reset link shortly.
                </DialogDescription>

                <div className='mt-2 flex justify-start'>
                  <Button
                    className='h-auto p-0 text-sm underline underline-offset-4'
                    variant='link'
                    asChild
                  >
                    <Link
                      to='/login'
                      onClick={() => {
                        setForgotPasswordApiResponse &&
                          setForgotPasswordApiResponse(null)
                        setDialogType(null)
                      }}
                    >
                      Go back to the login page.
                    </Link>
                  </Button>
                </div>
              </>
            ) : (
              <>
                <DialogTitle>Error</DialogTitle>
                <DialogDescription>
                  There was a problem sending the reset link. Please try again.
                </DialogDescription>
              </>
            )}
          </DialogHeader>
        </DialogContent>
      </Dialog>
    </div>
  )
}

export default ForgotPasswordPage
