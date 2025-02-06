import { FormQuestions } from "@/components/FormQuestions";
import { LocationSelector } from "@/components/LocationSelector";
import { useFormAnswersStore } from "@/components/questionsEditor/answers-store";
import { Button } from "@/components/ui/button";
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import {
  useCitizenForms,
  useLocationFilters,
  usePostFormMutation,
} from "@/hooks";
import { createFileRoute, Link } from "@tanstack/react-router";
import { v4 as uuidv4 } from "uuid";

export const Route = createFileRoute("/forms/$formId")({
  component: RouteComponent,
});

function RouteComponent() {
  const { formId } = Route.useParams();
  const answersFromStore = useFormAnswersStore((s) => s.answers);
  const { DEFAULT_LANGUAGE, ELECTION_ROUND_ID } = Route.useRouteContext();
  const { data } = useCitizenForms(ELECTION_ROUND_ID);
  const { search, locationId, handleLocationChange } =
    useLocationFilters(ELECTION_ROUND_ID);

  const { postFormMutation } = usePostFormMutation(ELECTION_ROUND_ID);
  const formData = data?.forms.find((form) => form.id === formId);
  const FORM_LANGUAGE = formData?.defaultLanguage ?? DEFAULT_LANGUAGE;

  const handleSubmit = () => {
    const answers = Object.values(answersFromStore);

    postFormMutation.mutate({
      citizenReportId: uuidv4(),
      formId,
      answers,
      locationId,
    });
  };

  return (
    <>
      <div className="flex flex-col  justify-center items-center">
        <h1 className="mb-4 text-4xl font-extrabold leading-none tracking-tight text-purple-900 md:text-5xl lg:text-6xl dark:text-white">
          Citizen Reporting
        </h1>
        <Link
          to="/"
          className="text-lg font-normal text-gray-500 lg:text-xl sm:px-16 xl:px-48 dark:text-gray-400"
        >
          Go back to the homepage
        </Link>
      </div>
      <div className="max-w-7xl mx-auto mt-4 md:mt-12">
        <Card>
          <CardHeader>
            <CardTitle>{formData?.name[FORM_LANGUAGE]}</CardTitle>
            {formData?.description && (
              <CardDescription>
                {formData?.description[FORM_LANGUAGE]}
              </CardDescription>
            )}
          </CardHeader>
          <CardContent>
            <FormQuestions
              questions={formData?.questions}
              languageCode="EN"
              noContentMessage="No questions to answer"
            />

            <LocationSelector
              search={search}
              handleReducerSearch={handleLocationChange}
            />
            <Button
              className="mt-8"
              title="Submit"
              onClick={() => handleSubmit()}
            >
              Submit
            </Button>
          </CardContent>
        </Card>
      </div>
    </>
  );
}
