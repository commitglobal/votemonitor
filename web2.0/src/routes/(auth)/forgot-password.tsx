import z from 'zod'
import { createFileRoute, redirect } from '@tanstack/react-router'
import ForgotPasswordPage from '@/pages/Auth/ForgotPasswordPage'

const fallback = '/'

export const Route = createFileRoute('/(auth)/forgot-password')({
  validateSearch: z.object({
    redirect: z.string().optional().default('').catch(''),
  }),
  beforeLoad: ({ context, search }) => {
    if (context.auth.isAuthenticated) {
      throw redirect({ to: search.redirect || fallback })
    }
  },
  component: ForgotPasswordPage,
})
