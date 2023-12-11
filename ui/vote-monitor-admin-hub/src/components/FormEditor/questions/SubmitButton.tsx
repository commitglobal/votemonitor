import { useCallback } from "react";


interface SubmitButtonProps {
  isLastQuestion: boolean;
  onClick: () => void;
  focus?: boolean;
  tabIndex?: number;
  type?: "submit" | "button";
}

function SubmitButton({
  isLastQuestion,
  onClick,
  tabIndex = 1,
  focus = false,
  type = "submit",
}: SubmitButtonProps) {
  const buttonRef = useCallback(
    (currentButton: HTMLButtonElement | null) => {
      if (currentButton && focus) {
        setTimeout(() => {
          currentButton.focus();
        }, 200);
      }
    },
    [focus]
  );

  return (
    <button
      ref={buttonRef}
      type={type}
      tabIndex={tabIndex}
      autoFocus={focus}
      className="bg-brand border-submit-button-border text-on-brand focus:ring-focus flex items-center rounded-md border px-3 py-3 text-base font-medium leading-4 shadow-sm hover:opacity-90 focus:outline-none focus:ring-2 focus:ring-offset-2"
      onClick={onClick}>
      {isLastQuestion ? "Finish" : "Next"}
    </button>
  );
}
export default SubmitButton;
