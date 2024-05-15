import { useRef, useState } from "react";

export function useDialog() {
  const [isOpen, setIsOpen] = useState(false);
  const triggerRef = useRef();

  function trigger() {
    setIsOpen(true);
  }

  function dismiss() {
    setIsOpen(false);
    // @ts-ignore
    triggerRef.current?.focus();
  }

  return {
    triggerProps: {
      ref: triggerRef,
      onClick: trigger,
    },
    dialogProps: {
      open: isOpen,
      // @ts-ignore
      onOpenChange: open => {
        if (open) trigger();
        else dismiss();
      },
    },
    trigger,
    dismiss,
  };
}
