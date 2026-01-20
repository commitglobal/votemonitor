import { withFieldGroup } from "@/hooks/form";
import { isNullOrEmpty } from "@/lib/strings";
import { DisplayLogicCondition } from "@/types/form";
import * as z from "zod";

export const displayLogicSchema = z
  .object({
    hasDisplayLogic: z.boolean().catch(false),
    parentQuestionId: z.string().optional(),
    condition: z.enum(DisplayLogicCondition).optional().catch(DisplayLogicCondition.Equals),
    value: z.string().optional(),
  })
  .refine((data) => {
    if (!data.hasDisplayLogic) return true;
    return !isNullOrEmpty(data.parentQuestionId);
  }, {
    message: "Parent question ID is required",
    path: ["parentQuestionId"],
  })
  .refine((data) => {
    if (!data.hasDisplayLogic) return true;
    return !isNullOrEmpty(data.condition);
  }, {
    message: "Condition is required",
    path: ["condition"],
  }).refine((data) => {
    if (!data.hasDisplayLogic) return true;
    return !isNullOrEmpty(data.value);
  }, {
    message: "Value is required",
    path: ["value"],
  });

const defaultValues: z.infer<typeof displayLogicSchema> = {
  hasDisplayLogic: false,
  parentQuestionId: "",
  condition: DisplayLogicCondition.Equals,
  value: "",
};

export const DisplayLogicFields = withFieldGroup({
  defaultValues,
  render: function Render({ group }) {
    return (
      <>
        <group.AppField name="hasDisplayLogic">
          {(field) => {
            return (
              <field.Toggle
                label="Has Display Logic"
                id="hasDisplayLogic"
                description="Whether the question should be displayed based on the parent question."
              />
            );
          }}
        </group.AppField>

        <group.AppField name="parentQuestionId">
          {(field) => {
            return (
              <field.TextInput
                label="Parent Question ID"
                id="confirmPassword"
                type="password"
                description="Confirm your password."
              />
            );
          }}
        </group.AppField>
      </>
    );
  },
});
