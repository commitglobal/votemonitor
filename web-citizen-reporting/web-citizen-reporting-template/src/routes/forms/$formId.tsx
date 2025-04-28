import NotFound from "@/pages/NotFound";
import SubmitCitizenReport from "@/pages/ReportingForm";
import { formsOptions } from "@/queries/use-forms";
import { createFileRoute, notFound } from "@tanstack/react-router";

export const Route = createFileRoute("/forms/$formId")({
  loader: async (opts) => {
    const { formId } = opts.params;
    const allForms = await opts.context.queryClient.ensureQueryData(
      formsOptions()
    );
    const formsMap = new Map(allForms.map((form) => [form.id, form]));
    if (!formsMap.has(formId)) throw notFound({ throw: false });

    return formsMap.get(formId);
  },
  component: SubmitCitizenReport,
  notFoundComponent: NotFound,
});
