import AcceptInvite from '@/features/auth/AcceptInvite';
import { createFileRoute } from '@tanstack/react-router';
import { z } from 'zod';

const acceptInviteRouteSearchSchema = z.object({
  invitationToken: z.any().catch(''),
});

export const Route = createFileRoute('/(auth)/accept-invite/')({
  component: AcceptInvite,
  validateSearch: acceptInviteRouteSearchSchema,
});
