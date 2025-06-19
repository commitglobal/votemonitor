import LoginPage from '@/features/auth/LoginPage';
import { createFileRoute, redirect } from '@tanstack/react-router';
import { z } from 'zod';

const fallback = '/' as const;

export const Route = createFileRoute('/(auth)/login')({
  validateSearch: z.object({
    redirect: z.string().optional().catch(''),
  }),
  beforeLoad: ({ context, search }) => {
    if (context.authContext.isAuthenticated) {
      throw redirect({ to: search.redirect || fallback });
    }
  },
  component: () => {
    const { redirect } = Route.useSearch();
    return <LoginPage redirect={redirect || fallback} />;
  },
});
