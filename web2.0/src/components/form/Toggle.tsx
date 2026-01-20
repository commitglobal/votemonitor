import {
  Field,
  FieldContent,
  FieldDescription,
  FieldError,
  FieldLabel,
} from "@/components/ui/field";
import { useFieldContext } from "@/hooks/form-context";
import { Switch } from "../ui/switch";

export default function Toggle({
  label,
  id,
  className,
  description,
  onChange,
}: {
  label: string;
  description?: string;
  id: string;
  className?: string;
  onChange?: (value: boolean) => void;
}) {
  const field = useFieldContext<boolean>();
  return (
    <Field className={className}>
      <div className="flex items-center gap-2">
        <Switch
          id={id}
          checked={field.state.value}
          onCheckedChange={(checked) => {
            field.handleChange(checked === true)
            onChange?.(checked === true)
          }}
          onBlur={field.handleBlur}
        />
        <FieldContent>
          <FieldLabel htmlFor={id}>{label}</FieldLabel>
        </FieldContent>
      </div>
      {description && <FieldDescription>{description}</FieldDescription>}
      {!field.state.meta.isValid && (
        <FieldError>
          {field.state.meta.errors.map((error) => error?.message).join(", ")}
        </FieldError>
      )}
    </Field>
  );
}
