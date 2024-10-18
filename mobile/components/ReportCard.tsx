import React, { useMemo } from "react";
import { useTranslation } from "react-i18next";
import { YStack } from "tamagui";
import { localizeIncidentCategory } from "../helpers/translationHelper";
import { IncidentCategory } from "../services/api/quick-report/post-quick-report.api";
import Card, { CardProps } from "./Card";
import CardFooter from "./CardFooter";
import { Typography } from "./Typography";
export interface ReportCardProps extends CardProps {
  title: string;
  numberOfAttachments: number;
  description: string;
  incidentCategory: IncidentCategory;
  onPress?: () => void;
}

const ReportCard = ({
  title,
  incidentCategory,
  description,
  numberOfAttachments,
  onPress,
}: ReportCardProps): JSX.Element => {
  const { t } = useTranslation("quick_report");

  const incidentCategoryString = useMemo(() => {
    return localizeIncidentCategory(incidentCategory);
  }, [incidentCategory]);

  return (
    <Card onPress={onPress} marginTop="$xxs">
      <YStack gap="$md">
        <Typography preset="body2">{title}</Typography>
        <Typography >{incidentCategoryString}</Typography>
        <Typography>{description}</Typography>
        <CardFooter
          text={
            numberOfAttachments === 0
              ? t("list.no_files")
              : numberOfAttachments === 1
                ? t("list.attachment_one", { count: numberOfAttachments })
                : t("list.attachment_other", { count: numberOfAttachments })
          }
        />
      </YStack>
    </Card>
  );
};

export default ReportCard;
