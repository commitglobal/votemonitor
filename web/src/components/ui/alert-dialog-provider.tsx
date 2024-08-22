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
        actionButtonClass: action.actionButtonClass ?? buttonVariants({ variant: "default" })
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

  function close() {
    dispatch({ type: "close" });
    resolveRef.current?.(false);
  }

  function confirm() {
    dispatch({ type: "close" });
    resolveRef.current?.(true);
  }

  const dialog = React.useCallback(async <T extends AlertAction>(params: T) => {
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
          </AlertDialogHeader>
          <AlertDialogFooter>
            {state.cancelButton ? (
              <AlertDialogCancel
                onClick={close}
                className={state.cancelButtonClass}>
                {state.cancelButton}
              </AlertDialogCancel>) : null}
            <AlertDialogAction
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