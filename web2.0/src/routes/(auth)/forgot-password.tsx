import { createFileRoute } from '@tanstack/react-router'
import ForgotPasswordPage from '@/pages/Auth/ForgotPasswordPage'

export const Route = createFileRoute('/(auth)/forgot-password')({
  component: ForgotPasswordPage,
})
