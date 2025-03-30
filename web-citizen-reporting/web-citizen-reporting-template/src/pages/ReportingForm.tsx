import { DevTool } from "@hookform/devtools";
import { useForm, type FieldValues } from "react-hook-form";

import AnswersForm from "@/components/AnswersForm";
import LocationForm from "@/components/LocationForm";
import { Button } from "@/components/ui/button";
import { Form } from "@/components/ui/form";
import { defineStepper } from "@/components/ui/stepper";
import ThankYou from "./ThankYou";
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
    Component: LocationForm,
  },
  {
    id: "answers",
    title: "Answers",
    Component: AnswersForm,
  },
  {
    id: "complete",
    title: "Complete",
    Component: ThankYou,
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
  });

  const onSubmit = (values: FieldValues) => {
    console.log(`Form values for step ${methods.current.id}:`, values);
  };

  return (
    <Form {...form}>
      <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-4">
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
          complete: ({ Component }) => <Component />,
        })}
        <StepperControls>
          {!methods.isLast && !methods.isFirst && (
            <Button
              variant="secondary"
              onClick={methods.prev}
              disabled={methods.isFirst}
            >
              Previous
            </Button>
          )}
          <Button
            type="submit"
            onClick={() => {
              methods.beforeNext(async () => {
                const valid = await form.trigger();
                if (!valid) return false;
                return true;
              });
            }}
          >
            {methods.isLast ? "Send" : "Next"}
          </Button>
        </StepperControls>
        <DevTool control={form.control} /> {/* set up the dev tool */}
      </form>
    </Form>
  );
};
