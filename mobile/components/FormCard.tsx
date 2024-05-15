import React from "react";
import { View, styled } from "tamagui";
import Badge from "./Badge";
import Card, { CardProps } from "./Card";
import { Typography } from "./Typography";
import CardFooter from "./CardFooter";
import { FormStatus } from "../services/form.parser";
import { useTranslation } from "react-i18next";

export const FormStateToTextMapper = (t: (key: string) => string): Record<FormStatus, string> => ({
  [FormStatus.NOT_STARTED]: t("form_overview.not_started"),
  [FormStatus.IN_PROGRESS]: t("form_overview.in_progress"),
  [FormStatus.COMPLETED]: t("form_overview.completed"),
});

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
  const { t } = useTranslation("form_overview");
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
        <Typography preset="body1" color="$gray9" fontWeight="700" maxWidth="65%" numberOfLines={2}>
          {form.name}
        </Typography>

        <Badge status={form.status}>{FormStateToTextMapper(t)[form.status]}</Badge>
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
