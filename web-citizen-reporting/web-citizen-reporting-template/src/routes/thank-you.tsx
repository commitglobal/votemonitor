import ThankYou from "@/pages/ThankYou";
import { createFileRoute } from "@tanstack/react-router";

export const Route = createFileRoute("/thank-you")({
  component: ThankYou,
});
