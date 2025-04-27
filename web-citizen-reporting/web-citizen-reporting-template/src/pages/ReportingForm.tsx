import ReportAnswersStep from "@/components/ReportAnswersStep";
import ReportLocationStep, {
  locationSchema,
} from "@/components/ReportLocationStep";
import ReportReviewStep from "@/components/ReportReviewStep";
import { Button } from "@/components/ui/button";
import { Form } from "@/components/ui/form";
import { defineStepper } from "@/components/ui/stepper";
import { zodResolver } from "@hookform/resolvers/zod";
import { useForm, type FieldValues } from "react-hook-form";

const {
  StepperProvider,
  StepperControls,
  StepperNavigation,
  StepperStep,
  StepperTitle,
  useStepper,
} = defineStepper(
  {
    id: "location",
    title: "Location",
    Component: ReportLocationStep,
    schema: locationSchema,
  },
  {
    id: "answers",
    title: "Answers",
    Component: ReportAnswersStep,
    schema: undefined,
  },
  {
    id: "review",
    title: "Review",
    Component: ReportReviewStep,
    schema: undefined,
  }
);

export default function SubmitCitizenReport() {
  return (
    <StepperProvider>
      <CitizenReportStepperComponent />
    </StepperProvider>
  );
}

const CitizenReportStepperComponent = () => {
  const methods = useStepper();

  const form = useForm({
    mode: "onSubmit",
    resolver: methods.current.schema
      ? zodResolver(methods.current.schema)
      : undefined,
    // Pre-populate with any existing values for this step
    defaultValues: {
      selectedLevel1: "",
      selectedLevel2: "",
      selectedLevel3: "",
      selectedLevel4: "",
      selectedLevel5: "",
      locationId: "",
    },
  });

  // // Reset form when step changes
  // React.useEffect(() => {
  //   form.reset(formValues[methods.current.id] || {});
  // }, [methods.current.id, form.reset, formValues]);

  const onSubmit = (values: FieldValues) => {
    console.log(`Form values for step ${methods.current.id}:`, values);

    // Move to next step if not on the last step
    // if (!methods.isLast) {
    //   methods.next();
    // }
  };

  return (
    <Form {...form}>
      <form onSubmit={(e) => e.preventDefault()} className="space-y-4">
        <StepperNavigation>
          {methods.all.map((step) => (
            <StepperStep
              key={step.id}
              of={step.id}
              type={step.id === methods.current.id ? "submit" : "button"}
              onClick={async () => {
                const valid = await form.trigger();
                if (!valid) return;
                methods.goTo(step.id);
              }}
            >
              <StepperTitle>{step.title}</StepperTitle>
            </StepperStep>
          ))}
        </StepperNavigation>
        {methods.switch({
          location: ({ Component }) => <Component />,
          answers: ({ Component }) => <Component />,
          review: ({ Component }) => <Component />,
        })}
        <StepperControls>
          {!methods.isLast && (
            <Button
              variant="secondary"
              onClick={methods.prev}
              disabled={methods.isFirst}
            >
              Previous
            </Button>
          )}
          {methods.isLast && (
            <Button variant="secondary" onClick={methods.reset}>
              Reset
            </Button>
          )}
          <Button
            onClick={() => {
              if (methods.isLast) {
                return form.handleSubmit(onSubmit)();
              }
              methods.beforeNext(async () => {
                const fieldsToValidate: any = methods.current.schema
                  ? (Object.keys(methods.current.schema.shape) as string[])
                  : undefined;

                const valid = fieldsToValidate
                  ? await form.trigger(fieldsToValidate)
                  : await form.trigger();
                if (!valid) return false;

                return true;
              });
            }}
          >
            {methods.isLast ? "Submit" : "Next"}
          </Button>
        </StepperControls>
      </form>
    </Form>
  );
};
