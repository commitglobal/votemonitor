import { AlertDialog, View, XStack } from "tamagui";
import { Dialog } from "./Dialog";
import { Icon } from "./Icon";
import { Typography } from "./Typography";
import Button from "./Button";
import { useTranslation } from "react-i18next";
import { useUserData } from "../contexts/user/UserContext.provider";
import { useDeletePollingStationMutation } from "../services/mutations/delete-polling-station.mutation";
import { DeletePollingStationPayload } from "../services/definitions.api";

interface DeletePollingStationDialogProps {
  pollingStationNumber: string;
  pollingStationId: string;
}

const DeletePollingStationDialog = (props: DeletePollingStationDialogProps) => {
  const { t } = useTranslation("modals");
  const { pollingStationNumber, pollingStationId } = props;
  const { activeElectionRound, visits } = useUserData();

  if (!activeElectionRound) {
    return <Typography> Problem with electionRoundId! </Typography>;
  }

  // React Query Mutation
  const { mutate: deletePS, isSuccess, isError } = useDeletePollingStationMutation();
  const payload: DeletePollingStationPayload = {
    electionRoundId: activeElectionRound.id,
    pollingStationId: pollingStationId,
  };
  console.log("Payload: ", payload);
  console.log("isSuccess:", isSuccess);
  console.log("\n");

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
            <Button
              preset="red"
              flex={1}
              onPress={() => {
                deletePS(payload);
              }}
            >
              {t("delete_station.actions.delete")}
            </Button>
          </AlertDialog.Cancel>
        </XStack>
      }
    />
  );
};

export default DeletePollingStationDialog;
