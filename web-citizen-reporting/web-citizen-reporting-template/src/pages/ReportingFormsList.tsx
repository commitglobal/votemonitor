import { Button } from "@/components/ui/button";
import { useForms } from "@/queries/use-forms";
import { Link } from "@tanstack/react-router";

function ReportingFormsList() {
  const { data: forms } = useForms();

  return (
    <>
      {forms?.map((form) => (
        <div key={form.id} className="flex gap-4">
          <span>{form.name[form.defaultLanguage]}</span>
          <Button asChild variant="link">
            <Link to={"/forms/$formId"} params={{ formId: form.id }}>
              Fill in
            </Link>
          </Button>
        </div>
      ))}
    </>
  );
}

export default ReportingFormsList;
