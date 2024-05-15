import ForgotPassword from '@/features/auth/ForgotPassword';
import { createFileRoute } from '@tanstack/react-router';

export const Route = createFileRoute('/forgot-password/')({
  component: ForgotPassword,
});
