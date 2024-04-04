import React from "react";
import { StyleProp, ViewStyle, View, TextStyle } from "react-native";
import {
  Card as TamaguiCard,
  CardProps as TamaguiCardProps,
  useTheme,
} from "tamagui";
import { tokens } from "../theme/tokens";
// import { Badge, BadgePresets, BadgeProps } from "./Badge";
import { Card } from "./Card";
import { Typography } from "./Typography";
import { Icon } from "./Icon";

export interface FormCardProps {
  header: String;
  subHeader?: String;
  footer: String;
  // badgePreset?: BadgePresets;

  headerStyle?: StyleProp<TextStyle>;
  footerStyle?: StyleProp<TextStyle>;
  subHeaderStyle?: StyleProp<TextStyle>;
}

export function FormCard(props: FormCardProps) {
  const theme = useTheme();

  const {
    header,
    subHeader,
    footer,
    headerStyle,
    footerStyle,
    subHeaderStyle,
    // badgePreset,
  } = props;

  const hasSubHeader = subHeader ? subHeader.trim() !== "" : false;

  const defaultHeaderStyle = { color: theme.gray9?.val };
  const headerStyles = [defaultHeaderStyle, headerStyle];

  return (
    <Card style={{ width: "100%" }}>
      <View
        style={{
          flexDirection: "row",
          justifyContent: "space-between",
          marginBottom: 8,
        }}
      >
        <Typography preset="body2" style={headerStyles}>
          {header}
        </Typography>
        {/* <Badge preset={badgePreset}></Badge> */}
      </View>

      {hasSubHeader === true && (
        <Typography
          preset="body1"
          style={{ color: theme.gray11?.val, marginBottom: 8 }}
        >
          {subHeader}
        </Typography>
      )}

      <View style={{ flexDirection: "row", justifyContent: "space-between" }}>
        <Typography
          preset="default"
          style={{ fontWeight: "500", color: theme.gray14?.val }}
        >
          {footer}
        </Typography>
        <Icon icon="chevronRight" color={theme.purple7?.val}></Icon>
      </View>
    </Card>
  );
}
