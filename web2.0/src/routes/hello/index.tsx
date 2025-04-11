import HelloNamelessPage from "@/pages/NgoAdmin/HelloNamelessPage";
import { createFileRoute } from "@tanstack/react-router";

export const Route = createFileRoute("/hello/")({
  component: () => <HelloNamelessPage />,
});
