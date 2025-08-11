import LoginPage from "@/pages/LoginPage";
import { createFileRoute, redirect } from "@tanstack/react-router";
import { z } from "zod";

const fallback = "/" as const;

export const Route = createFileRoute("/(auth)/login")({
  validateSearch: z.object({
    redirect: z.string().optional().default("").catch(""),
  }),
  beforeLoad: ({ context, search }) => {
    if (context.auth.isAuthenticated && !context.auth.isLoading) {
      throw redirect({ to: search.redirect || fallback });
    }
  },
  component: LoginPage,
});
