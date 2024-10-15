import React from "react";
import { View, styled } from "tamagui";
import Badge from "./Badge";
import Card, { CardProps } from "./Card";
import { Typography } from "./Typography";
import CardFooter from "./CardFooter";
import { FormStatus } from "../services/form.parser";
import { useTranslation } from "react-i18next";

export const FormStateToTextMapper: Record<FormStatus, string> = {
  [FormStatus.NOT_STARTED]: "status.not_started",
  [FormStatus.IN_PROGRESS]: "status.in_progress",
  [FormStatus.COMPLETED]: "status.completed",
  [FormStatus.MARKED_AS_COMPLETED]: "status.marked_as_completed",
};

export interface Form {
  id?: string;
  name?: string;
  options?: string;
  numberOfQuestions?: number;
  numberOfCompletedQuestions?: number;
  status: FormStatus;
}

export interface FormCardProps extends CardProps {
  form: Form;
  onPress: () => void;
}

const FormCard = (props: FormCardProps): JSX.Element => {
  const { t } = useTranslation("common");
  const { form, onPress, ...rest } = props;

  const CardHeader = styled(View, {
    name: "CardHeader",
    justifyContent: "space-between",
    flexDirection: "row",
    alignItems: "center",
    marginBottom: "$xxs",
  });

  return (
    <Card width="100%" onPress={onPress} {...rest}>
      <CardHeader>
        <Typography preset="body1" color="$gray9" fontWeight="700" maxWidth="55%" numberOfLines={2}>
          {form.name}
        </Typography>

        <Badge status={form.status} maxWidth="45%" textStyle={{ textAlign: "center" }}>
          {t(`${FormStateToTextMapper[form.status]}`)}
        </Badge>
      </CardHeader>

      {form.options && (
        <Typography preset="body1" color="$gray6" marginBottom="$xxs" numberOfLines={1}>
          {form.options}
        </Typography>
      )}

      <CardFooter text={form.numberOfCompletedQuestions + "/" + form.numberOfQuestions} />
    </Card>
  );
};

export default FormCard;
