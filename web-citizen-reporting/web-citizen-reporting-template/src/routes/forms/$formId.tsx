import NotFound from "@/pages/NotFound";
import ReportingForm from "@/pages/ReportingForm";
import { formsOptions } from "@/queries/use-forms";
import { createFileRoute } from "@tanstack/react-router";

export const Route = createFileRoute("/forms/$formId")({
  loader: (opts) => opts.context.queryClient.ensureQueryData(formsOptions()),
  component: ReportingForm,
  notFoundComponent: NotFound,
});
