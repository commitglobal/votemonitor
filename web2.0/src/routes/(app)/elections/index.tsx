import { createFileRoute, Navigate } from "@tanstack/react-router";

export const Route = createFileRoute("/(app)/elections/")({
  component: () => <Navigate to="/" replace={true} />,
});
