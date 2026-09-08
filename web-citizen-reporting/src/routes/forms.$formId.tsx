import { FormQuestions } from "@/components/FormQuestions";
import { LocationSelector } from "@/components/LocationSelector";
import { useFormAnswersStore } from "@/components/questionsEditor/answers-store";
import { useConfirm } from "@/components/ui/alert-dialog-provider";
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
import { createFileRoute, useNavigate } from "@tanstack/react-router";
import { v4 as uuidv4 } from "uuid";

export const Route = createFileRoute("/forms/$formId")({
  component: RouteComponent,
});

function RouteComponent() {
  const { formId } = Route.useParams();
  const answersFromStore = useFormAnswersStore((s) => s.answers);
  const resetForm = useFormAnswersStore((s) => s.resetForm);
  const { DEFAULT_LANGUAGE, ELECTION_ROUND_ID } = Route.useRouteContext();
  const { data } = useCitizenForms(ELECTION_ROUND_ID);
  const { search, locationId, handleLocationChange } =
    useLocationFilters(ELECTION_ROUND_ID);

  const confirm = useConfirm();
  const navigate = useNavigate();

  const { postFormMutation } = usePostFormMutation(ELECTION_ROUND_ID);
  const formData = data?.forms.find((form) => form.id === formId);
  const FORM_LANGUAGE = formData?.defaultLanguage ?? DEFAULT_LANGUAGE;

  // single and multiselect empty answers are not valid

  const isValidAnswer = (answer: any) => {
    if (
      !["singleSelectAnswer", '"multiSelectAnswer"'].includes(
        answer.$answerType
      )
    )
      return true;

    if (answer.$answerType === "singleSelectAnswer" && answer.selection)
      return true;

    if (
      answer.$answerType === "multiSelectAnswer" &&
      answer.selection.length > 0
    )
      return true;

    return false;
  };

  const handleGoingBackToHomepage = async () => {
    if (
      await confirm({
        title: `Are you sure you want to go back?`,
        actionButton: "Yes",
        cancelButton: "Cancel",
        body: (
          <>
            Are you sure you want to go to the homepage and reset this form?
            This action cannot be undone
          </>
        ),
      })
    ) {
      resetForm();
      navigate({ to: "/" });
    }
  };

  const handleSubmit = () => {
    const answers = Object.values(answersFromStore).filter((answer) =>
      isValidAnswer(answer as any)
    );

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
        <a
          href="#"
          onClick={() => handleGoingBackToHomepage()}
          className="text-lg font-normal text-gray-500 lg:text-xl sm:px-16 xl:px-48 dark:text-gray-400 underline hover:no-underline"
        >
          Go back to the homepage
        </a>
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
