import GuideDetails from "@/pages/GuideDetails";
import { createFileRoute } from "@tanstack/react-router";

export const Route = createFileRoute("/guides/$guideId")({
  component: GuideDetails,
});
