import { ResetPasswordSuccess } from '@/features/auth/ResetPasswordSuccess';
import { createFileRoute } from '@tanstack/react-router';

export const Route = createFileRoute('/(auth)/reset-password/success')({
  component: ResetPasswordSuccess,
});
