import { JSX } from "react";
import { View, styled } from "tamagui";
import Card, { CardProps } from "./Card";
import CardFooter from "./CardFooter";
import { Typography } from "./Typography";
import { useTranslation } from "react-i18next";

export interface Form {
  id?: string;
  name?: string;
  options?: string;
  numberOfSubmissions: number;
}

export interface MultiSubmissionFormCardProps extends CardProps {
  form: Form;
  onPress: () => void;
}

const MultiSubmissionFormCard = (props: MultiSubmissionFormCardProps): JSX.Element => {
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
        <Typography preset="body1" color="$gray9" fontWeight="700" maxWidth="55%" numberOfLines={2}>
          {form.name}
        </Typography>
      </CardHeader>

      {form.options && (
        <Typography preset="body1" color="$gray6" marginBottom="$xxs" numberOfLines={1}>
          {form.options}
        </Typography>
      )}

      <CardFooter text={t("overview.number_of_submissions", { value: form.numberOfSubmissions })} />
    </Card>
  );
};

export default MultiSubmissionFormCard;
