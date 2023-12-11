import LogicEditor from "./LogicEditor";
import { FormModel, TFormQuestion } from "@/redux/api/types";

interface AdvancedSettingsProps {
  question: TFormQuestion;
  questionIdx: number;
  localForm: FormModel;
  updateQuestion: (questionIdx: number, updatedAttributes: any) => void;
}

export default function AdvancedSettings({
  question,
  questionIdx,
  localForm,
  updateQuestion,
}: AdvancedSettingsProps) {
  return (
    <div>
      <div className="mb-4">
        <LogicEditor
          question={question}
          updateQuestion={updateQuestion}
          localForm={localForm}
          questionIdx={questionIdx}
        />
      </div>
    </div>
  );
}
