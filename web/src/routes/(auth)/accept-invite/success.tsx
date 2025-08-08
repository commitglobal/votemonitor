import { AcceptInviteSuccess } from '@/features/auth/AcceptInviteSuccess';
import { createFileRoute } from '@tanstack/react-router';

export const Route = createFileRoute('/(auth)/accept-invite/success')({
  component: AcceptInviteSuccess,
});
