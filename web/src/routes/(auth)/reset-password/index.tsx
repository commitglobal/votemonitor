import ResetPassword from '@/features/auth/ResetPassword';
import { createFileRoute } from '@tanstack/react-router';
import { z } from 'zod';

const resetPasswordRouteSearchSchema = z.object({
  token: z.any().catch('')
});

export const Route = createFileRoute('/(auth)/reset-password/')({
  component: ResetPassword,
  validateSearch: resetPasswordRouteSearchSchema
});
