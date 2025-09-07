import React from "react";
import { View, styled } from "tamagui";
import Badge from "./Badge";
import Card, { CardProps } from "./Card";
import { Typography } from "./Typography";
import CardFooter from "./CardFooter";
import { SubmissionStatus } from "../services/form.parser";
import { useTranslation } from "react-i18next";
import { SubmissionStateToTextMapper } from "../common/utils/form-submissions";

export interface Form {
  id?: string;
  name?: string;
  options?: string;
  numberOfQuestions?: number;
  numberOfCompletedQuestions?: number;
  status: SubmissionStatus;
}

export interface SingleSubmissionFormCardProps extends CardProps {
  form: Form;
  onPress: () => void;
}

const SingleSubmissionFormCard = (props: SingleSubmissionFormCardProps): JSX.Element => {
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
          {t(`${SubmissionStateToTextMapper[form.status]}`)}
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

export default SingleSubmissionFormCard;
