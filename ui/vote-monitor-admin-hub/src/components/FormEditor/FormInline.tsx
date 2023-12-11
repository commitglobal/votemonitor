import { Form, FormBaseProps } from "./Form";

export function FormInline({
  form,
  activeQuestionId,
  onDisplay = () => {},
  onActiveQuestionChange = () => {},
  onResponse = () => {},
  prefillResponseData,
  responseCount,
}: FormBaseProps) {
  return (
    <div id="fbjs" className="formbricks-form h-full w-full">
      <Form
        form={form}
        activeQuestionId={activeQuestionId}
        onDisplay={onDisplay}
        onActiveQuestionChange={onActiveQuestionChange}
        onResponse={onResponse}
        prefillResponseData={prefillResponseData}
        responseCount={responseCount}
      />
    </div>
  );
}
