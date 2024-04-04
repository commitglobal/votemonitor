import React from "react";
import { useTheme, View, styled } from "tamagui";
import { Presets } from "./Badge";
import Badge from "./Badge";
import Card from "./Card";
import { Typography } from "./Typography";
import { Icon } from "./Icon";

export interface FormCardProps {
  /**
   * Header text
   */
  header: String;

  /**
   * Subheader optional text
   */
  subHeader?: String;

  /**
   * Footer text
   */
  footer: String;

  /**
   * Optional preset type.
   * The default is 'Not started'
   */
  badgePreset?: Presets;
}

const FormCard = (props: FormCardProps): JSX.Element => {
  const theme = useTheme();

  const { header, subHeader, footer, badgePreset } = props;
  const hasSubHeader = subHeader ? subHeader.trim() !== "" : false;

  return (
    <Card style={{ width: "100%" }}>
      <HeaderContainer>
        <Typography preset="body1" color="$gray9" style={{ fontWeight: "700" }}>
          {header}
        </Typography>
        <Badge preset={badgePreset}></Badge>
      </HeaderContainer>

      {hasSubHeader === true && (
        <Typography preset="body1" color="$gray6" style={{ marginBottom: 8 }}>
          {subHeader}
        </Typography>
      )}

      {/* TODO: Footer will come as a separate component: Card footer component */}
      <View style={{ flexDirection: "row", justifyContent: "space-between" }}>
        <Typography preset="default" style={{ fontWeight: "400", color: theme.gray5?.val }}>
          {footer}
        </Typography>
        <Icon icon="chevronRight" color={theme.purple7?.val}></Icon>
      </View>
    </Card>
  );
};

const HeaderContainer = styled(View, {
  name: "HeaderContainer",
  justifyContent: "space-between",
  flexDirection: "row",
  alignItems: "center",
  marginBottom: 8,
});

export default FormCard;
