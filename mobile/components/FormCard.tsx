import React, { useState } from "react";
import { View, styled } from "tamagui";
import Badge from "./Badge";
import Card from "./Card";
import { Typography } from "./Typography";
import CardFooter from "./CardFooter";

export interface Form {
  id?: string;
  name?: string;
  options?: string;
  numberOfQuestions?: string;
  numberOfCompletedQuestions?: string;
  status: string;
}

export interface FormCardProps {
  form: Form;

  /**
   * Performed action for onPress
   */
  action: () => void;
}

const FormCard = (props: FormCardProps): JSX.Element => {
  const { form, action } = props;
  const presetType =
    form.status === "completed" ? "success" : form.status === "in progress" ? "warning" : "default";

  const CardHeader = styled(View, {
    name: "CardHeader",
    justifyContent: "space-between",
    flexDirection: "row",
    alignItems: "center",
    marginBottom: "$xxs",
  });

  const [isPressed, setIsPressed] = useState(false);

  return (
    <Card
      width="100%"
      onPress={action}
      onPressIn={() => setIsPressed(true)}
      onPressOut={() => setIsPressed(false)}
      opacity={isPressed ? 0.5 : 1}
    >
      <CardHeader>
        <Typography preset="body1" color="$gray9" fontWeight="700">
          {form.name}
        </Typography>

        <Badge preset={presetType}>{form.status}</Badge>
      </CardHeader>

      {form.options && (
        <Typography preset="body1" color="$gray6" marginBottom="$xxs">
          {form.options}
        </Typography>
      )}

      <CardFooter text={form.numberOfCompletedQuestions + "/" + form.numberOfQuestions} />
    </Card>
  );
};

export default FormCard;
