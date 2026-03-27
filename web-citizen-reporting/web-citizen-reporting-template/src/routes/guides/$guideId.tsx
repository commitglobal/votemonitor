import GuideDetails from "@/pages/GuideDetails";
import NotFound from "@/pages/NotFound";
import { createFileRoute } from "@tanstack/react-router";

export const Route = createFileRoute("/guides/$guideId")({
  component: GuideDetails,
  notFoundComponent: NotFound,
});
