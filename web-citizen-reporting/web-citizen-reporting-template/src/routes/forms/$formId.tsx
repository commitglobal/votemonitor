import NotFound from "@/pages/NotFound";
import ReportingForm from "@/pages/ReportingForm";
import { createFileRoute } from "@tanstack/react-router";

export const Route = createFileRoute("/forms/$formId")({
  component: ReportingForm,
  notFoundComponent: NotFound,
});
