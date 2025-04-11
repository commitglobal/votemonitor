import HelloPage from "@/pages/NgoAdmin/HelloPage/Page";
import { createFileRoute } from "@tanstack/react-router";

export const Route = createFileRoute("/hello/$name")({
  component: HelloPage,
});
