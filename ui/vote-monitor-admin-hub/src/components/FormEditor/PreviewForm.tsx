import { useRef, } from "react";
import { FormModel } from "@/redux/api/types";
import { Button } from "../ui/button";
import { FormInline } from "./FormInline";

interface PreviewFormProps {
  form: FormModel;
  setActiveQuestionId: (id: string | null) => void;
  activeQuestionId?: string | null;
}


export default function PreviewForm({
  setActiveQuestionId,
  activeQuestionId,
  form,
}: PreviewFormProps) {
  const ContentRef = useRef<HTMLDivElement | null>(null);


  function resetQuestionProgress() {
    setActiveQuestionId(form?.questions[0]?.id);
  }


  return (
    <div className="flex h-full w-full flex-col items-center justify-items-center">

      <div
        className="relative flex h-[95] max-h-[95%] w-5/6 items-center justify-center rounded-lg border border-slate-300 bg-slate-200">


        <div className="flex h-full w-5/6 flex-1 flex-col">
          <div className="flex h-8 w-full items-center rounded-t-lg bg-slate-100">
            <div className="ml-4 flex w-full justify-between font-mono text-sm text-slate-400">

              <div className="flex items-center">
                <ResetProgressButton resetQuestionProgress={resetQuestionProgress} />
              </div>
            </div>
          </div>


          <div className="flex flex-grow flex-col overflow-y-auto rounded-b-lg" ref={ContentRef}>
            <div className="flex w-full flex-grow flex-col items-center justify-center bg-white p-4 py-6">
              <div className="w-full max-w-md">
                <FormInline
                  form={form}
                  activeQuestionId={activeQuestionId || undefined}
                  onActiveQuestionChange={setActiveQuestionId}
                />
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}

function ResetProgressButton({ resetQuestionProgress }) {
  return (
    <Button
      className="py-0.2 mr-2 bg-white px-2 font-sans text-sm text-slate-500"
      onClick={resetQuestionProgress}>
      Restart
    </Button>
  );
}
