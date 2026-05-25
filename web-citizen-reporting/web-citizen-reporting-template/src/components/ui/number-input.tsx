import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { cn } from "@/lib/utils";
import * as React from "react";

interface NumberInputProps extends React.InputHTMLAttributes<HTMLInputElement> {
  label?: string;
  helperText?: string;
  error?: string;
  min?: number;
  max?: number;
  step?: number;
  onChange?: (value: any) => void;
  className?: string;
}

export function NumberInput({
  label,
  helperText,
  error,
  min,
  max,
  step = 1,
  onChange,
  className,
  id,
  ...props
}: NumberInputProps) {
  const inputId = React.useId();

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const value = e.target.value === "" ? undefined : Number(e.target.value);
    if (onChange) {
      onChange(value);
    }
  };

  const handleKeyDown = (e: React.KeyboardEvent<HTMLInputElement>) => {
    // Allow: backspace, delete, tab, escape, enter, decimal point, minus sign (for negative numbers)
    const allowedKeys = [
      "Backspace",
      "Delete",
      "Tab",
      "Escape",
      "Enter",
      ".",
      "-",
      "ArrowLeft",
      "ArrowRight",
      "ArrowUp",
      "ArrowDown",
      "Home",
      "End",
    ];

    // Allow Ctrl+A, Ctrl+C, Ctrl+V, Ctrl+X
    if (
      (e.ctrlKey && ["a", "c", "v", "x"].includes(e.key.toLowerCase())) ||
      allowedKeys.includes(e.key)
    ) {
      // Check if decimal point is already present when trying to add another
      if (e.key === "." && e.currentTarget.value.includes(".")) {
        e.preventDefault();
      }

      // Check if minus sign is already present or not at the beginning
      if (
        e.key === "-" &&
        (e.currentTarget.value.includes("-") ||
          e.currentTarget.selectionStart !== 0)
      ) {
        e.preventDefault();
      }

      return;
    }

    // Allow numbers
    if (/^\d$/.test(e.key)) {
      return;
    }

    // Block everything else
    e.preventDefault();
  };

  return (
    <div className={cn("space-y-2", className)}>
      {label && (
        <Label htmlFor={inputId} className="text-sm font-medium">
          {label}
        </Label>
      )}
      <Input
        id={inputId}
        type="number"
        min={min}
        max={max}
        step={step}
        onChange={handleChange}
        onKeyDown={handleKeyDown}
        className={cn(error && "border-red-500 focus-visible:ring-red-500")}
        aria-invalid={!!error}
        aria-describedby={
          error
            ? `${inputId}-error`
            : helperText
            ? `${inputId}-description`
            : undefined
        }
        {...props}
      />
      {helperText && !error && (
        <p
          id={`${inputId}-description`}
          className="text-sm text-muted-foreground"
        >
          {helperText}
        </p>
      )}
      {error && (
        <p id={`${inputId}-error`} className="text-sm text-red-500">
          {error}
        </p>
      )}
    </div>
  );
}
