import { withFieldGroup } from "@/hooks/form";
import * as z from "zod";

export const passwordsSchema = z
  .object({
    password: z.string().min(6, "Password should be at least 6 characters."),
    confirmPassword: z.string(),
  })
  .refine((data) => data.password === data.confirmPassword, {
    message: "Passwords do not match",
    path: ["confirmPassword"], // Optional: specifies where the error should appear
  });

const defaultValues: z.infer<typeof passwordsSchema> = {
  password: "",
  confirmPassword: "",
};

export const PasswordFields = withFieldGroup({
  defaultValues,
  render: function Render({ group }) {
    return (
      <>
        <group.AppField name="password">
          {(field) => {
            return (
              <field.TextInput
                label="Password"
                id="password"
                type="password"
                description="Your password (must be at least 8 characters long)"
              />
            );
          }}
        </group.AppField>

        <group.AppField name="confirmPassword">
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
      </>
    );
  },
});
