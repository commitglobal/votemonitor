import CitizenForm from "@/pages/CitizenForm";
import { createFileRoute } from "@tanstack/react-router";

export const Route = createFileRoute("/forms/$id")({
  component: CitizenForm,
});
