import {
  AlertDialog,
  AlertDialogAction,
  AlertDialogCancel,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogTitle
} from "@/components/ui/alert-dialog";
import { Input } from "@/components/ui/input";
import { Separator } from "@/components/ui/separator";

import * as React from "react";
import { buttonVariants } from "./button";

export const AlertDialogContext = React.createContext<(params: AlertAction) => Promise<boolean>>(() => null!);

export type AlertAction =
  {
    type: "confirm";
    title: string | React.ReactNode;
    body?: string | React.ReactNode;
    cancelButton?: string;
    cancelButtonClass?: string;
    actionButton?: string;
    actionButtonClass?: string;
    /** When set, the action button stays disabled until the user types this exact text. */
    confirmationText?: string;
  }
  | { type: "close" };

interface AlertDialogState {
  open: boolean;
  title: string | React.ReactNode;
  body: string | React.ReactNode;
  type: "confirm";
  cancelButton?: string;
  cancelButtonClass?: string;
  actionButton: string;
  actionButtonClass?: string;
  confirmationText?: string;
}

export function alertDialogReducer(
  state: AlertDialogState,
  action: AlertAction
): AlertDialogState {
  switch (action.type) {
    case "close":
      return { ...state, open: false };
    case "confirm":
      return {
        ...state,
        open: true,
        ...action,
        cancelButton: action.cancelButton,
        actionButton: action.actionButton || 'Ok',
        actionButtonClass: action.actionButtonClass ?? buttonVariants({ variant: "default" }),
        confirmationText: action.confirmationText
      };
    default:
      return state;
  }
}

export function AlertDialogProvider({
  children,
}: {
  children: React.ReactNode;
}) {
  const [state, dispatch] = React.useReducer(alertDialogReducer, {
    open: false,
    title: "",
    body: "",
    type: "confirm",
    cancelButton: "Cancel",
    actionButton: "Okay",
  });

  const resolveRef = React.useRef<(tf: any) => void>();
  const [typedConfirmation, setTypedConfirmation] = React.useState("");
  const isConfirmationPending = !!state.confirmationText && typedConfirmation !== state.confirmationText;

  function close() {
    dispatch({ type: "close" });
    resolveRef.current?.(false);
  }

  function confirm() {
    dispatch({ type: "close" });
    resolveRef.current?.(true);
  }

  const dialog = React.useCallback(async <T extends AlertAction>(params: T) => {
    setTypedConfirmation("");
    dispatch(params);

    return new Promise<boolean>((resolve) => {
      resolveRef.current = resolve;
    });
  }, []);

  return (
    <AlertDialogContext.Provider value={dialog}>
      {children}
      <AlertDialog
        open={state.open}
        onOpenChange={(open) => {
          if (!open) close();
          return;
        }}
      >
        <AlertDialogContent>
          <AlertDialogHeader>
            <AlertDialogTitle>{state.title}</AlertDialogTitle>
            <Separator />
            {state.body ? (<AlertDialogDescription>{state.body}</AlertDialogDescription>) : null}
            {state.confirmationText ? (
              <div className="space-y-2">
                <p className="text-sm text-muted-foreground">
                  Type <span className="font-mono font-semibold text-foreground">{state.confirmationText}</span> to confirm.
                </p>
                <Input
                  autoFocus
                  value={typedConfirmation}
                  onChange={(e) => setTypedConfirmation(e.target.value)}
                  onKeyDown={(e) => {
                    if (e.key === "Enter" && !isConfirmationPending) confirm();
                  }}
                />
              </div>
            ) : null}
          </AlertDialogHeader>
          <AlertDialogFooter>
            {state.cancelButton ? (
              <AlertDialogCancel
                onClick={close}
                className={state.cancelButtonClass}>
                {state.cancelButton}
              </AlertDialogCancel>) : null}
            <AlertDialogAction
              disabled={isConfirmationPending}
              onClick={confirm}
              className={state.actionButtonClass}>
              {state.actionButton}
            </AlertDialogAction>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>

    </AlertDialogContext.Provider>
  );
}
type Params<T extends "confirm"> = Omit<Extract<AlertAction, { type: T }>, "type"> | string;

export function useConfirm() {
  const dialog = React.useContext(AlertDialogContext);

  return React.useCallback((params: Params<"confirm">) => {
    return dialog({
      ...(typeof params === "string" ? { title: params } : params),
      type: "confirm"
    });
  },
    [dialog]
  );
}