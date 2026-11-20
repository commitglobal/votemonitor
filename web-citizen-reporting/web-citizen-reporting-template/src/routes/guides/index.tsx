import GuidesList from "@/pages/GuidesList";
import { createFileRoute } from "@tanstack/react-router";

export const Route = createFileRoute("/guides/")({
  component: GuidesList,
});
