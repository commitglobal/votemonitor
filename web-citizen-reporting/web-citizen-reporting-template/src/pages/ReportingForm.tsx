import { useForms } from "@/queries/use-forms";
import { Route } from "@/routes/forms/$formId";

function ReportingForm() {
  const { formId } = Route.useParams();
  const { data: form } = useForms((forms) =>
    forms.find((f) => f.id === formId)
  );
  return <>{JSON.stringify(form)}</>;
}

export default ReportingForm;
