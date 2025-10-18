import { createFileRoute } from "@tanstack/react-router";
import Page from "@/pages/NgoAdmin/GuidesObservers/Page"
import {zodValidator} from "@tanstack/zod-adapter";
import {guidesObserversSearchSchema} from "@/types/guides-observer.ts";

export const Route = createFileRoute(
  "/(app)/elections/$electionRoundId/guides/"
)({
  component: Page,
    validateSearch: zodValidator(guidesObserversSearchSchema)
});
