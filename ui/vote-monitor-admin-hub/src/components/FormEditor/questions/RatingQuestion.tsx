import { cn } from "@/lib/utils";
import { TResponseData } from "@/redux/api/responses";
import { TFormRatingQuestion } from "@/redux/api/types";
import Headline from "./Headline";
import Subheader from "./Subheader";
import { BackButton } from "./BackButton";
import SubmitButton from "./SubmitButton";


interface RatingQuestionProps {
  question: TFormRatingQuestion;
  value: string | number | string[];
  onChange: (responseData: TResponseData) => void;
  onSubmit: (data: TResponseData) => void;
  onBack: () => void;
  isFirstQuestion: boolean;
  isLastQuestion: boolean;
}

export default function RatingQuestion({
  question,
  value,
  onChange,
  onSubmit,
  onBack,
  isFirstQuestion,
  isLastQuestion,
}: RatingQuestionProps) {

  const handleSelect = (number: number) => {
    onChange({ [question.id]: number });
    onSubmit(
      {
        [question.id]: number,
      }
    );
  };

  const HiddenRadioInput = ({ number }: { number: number }) => (
    <input
      type="radio"
      name="rating"
      value={number}
      className="absolute left-0 h-full w-full cursor-pointer opacity-0"
      onChange={() => handleSelect(number)}
      required={true}
      checked={value === number}
    />
  );

  return (
    <form
      onSubmit={(e) => {
        e.preventDefault();
        onSubmit({ [question.id]: value });
      }}
      className="w-full">
      <Headline headline={question.headline} questionId={question.id} />
      <Subheader subheader={question.subheader} questionId={question.id} />
      <div className="mb-4 mt-8">
        <fieldset>
          <legend className="sr-only">Choices</legend>
          <div className="flex">
            {Array.from({ length: question.range }, (_, i) => i + 1).map((number, i, a) => (
              <span
                key={number}
                className="max-w-10 relative max-h-10 flex-1 cursor-pointer text-center text-sm leading-10">

                <label
                  tabIndex={i + 1}
                  onKeyDown={(e) => {
                    if (e.key == "Enter") {
                      handleSelect(number);
                    }
                  }}
                  className={cn(
                    value === number ? "bg-accent-selected-bg border-border-highlight z-10" : "",
                    a.length === number ? "rounded-r-md" : "",
                    number === 1 ? "rounded-l-md" : "",
                    "text-heading hover:bg-accent-bg focus:bg-accent-bg block h-full w-full border focus:outline-none"
                  )}>
                  <HiddenRadioInput number={number} />
                  {number}
                </label>

              </span>
            ))}
          </div>
          <div className="text-subheading flex justify-between px-1.5 text-xs leading-6">
            <p className="w-1/2 text-left">{question.lowerLabel}</p>
            <p className="w-1/2 text-right">{question.upperLabel}</p>
          </div>
        </fieldset>
      </div>

      <div className="mt-4 flex w-full justify-between">
        {!isFirstQuestion && (
          <BackButton
            tabIndex={value ? question.range + 2 : question.range + 1}
            onClick={() => {
              onBack();
            }}
          />
        )}
        <div></div>
        {(value) && (
          <SubmitButton
            tabIndex={question.range + 1}
            isLastQuestion={isLastQuestion}
            onClick={() => { }}
          />
        )}
      </div>
    </form>
  );
}
