import ReportingFormsList from "@/pages/ReportingFormsList";
import { formsOptions } from "@/queries/use-forms";
import { createFileRoute } from "@tanstack/react-router";

export const Route = createFileRoute("/forms/")({
  loader: (opts) => opts.context.queryClient.ensureQueryData(formsOptions()),
  component: ReportingFormsList,
});
