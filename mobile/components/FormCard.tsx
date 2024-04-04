import React from "react";
import { StyleProp, View, ViewStyle } from "react-native";
import { useTheme } from "tamagui";
import { tokens } from "../theme/tokens";
import { Badge, Presets } from "./Badge";
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

  const $headerContainerStyles: StyleProp<ViewStyle> = {
    flexDirection: "row",
    justifyContent: "space-between",
    alignItems: "center",
    marginBottom: tokens.space.xxs.val,
  };

  const $subHeaderStyles = {
    color: theme.gray14?.val,
    marginBottom: tokens.space.xxs.val,
  };

  return (
    <Card style={{ width: "100%" }}>
      <View style={$headerContainerStyles}>
        <Typography preset="body2" style={{ color: theme.gray9?.val }}>
          {header}
        </Typography>
        <Badge preset={badgePreset}></Badge>
      </View>

      {hasSubHeader === true && (
        <Typography preset="body1" style={$subHeaderStyles}>
          {subHeader}
        </Typography>
      )}

      {/* TODO: Footer will come as a separate component: Card footer component */}
      <View style={{ flexDirection: "row", justifyContent: "space-between" }}>
        <Typography
          preset="default"
          style={{ fontWeight: "400", color: theme.gray5?.val }}
        >
          {footer}
        </Typography>
        <Icon icon="chevronRight" color={theme.purple7?.val}></Icon>
      </View>
    </Card>
  );
};

export default FormCard;
