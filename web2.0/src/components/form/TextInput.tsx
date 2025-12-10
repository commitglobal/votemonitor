import {
  Field,
  FieldDescription,
  FieldError,
  FieldLabel,
} from "@/components/ui/field";
import { Input } from "../ui/input";
import { useFieldContext } from "@/hooks/form-context";

export default function TextInput({
  label,
  description,
  id,
  type = "text",
  required,
  className,
}: {
  label: string;
  id: string;
  description?: string;
  type?: string;
  required?: boolean;
  className?: string;
}) {
  const field = useFieldContext<string>();
  return (
    <Field className={className}>
      <FieldLabel htmlFor={id}>
        {label}
        {required && <span className="text-destructive ml-1">*</span>}
      </FieldLabel>
      <Input
        id={id}
        type={type}
        placeholder={label}
        required={required}
        value={field.state.value}
        onBlur={field.handleBlur}
        onChange={(e) => field.handleChange(e.target.value)}
      />
      {description && <FieldDescription>{description}</FieldDescription>}
      {!field.state.meta.isValid && (
        <FieldError>
          {field.state.meta.errors.map((error) => error?.message).join(", ")}
        </FieldError>
      )}
    </Field>
  );
}
