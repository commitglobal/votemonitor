import { XStack, YStack } from "tamagui";
import Card, { CardProps } from "./Card";
import { Typography } from "./Typography";
import { useTranslation } from "react-i18next";
import { PollingStationVisitVM } from "../common/models/polling-station.model";
import { Icon } from "./Icon";

interface PollingStationCardProps extends CardProps {
  visit: PollingStationVisitVM;
}

const PollingStationCard = (props: PollingStationCardProps) => {
  const { t } = useTranslation("manage_my_polling_stations");
  const { visit, onPress, ...rest } = props;

  return (
    <Card pressStyle={{ opacity: 1 }} paddingTop="$0" paddingRight="$0" {...rest}>
      <YStack gap="$md">
        {/* header */}
        {/* the '-' margin is used in order to keep the alignment while having a big enough press area for the icon */}
        <XStack justifyContent="space-between" alignItems="center" marginBottom={-16}>
          <Typography preset="body1" fontWeight="700">
            {t("ps_card.header", { value: visit.number })}
          </Typography>

          <YStack
            padding="$md"
            onPress={onPress}
            pressStyle={{ opacity: 0.5 }}
            justifyContent="center"
            alignItems="center"
          >
            <Icon icon="bin" color="transparent" />
          </YStack>
        </XStack>

        {visit.level1 && (
          <Typography>
            {t("ps_card.l1")}: <Typography fontWeight="500">{visit.level1}</Typography>{" "}
          </Typography>
        )}

        {visit.level2 && (
          <Typography>
            {t("ps_card.l2")}: <Typography fontWeight="500"> {visit.level2} </Typography>{" "}
          </Typography>
        )}

        {visit.level3 && (
          <Typography>
            {t("ps_card.l3")}: <Typography fontWeight="500">{visit.level3}</Typography>
          </Typography>
        )}

        <Typography>
          {t("ps_card.street")}: <Typography fontWeight="500">{visit.address}</Typography>
        </Typography>
        <Typography>
          {t("ps_card.ps_number")}: <Typography fontWeight="500">{visit.number}</Typography>
        </Typography>
      </YStack>
    </Card>
  );
};

export default PollingStationCard;
