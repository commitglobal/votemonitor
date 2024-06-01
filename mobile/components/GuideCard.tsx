import React from "react";
import Card, { CardProps } from "./Card";
import { Typography } from "./Typography";
import { Guide } from "../services/api/get-guides.api";
import { useTranslation } from "react-i18next";
import CardFooter from "./CardFooter";

interface GuideCardProps extends CardProps {
  guide: Guide;
}

const GuideCard = ({ guide, ...rest }: GuideCardProps) => {
  const { t } = useTranslation("guides");
  const createdOn = new Date(guide.createdOn);
  return (
    <Card gap="$xxs" {...rest}>
      <Typography preset="body1" fontWeight="700">
        {guide.title || t("list.guide.backup_title")}
      </Typography>
      {guide.guideType && (
        <Typography preset="body1" color="$gray6">
          {guide.guideType}
        </Typography>
      )}
      {createdOn && (
        <CardFooter
          text={`${t("list.guide.created_on")} ${createdOn.toLocaleDateString()}, ${createdOn.toLocaleTimeString([], { hour: "2-digit", minute: "2-digit" })}`}
        />
      )}
    </Card>
  );
};

export default GuideCard;
