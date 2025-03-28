import { Button } from "@/components/ui/button";
import {
  Card,
  CardContent,
  CardFooter,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import { useForms } from "@/queries/use-forms";
import { Route } from "@/routes/forms/$formId";
import { notFound } from "@tanstack/react-router";

function ReportingForm() {
  const { formId } = Route.useParams();
  const { data: form } = useForms((forms) =>
    forms.find((f) => f.id === formId)
  );

  if (form === undefined) {
    throw notFound({ throw: false });
  }

  return (
    <Card>
      <CardHeader>
        <CardTitle>{form.name[form.defaultLanguage]}</CardTitle>
      </CardHeader>
      <CardContent></CardContent>
      <CardFooter className="w-full flex justify-end">
        <div className="flex gap-4">
          <Button variant="outline">Cancel</Button>
          <Button>Submit</Button>
        </div>
      </CardFooter>
    </Card>
  );
}

export default ReportingForm;
