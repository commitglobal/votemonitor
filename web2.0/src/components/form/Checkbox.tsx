import { useFieldContext } from "@/hooks/form-context";
import { Field, FieldContent, FieldError, FieldLabel } from "../ui/field";

import { Checkbox } from "../ui/checkbox";

export default function CustomCheckbox({
  label,
  id,
}: {
  label: string;
  id: string;
}) {
  const field = useFieldContext<boolean>();
  return (
    <Field>
      <div className="flex items-center gap-2">
        <Checkbox
          id={id}
          checked={field.state.value}
          onCheckedChange={(checked) => field.handleChange(checked === true)}
          onBlur={field.handleBlur}
        />
        <FieldContent>
          <FieldLabel htmlFor={id}>{label}</FieldLabel>
        </FieldContent>
      </div>
      {!field.state.meta.isValid && (
        <FieldError>
          {field.state.meta.errors.map((error) => error?.message).join(", ")}
        </FieldError>
      )}
    </Field>
  );
}
