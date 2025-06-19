import ForgotPassword from '@/features/auth/ForgotPassword';
import { createFileRoute } from '@tanstack/react-router';

export const Route = createFileRoute('/(auth)/forgot-password')({
  component: ForgotPassword,
});
