import { createContext, useContext, useMemo, useState } from "react";
import {
  electionRoundsKeys,
  pollingStationInformationQueryFn,
  pollingStationsKeys,
  useElectionRoundsQuery,
  usePollingStationById,
  usePollingStationsNomenclatorQuery,
  usePollingStationsVisits,
} from "../../services/queries.service";
import {
  PollingStationNomenclatorNodeVM,
  PollingStationVisitVM,
} from "../../common/models/polling-station.model";
import { ElectionRoundVM } from "../../common/models/election-round.model";
import {
  skipToken,
  useMutation,
  useQueries,
  useQuery,
  useQueryClient,
} from "@tanstack/react-query";
import LoadingScreen from "../../components/LoadingScreen";
import GenericErrorScreen from "../../components/GenericErrorScreen";
import { getElectionRoundAllForms } from "../../services/definitions.api";
import { formSubmissionsQueryFn } from "../../services/queries/form-submissions.query";
import { NotificationsKeys } from "../../services/queries/notifications.query";
import { getNotifications } from "../../services/api/notifications/notifications-get.api";
import { QuickReportKeys } from "../../services/queries/quick-reports.query";
import { getQuickReports } from "../../services/api/quick-report/get-quick-reports.api";
import { ASYNC_STORAGE_KEYS } from "../../common/constants";
import AsyncStorage from "@react-native-async-storage/async-storage";

type UserContextType = {
  electionRounds: ElectionRoundVM[] | undefined;
  visits: PollingStationVisitVM[] | undefined;

  activeElectionRound?: ElectionRoundVM;
  selectedPollingStation?: PollingStationNomenclatorNodeVM | null;

  isLoading: boolean;

  error: Error | null;
  setSelectedPollingStationId: (pollingStationId: string | null) => void;
  setSelectedElectionRoundId: (electionRoundId: string) => void;
};

export const UserContext = createContext<UserContextType | null>(null);

const useSelectedElectionRoundId = () => {
  return useQuery({
    queryKey: [ASYNC_STORAGE_KEYS.SELECTED_ELECTION_ROUND_ID],
    queryFn: () => AsyncStorage.getItem(ASYNC_STORAGE_KEYS.SELECTED_ELECTION_ROUND_ID),
    staleTime: 0,
    networkMode: "always",
  });
};

const useSetSelectedElectionRoundIdMutation = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationKey: ["SET_SELECTED_ELECTION_ROUND_ID"],
    mutationFn: (electionRoundId: string) => {
      console.log("Selected election round id", electionRoundId);
      return AsyncStorage.setItem(ASYNC_STORAGE_KEYS.SELECTED_ELECTION_ROUND_ID, electionRoundId);
    },
    onSettled: async () => {
      await queryClient.invalidateQueries({
        queryKey: [ASYNC_STORAGE_KEYS.SELECTED_ELECTION_ROUND_ID],
      });
      // await queryClient.invalidateQueries();
    },
    networkMode: "always",
  });
};

const UserContextProvider = ({ children }: React.PropsWithChildren) => {
  const [selectedPollingStationId, setSelectedPollingStationId] = useState<string | null>();

  const { data: selectedElectionRoundId, isLoading: isLoadingSelectedElectionRoundId } =
    useSelectedElectionRoundId();
  const { mutate: setSelectedElectionRoundId, isPending: isSettingSelectedElectionRoundId } =
    useSetSelectedElectionRoundIdMutation();

  const {
    data: rounds,
    isLoading: isLoadingRounds,
    error: ElectionRoundsError,
  } = useElectionRoundsQuery();

  const activeElectionRound = useMemo(() => {
    if (!rounds || isLoadingSelectedElectionRoundId) {
      return undefined;
    }

    let selectedElectionRound;
    if (selectedElectionRoundId) {
      // Look for the election round saved in AsyncStorage for the user
      selectedElectionRound = rounds?.find(
        (round) => round.id === selectedElectionRoundId && round.status === "Started",
      );
    }

    if (!selectedElectionRound) {
      // If no election round is saved in AsyncStorage for the user, return the first election round that is started
      AsyncStorage.removeItem(ASYNC_STORAGE_KEYS.SELECTED_ELECTION_ROUND_ID);
      return rounds?.find((round) => round.status === "Started");
    }

    return selectedElectionRound;
  }, [rounds, selectedElectionRoundId]);

  const {
    data: visits,
    isLoading: isLoadingVisits,
    error: PollingStationsError,
  } = usePollingStationsVisits(activeElectionRound?.id);

  const currentSelectedPollingStationId = useMemo(() => {
    return (
      selectedPollingStationId ||
      visits?.sort((a, b) => new Date(b.visitedAt).getTime() - new Date(a.visitedAt).getTime())[0]
        ?.pollingStationId
    );
  }, [visits, selectedPollingStationId]);

  const {
    data: nomenclatorData,
    isLoading: isLoadingNomenclature,
    error: NomenclatureError,
  } = usePollingStationsNomenclatorQuery(activeElectionRound?.id);

  const {
    data: lastVisitedPollingStation,
    // isLoading: isLoadingCurrentPS,
    error: PollingStationNomenclatorNodeDBError,
  } = usePollingStationById(currentSelectedPollingStationId, nomenclatorData);

  useQueries({
    queries: [
      ...(visits
        ?.map((visit) => {
          // const nodes = {
          //   queryKey: pollingStationsKeys.one(visit.pollingStationId),
          //   queryFn: () =>
          //     nomenclatorData ? pollingStationByIdQueryFn(visit.pollingStationId) : skipToken,
          //   staleTime: 5 * 60 * 1000,
          // };
          const informations = {
            queryKey: pollingStationsKeys.pollingStationInformation(
              activeElectionRound?.id,
              visit.pollingStationId,
            ),
            queryFn: () =>
              pollingStationInformationQueryFn(activeElectionRound?.id, visit.pollingStationId),
            staleTime: 5 * 60 * 1000,
          };
          const submissions = {
            queryKey: pollingStationsKeys.allFormSubmissions(
              activeElectionRound?.id,
              visit.pollingStationId,
            ),
            queryFn: () => formSubmissionsQueryFn(activeElectionRound?.id, visit.pollingStationId),
          };
          return [informations, submissions];
        })
        ?.flat() || []),
      {
        queryKey: electionRoundsKeys.forms(activeElectionRound?.id),
        queryFn: activeElectionRound?.id
          ? () => getElectionRoundAllForms(activeElectionRound.id)
          : skipToken,
      },
      {
        queryKey: NotificationsKeys.notifications(activeElectionRound?.id),
        queryFn: activeElectionRound?.id
          ? () => getNotifications({ electionRoundId: activeElectionRound.id })
          : skipToken,
      },
      {
        queryKey: QuickReportKeys.byElectionRound(activeElectionRound?.id),
        queryFn: activeElectionRound?.id
          ? () => getQuickReports(activeElectionRound.id)
          : skipToken,
      },
    ],
  });

  const error =
    ElectionRoundsError ||
    PollingStationsError ||
    NomenclatureError ||
    PollingStationNomenclatorNodeDBError;

  const isLoading =
    isLoadingRounds || isLoadingVisits || isLoadingNomenclature || isSettingSelectedElectionRoundId; // will be false while offline, because the queryFn is not running. isPending will be true

  const contextValues = useMemo(() => {
    return {
      error,
      isLoading,
      visits,
      electionRounds: rounds,
      activeElectionRound,
      selectedPollingStation: lastVisitedPollingStation,
      setSelectedPollingStationId,
      setSelectedElectionRoundId,
    };
  }, [error, isLoading, visits, rounds, activeElectionRound, lastVisitedPollingStation]);

  if (error) {
    console.log(error);
    return <GenericErrorScreen />;
  }

  // console.log("👀👀👀 isLoading", isLoadingRounds || isLoadingVisits || isLoadingNomenclature);
  // console.log("✅ isLoadingRounds", isLoadingRounds);
  // console.log("✅ isLoadingVisits", isLoadingVisits);
  // console.log("✅ isLoadingNomenclature", isLoadingNomenclature);
  // console.log("✅ isLoadingCurrentPS", isLoadingCurrentPS);
  // console.log("🚀🚀🚀🚀🚀🚀🚀🚀🚀🚀🚀🚀🚀🚀🚀🚀🚀🚀🚀🚀🚀🚀🚀🚀🚀");

  if (isLoading) {
    return <LoadingScreen />;
  } else {
    if (!rounds) {
      // No internet and no data available in cache (restoration already happeened here)
      return <GenericErrorScreen />;
    }
  }

  return <UserContext.Provider value={contextValues}>{children}</UserContext.Provider>;
};

export const useUserData = () => {
  const data = useContext(UserContext);

  if (!data) {
    throw new Error("No data in UserContext found. Was used outside the tree");
  }

  return data;
};

export default UserContextProvider;
