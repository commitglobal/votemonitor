import { withFieldGroup } from "@/hooks/form";
import * as z from "zod";

export const commonQuestionFieldsSchema = z
  .object({
    code: z.string().min(1, "Code is required"),
    text: z.string().min(1, "Text is required"),
    helptext: z.string().optional(),
  });

const defaultValues: z.infer<typeof commonQuestionFieldsSchema> = {
  text: "",
  helptext: "",
  code: "",
};

export const CommonQuestionFields = withFieldGroup({
  defaultValues,
  render: function Render({ group }) {
    return (
      <>
        <group.AppField name="code">
          {(field) => {
            return (
              <field.TextInput
                label="Code"
                id="code"
                type="text"
                description="The code of the question"
              />
            );
          }}
        </group.AppField>

        <group.AppField name="text">
          {(field) => {
            return (
              <field.TextInput
                label="Confirm Password"
                id="confirmPassword"
                type="password"
                description="Confirm your password."
              />
            );
          }}
        </group.AppField>

        <group.AppField name="helptext">
          {(field) => {
            return (
              <field.TextInput
                label="Helptext"
                id="helptext"
                type="text"
                description="The helptext of the question"
              />
            );
          }}
        </group.AppField>
      </>
    );
  },
});
