import { Screen } from "../../components/Screen";
import Header from "../../components/Header";
import { Icon } from "../../components/Icon";
import { YStack } from "tamagui";
import { router } from "expo-router";
import { useUserData } from "../../contexts/user/UserContext.provider";
import { useTranslation } from "react-i18next";
import PollingStationCard from "../../components/PollingStationCard";
import { useMemo, useState } from "react";
import { PollingStationVisitVM } from "../../common/models/polling-station.model";
import WarningDialog from "../../components/WarningDialog";
import { useWindowDimensions } from "react-native";
import { ListView } from "../../components/ListView";
import { useDeletePollingStationVisitMutation } from "../../services/mutations/delete-polling-station.mutation";
import { usePSHasFormSubmissions } from "../../services/queries/form-submissions.query";
import { pollingStationsKeys } from "../../services/queries.service";
import { useQueryClient } from "@tanstack/react-query";
import Toast from "react-native-toast-message";
import NoVisitsMPS from "../../components/NoVisitsMPS";
import { useSafeAreaInsets } from "react-native-safe-area-context";
import { useNetInfoContext } from "../../contexts/net-info-banner/NetInfoContext";

const ESTIMATED_ITEM_SIZE = 225;

const ManagePollingStation = () => {
  const { t } = useTranslation("manage_my_polling_stations");
  const [selectedPS, setSelectedPS] = useState<PollingStationVisitVM | null>(null);
  const [removalAllowed, setRemovalAllowed] = useState(true);

  const queryClient = useQueryClient();
  const { visits, activeElectionRound } = useUserData();

  // get visits query key
  const getPollingStationsQK = useMemo(
    () => pollingStationsKeys.visits(activeElectionRound?.id),
    [activeElectionRound?.id],
  );

  // mutations:
  // check if a ps has form submissions
  // delete a ps visit
  const { mutate: checkPSHasFormSubmissions, isPending: isPendingPSHasFormSubmissions } =
    usePSHasFormSubmissions(activeElectionRound?.id, selectedPS?.pollingStationId);
  const { mutate: deletePSVisit, isPending: isPendingDeletePSVisit } =
    useDeletePollingStationVisitMutation(activeElectionRound?.id);

  const insets = useSafeAreaInsets();
  const { shouldDisplayBanner } = useNetInfoContext();
  const { width } = useWindowDimensions();

  if (visits === undefined || visits.length === 0) {
    return <NoVisitsMPS />;
  }

  const onConfirmDelete = () => {
    if (!selectedPS?.pollingStationId || !activeElectionRound?.id) {
      return;
    }

    // check if ps has any form submissions
    checkPSHasFormSubmissions(
      { pollingStationId: selectedPS.pollingStationId, electionRoundId: activeElectionRound.id },
      {
        // if it has form submissions -> show removal not allowed modal
        onSuccess: () => {
          setRemovalAllowed(false);
        },
        // if it doesn't have form submissions -> delete the ps visit
        onError: () => {
          deletePSVisit(
            {
              electionRoundId: activeElectionRound.id,
              pollingStationId: selectedPS.pollingStationId,
            },
            {
              onSuccess: () => {
                // after deletion -> get new visits (without the deleted one) to display in the list
                queryClient.invalidateQueries({ queryKey: getPollingStationsQK });
                // close dialog
                setSelectedPS(null);
                // show success toast
                Toast.show({
                  type: "success",
                  text2: t("delete_success"),
                });
              },
              onError: () => {
                // close dialog
                setSelectedPS(null);
                // show success toast
                Toast.show({
                  type: "error",
                  text2: t("delete_error"),
                });
              },
            },
          );
        },
      },
    );
  };

  return (
    <Screen preset="fixed" contentContainerStyle={{ flexGrow: 1 }}>
      <Header
        title={t("general_text")}
        titleColor="white"
        barStyle="light-content"
        leftIcon={<Icon icon="chevronLeft" color="white" />}
        onLeftPress={() => router.back()}
      />

      <YStack
        flex={1}
        paddingHorizontal="$md"
        paddingTop="$lg"
        paddingBottom={shouldDisplayBanner ? 16 : insets.bottom + 16}
      >
        <ListView<PollingStationVisitVM>
          data={visits}
          showsVerticalScrollIndicator={false}
          bounces={false}
          renderItem={({ item, index }) => {
            return (
              <PollingStationCard
                key={item.pollingStationId + index}
                visit={item}
                marginBottom="$md"
                onPress={() => setSelectedPS(item)}
              />
            );
          }}
          estimatedListSize={{ height: ESTIMATED_ITEM_SIZE * 5, width: width - 32 }}
          estimatedItemSize={ESTIMATED_ITEM_SIZE}
        />
      </YStack>
      {selectedPS && (
        <WarningDialog
          title={
            removalAllowed
              ? t("warning_dialog.title", { value: selectedPS.number })
              : t("removal_unallowed_dialog.title", { value: selectedPS.number })
          }
          description={
            removalAllowed
              ? t("warning_dialog.description")
              : t("removal_unallowed_dialog.description")
          }
          action={onConfirmDelete}
          onCancel={() => {
            setSelectedPS(null);
            setRemovalAllowed(true);
          }}
          actionBtnText={
            isPendingPSHasFormSubmissions || isPendingDeletePSVisit
              ? t("warning_dialog.actions.loading")
              : removalAllowed
                ? t("warning_dialog.actions.remove")
                : ""
          }
          cancelBtnText={t("warning_dialog.actions.cancel")}
        />
      )}
    </Screen>
  );
};

export default ManagePollingStation;
