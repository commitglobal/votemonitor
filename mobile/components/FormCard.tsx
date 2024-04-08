import React from "react";
import { useState } from "react";
import { View, styled } from "tamagui";
import Badge, { BadgeProps } from "./Badge";
import Card from "./Card";
import { Typography } from "./Typography";
import CardFooter from "./CardFooter";

export interface FormCardProps {
  /**
   * Header text
   */
  header: string;

  /**
   * Subheader optional text
   */
  subHeader?: string;

  /**
   * Footer text
   */
  footer: string;

  /**
   * Optional preset type.
   */
  badgeProps: BadgeProps;

  /**
   * Performed action for onPress
   */
  action: () => void;
}

const FormCard = (props: FormCardProps): JSX.Element => {
  const { header, subHeader, footer, badgeProps, action } = props;
  const hasSubHeader = subHeader ? subHeader.trim() !== "" : false;

  const badgePreset = badgeProps.preset || "default";
  const badgeChildren = badgeProps.children;

  const CardHeader = styled(View, {
    name: "CardHeader",
    justifyContent: "space-between",
    flexDirection: "row",
    alignItems: "center",
    marginBottom: 8,
  });

  const [isPressed, setIsPressed] = useState(false);

  return (
    <Card
      style={{ width: "100%" }}
      onPress={action}
      onPressIn={() => setIsPressed(true)}
      onPressOut={() => setIsPressed(false)}
      opacity={isPressed ? 0.5 : 1}
    >
      <CardHeader>
        <Typography preset="body1" color="$gray9" style={{ fontWeight: "700" }}>
          {header}
        </Typography>

        <Badge preset={badgePreset}>{badgeChildren}</Badge>
      </CardHeader>

      {hasSubHeader && (
        <Typography preset="body1" color="$gray6" style={{ marginBottom: 8 }}>
          {subHeader}
        </Typography>
      )}

      <CardFooter text={footer} action={() => {}} />
    </Card>
  );
};

export default FormCard;
