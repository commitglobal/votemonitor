import { XStack, YStack } from "tamagui";
import Card from "./Card";
import { Typography } from "./Typography";
import DeletePollingStationDialog from "./DeletePollingStationDialog";
import { useTranslation } from "react-i18next";
import { PollingStationVisitVM } from "../common/models/polling-station.model";

interface PollingStationCardProps {
  visit: PollingStationVisitVM;
}

const PollingStationCard = (props: PollingStationCardProps) => {
  const { t } = useTranslation("manage_polling_stations");
  const { visit } = props;

  return (
    <Card pressStyle={{ opacity: 1 }}>
      <YStack gap="$md">
        <XStack justifyContent="space-between" alignItems="center">
          <Typography preset="body1" fontWeight="700">
            {t("station_card.title", { value: visit.number })}
          </Typography>

          <DeletePollingStationDialog
            pollingStationNumber={visit.number}
            pollingStationId={visit.pollingStationId}
          />
        </XStack>

        {visit.level1 !== "" && (
          <Typography>
            {t("station_card.level_1")} <Typography fontWeight="500">{visit.level1}</Typography>{" "}
          </Typography>
        )}

        {visit.level2 !== "" && (
          <Typography>
            {t("station_card.level_2")} <Typography fontWeight="500"> {visit.level2} </Typography>{" "}
          </Typography>
        )}

        {visit.level3 !== "" && (
          <Typography>
            {t("station_card.level_3")} <Typography fontWeight="500">{visit.level3}</Typography>
          </Typography>
        )}

        <Typography>
          {t("station_card.street")} <Typography fontWeight="500">{visit.address}</Typography>
        </Typography>
        <Typography>
          {t("station_card.number")} <Typography fontWeight="500">{visit.number}</Typography>
        </Typography>
      </YStack>
    </Card>
  );
};

export default PollingStationCard;
