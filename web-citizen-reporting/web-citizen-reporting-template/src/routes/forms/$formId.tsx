import { currentFormLanguageAtom } from "@/features/forms/atoms";
import NotFound from "@/pages/NotFound";
import SubmitCitizenReport from "@/pages/ReportingForm";
import { formQueryOptions } from "@/queries/use-forms";
import { useSuspenseQuery } from "@tanstack/react-query";
import { createFileRoute, notFound } from "@tanstack/react-router";
import { useSetAtom } from "jotai";
import { useEffect } from "react";

export const Route = createFileRoute("/forms/$formId")({
  loader: async (opts) => {
    const { formId } = opts.params;
    const formData = await opts.context.queryClient.ensureQueryData(
      formQueryOptions(formId)
    );

    if (!formData) throw notFound();
    return formData;
  },
  component: () => {
    const { formId } = Route.useParams();
    const { data } = useSuspenseQuery(formQueryOptions(formId));
    const setCurrentLanguage = useSetAtom(currentFormLanguageAtom);

    useEffect(() => {
      if (!data?.defaultLanguage) return;
      setCurrentLanguage(data.defaultLanguage);
    }, [data]);

    return <SubmitCitizenReport />;
  },
  notFoundComponent: NotFound,
});
