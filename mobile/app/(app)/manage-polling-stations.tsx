import { Screen } from "../../components/Screen";
import Header from "../../components/Header";
import { Icon } from "../../components/Icon";
import { YStack } from "tamagui";
import { Typography } from "../../components/Typography";
import { router } from "expo-router";
import { useUserData } from "../../contexts/user/UserContext.provider";
import { useTranslation } from "react-i18next";
import PollingStationCard from "../../components/PollingStationCard";
import { useMemo, useState } from "react";
import { PollingStationVisitVM } from "../../common/models/polling-station.model";
import WarningDialog from "../../components/WarningDialog";
import { useSafeAreaInsets } from "react-native-safe-area-context";
import { useWindowDimensions } from "react-native";
import { ListView } from "../../components/ListView";
import { useDeletePollingStationMutation } from "../../services/mutations/delete-polling-station.mutation";
import { useQueryClient } from "@tanstack/react-query";
import { pollingStationsKeys } from "../../services/queries.service";
import Toast from "react-native-toast-message";

const ManagePollingStation = () => {
  const { t } = useTranslation("manage_my_polling_stations");
  const { visits, activeElectionRound } = useUserData();
  const queryClient = useQueryClient();
  const getPollingStationsQK = useMemo(
    () => pollingStationsKeys.visits(activeElectionRound?.id),
    [activeElectionRound?.id],
  );
  const { mutate: deletePS, isPending: isPendingDeletePS } = useDeletePollingStationMutation(
    activeElectionRound?.id,
  );
  const [selectedPS, setSelectedPS] = useState<PollingStationVisitVM | null>(null);
  const [removalAllowed, setRemovalAllowed] = useState(true);
  console.log("🍇🍇🍇🍇 ps", selectedPS);
  console.log("🍇 elect id", activeElectionRound?.id);

  const insets = useSafeAreaInsets();
  const { height } = useWindowDimensions();
  const scrollHeight = height - insets.top - insets.bottom - 50;

  if (visits === undefined || visits.length === 0) {
    return <Typography>No visits</Typography>;
  }

  const onDelete = () => {
    if (activeElectionRound?.id) {
      deletePS(
        {
          // we enforce selectedPS because this function is called from the confirmation dialog, which is open only when selectedPS
          pollingStationId: selectedPS!.pollingStationId,
          electionRoundId: activeElectionRound?.id,
        },
        {
          // if the PS has any information added to it and cannot be deleted -> display removal unallowed content in dialog
          onError: (err) => {
            console.log("", err);
            // close dialog
            setSelectedPS(null);
            // fake successful deletion
            return Toast.show({
              type: "success",
              text2: t("delete_success"),
              visibilityTime: 3000,
              text2Style: { textAlign: "center" },
            });

            return setRemovalAllowed(false);
          },
          // if deletion complete -> close dialog and show success toast
          onSuccess: () => {
            setSelectedPS(null);
            // show toast
            Toast.show({
              type: "success",
              text2: t("delete_success"),
              visibilityTime: 3000,
              text2Style: { textAlign: "center" },
            });
          },
          onSettled: () => {
            // if the ps is only local, just refetch the visits and it will disappear
            queryClient.invalidateQueries({
              queryKey: getPollingStationsQK,
            });
          },
        },
      );
    }
  };

  return (
    <Screen
      preset="scroll"
      ScrollViewProps={{
        showsVerticalScrollIndicator: false,
        stickyHeaderIndices: [0],
        bounces: false,
      }}
    >
      <Header
        title={t("general_text")}
        titleColor="white"
        barStyle="light-content"
        leftIcon={<Icon icon="chevronLeft" color="white" />}
        onLeftPress={() => router.back()}
      />

      <YStack
        height={scrollHeight}
        paddingHorizontal="$md"
        paddingVertical="$lg"
        // backgroundColor={"pink"}
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
          estimatedItemSize={225}
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
          action={onDelete}
          onCancel={() => {
            setSelectedPS(null);
            setRemovalAllowed(true);
          }}
          actionBtnText={
            !isPendingDeletePS
              ? removalAllowed
                ? t("warning_dialog.actions.remove")
                : ""
              : t("warning_dialog.actions.loading")
          }
          cancelBtnText={t("warning_dialog.actions.cancel")}
        />
      )}
    </Screen>
  );
};

export default ManagePollingStation;
