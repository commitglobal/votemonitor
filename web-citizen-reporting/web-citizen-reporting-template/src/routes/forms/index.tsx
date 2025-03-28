import ReportingFormsList from "@/pages/ReportingFormsList";
import { createFileRoute } from "@tanstack/react-router";

export const Route = createFileRoute("/forms/")({
  component: ReportingFormsList,
});
