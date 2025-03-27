import TextGuide from "@/pages/TextGuide";
import { createFileRoute } from "@tanstack/react-router";

export const Route = createFileRoute("/guides/$id")({
  component: TextGuide,
});
