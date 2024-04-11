import React, { useState } from "react";
import { View, styled } from "tamagui";
import Badge, { BadgeProps } from "./Badge";
import Card from "./Card";
import { Typography } from "./Typography";
import CardFooter from "./CardFooter";

enum FormProgress {
  NOT_STARTED = "Not started",
  IN_PROGRESS = "In progress",
  COMPLETED = "Completed",
}

export interface FormCardProps {
  /**
   * Header text
   */
  headerText: string;

  /**
   * Subheader optional text
   */
  subHeaderText?: string;

  /**
   * Footer text
   */
  footerText: string;

  /**
   * Optional preset type.
   */
  badgeProps?: BadgeProps;

  /**
   * Performed action for onPress
   */
  action: () => void;
}

const FormCard = (props: FormCardProps): JSX.Element => {
  const { headerText, subHeaderText, footerText, badgeProps, action } = props;
  const hasSubHeader = subHeaderText ? subHeaderText.trim() !== "" : false;

  const badgePreset = badgeProps ? badgeProps.preset : "default";
  const badgeChildren = badgeProps ? badgeProps.children : "Not started";

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
          {headerText}
        </Typography>

        <Badge preset={badgePreset}>{badgeChildren}</Badge>
      </CardHeader>

      {hasSubHeader && (
        <Typography preset="body1" color="$gray6" marginBottom="$xxs">
          {subHeaderText}
        </Typography>
      )}

      <CardFooter text={footerText} />
    </Card>
  );
};

export default FormCard;
