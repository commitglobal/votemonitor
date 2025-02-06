import { FormQuestions } from "@/components/FormQuestions";
import { useFormAnswersStore } from "@/components/questionsEditor/answers-store";
import { Button } from "@/components/ui/button";
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import { useCitizenForms } from "@/hooks";
import { createFileRoute, Link } from "@tanstack/react-router";

export const Route = createFileRoute("/forms/$formId")({
  component: RouteComponent,
});

function RouteComponent() {
  const { formId } = Route.useParams();
  const answers = useFormAnswersStore((s) => s.answers);
  const { DEFAULT_LANGUAGE } = Route.useRouteContext();

  const { data } = useCitizenForms();
  const formData = data?.forms.find((form) => form.id === formId);
  const FORM_LANGUAGE = formData?.defaultLanguage ?? DEFAULT_LANGUAGE;
  return (
    <div className="max-w-7xl mx-auto mt-4 md:mt-12">
      <Card>
        <CardHeader>
          <CardTitle>{formData?.name[FORM_LANGUAGE]}</CardTitle>
          {formData?.description && (
            <CardDescription>
              {formData?.description[FORM_LANGUAGE]}
            </CardDescription>
          )}
          <Link to="/" className="text-purple-900">
            Go back to the homepage
          </Link>
        </CardHeader>
        <CardContent>
          <FormQuestions
            questions={formData?.questions}
            languageCode="EN"
            noContentMessage="No questions to answer"
          />
          <Button onClick={() => console.log(answers)}>Submit</Button>
        </CardContent>
      </Card>
    </div>
  );
}
