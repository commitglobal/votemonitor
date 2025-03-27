import FormsList from "@/pages/FormsList";
import { createFileRoute } from "@tanstack/react-router";

export const Route = createFileRoute("/forms/")({
  component: FormsList,
});
