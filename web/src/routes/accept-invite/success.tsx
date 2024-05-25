import AcceptInviteSuccess from '@/features/auth/AcceptInviteSuccess';
import { createFileRoute } from '@tanstack/react-router';

export const Route = createFileRoute('/accept-invite/success')({
  component: AcceptInviteSuccess,
});
