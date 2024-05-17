import { AlertDialog, View, XStack } from "tamagui";
import { Dialog } from "./Dialog";
import { Icon } from "./Icon";
import { Typography } from "./Typography";
import Button from "./Button";
import { useTranslation } from "react-i18next";

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

export default DeletePollingStationDialog;
