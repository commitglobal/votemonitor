import { Screen } from "../../../../../components/Screen";
import Header from "../../../../../components/Header";
import { Icon } from "../../../../../components/Icon";
import { YStack, XStack, View, AlertDialog } from "tamagui";
import { Typography } from "../../../../../components/Typography";
import { router } from "expo-router";
import Card from "../../../../../components/Card";
import { useUserData } from "../../../../../contexts/user/UserContext.provider";
import { Dialog } from "../../../../../components/Dialog";
import Button from "../../../../../components/Button";
import { PollingStationVisitVM } from "../../../../../common/models/polling-station.model";
import { useTranslation } from "react-i18next";

const ManagePollingStation = () => {
  const { t } = useTranslation("manage_polling_stations");
  const { visits } = useUserData();
  if (visits === undefined || visits.length === 0) {
    return <Typography>No visits</Typography>;
  }

  return (
    <Screen
      preset="scroll"
      ScrollViewProps={{
        showsVerticalScrollIndicator: false,
        stickyHeaderIndices: [0],
        bounces: false,
      }}
      backgroundColor="white"
    >
      <Header
        title={t("header.title")}
        titleColor="white"
        barStyle="light-content"
        leftIcon={<Icon icon="chevronLeft" color="white" />}
        onLeftPress={() => router.back()}
      />

      <YStack gap="$lg" paddingTop="$lg" paddingHorizontal="$md">
        {visits.map((visit) => (
          <PollingStationCard visit={visit} key={visit.pollingStationId} />
        ))}
      </YStack>
    </Screen>
  );
};

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
            onDelete={() => {
              console.log("TODO: API call for delete polling station!");
            }}
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

interface DeletePollingStationDialogProps {
  pollingStationNumber: string;
  onDelete: () => void;
}

const DeletePollingStationDialog = (props: DeletePollingStationDialogProps) => {
  const { t } = useTranslation("modals");
  const { onDelete, pollingStationNumber } = props;

  return (
    <Dialog
      trigger={<Icon icon="bin" color="white" />}
      header={
        <Typography preset="heading">
          {t("delete_station.title", { value: pollingStationNumber })}
        </Typography>
      }
      content={
        <View>
          <Typography preset="body1" color="$gray8">
            {t("delete_station.paragraph_1")}
            {"\n"}
          </Typography>
          <Typography preset="body1" color="$gray8">
            {t("delete_station.paragraph_2")}
          </Typography>
        </View>
      }
      footer={
        <XStack gap="$sm" justifyContent="center">
          <AlertDialog.Cancel asChild>
            <Button preset="chromeless" textStyle={{ color: "black" }}>
              {t("delete_station.actions.cancel")}
            </Button>
          </AlertDialog.Cancel>
          <AlertDialog.Cancel asChild>
            <Button preset="red" onPress={onDelete} flex={1}>
              {t("delete_station.actions.delete")}
            </Button>
          </AlertDialog.Cancel>
        </XStack>
      }
    />
  );
};

export default ManagePollingStation;
