import { withFieldGroup } from "@/hooks/form";
import * as z from "zod";

export const textQuestionFieldsSchema = z
  .object({
    inputPlaceholder: z.string().optional(),
  });

const defaultValues: z.infer<typeof textQuestionFieldsSchema> = {
  inputPlaceholder: "",
};

export const TextQuestionFields = withFieldGroup({
  defaultValues,
  render: function Render({ group }) {
    return (
      <>
        <group.AppField name="inputPlaceholder">
          {(field) => {
            return (
              <field.TextInput
                label="Input Placeholder"
                id="inputPlaceholder"
                type="text"
                description="The placeholder of the input field"
              />
            );
          }}
        </group.AppField>


      </>
    );
  },
});
