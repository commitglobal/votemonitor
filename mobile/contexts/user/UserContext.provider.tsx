import { createContext, useContext, useMemo, useState } from "react";
import {
  pollingStationByIdQueryFn,
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
import { useQueries, useQueryClient } from "@tanstack/react-query";
import LoadingScreen from "../../components/LoadingScreen";
import NoElectionRounds from "../../components/NoElectionRounds";
import GenericErrorScreen from "../../components/GenericErrorScreen";

type UserContextType = {
  electionRounds: ElectionRoundVM[] | undefined;
  visits: PollingStationVisitVM[] | undefined;

  activeElectionRound?: ElectionRoundVM;
  selectedPollingStation?: PollingStationNomenclatorNodeVM | null;

  isLoading: boolean;

  error: Error | null;
  setSelectedPollingStationId: (pollingStationId: string) => void;
};

export const UserContext = createContext<UserContextType | null>(null);

const UserContextProvider = ({ children }: React.PropsWithChildren) => {
  const queryClient = useQueryClient();

  const [selectedPollingStationId, setSelectedPollingStationId] = useState<string>();

  const {
    data: rounds,
    isLoading: isLoadingRounds,
    error: ElectionRoundsError,
  } = useElectionRoundsQuery();

  const activeElectionRound = useMemo(
    () => rounds?.find((round) => round.status === "Started"),
    [rounds],
  );

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

  const { isLoading: isLoadingNomenclature, error: NomenclatureError } =
    usePollingStationsNomenclatorQuery(activeElectionRound?.id);

  const {
    data: lastVisitedPollingStation,
    isLoading: isLoadingCurrentPS,
    error: PollingStationNomenclatorNodeDBError,
  } = usePollingStationById(currentSelectedPollingStationId);

  useQueries(
    {
      queries:
        visits
          ?.map((visit) => {
            const nodes = {
              queryKey: pollingStationsKeys.one(visit.pollingStationId),
              queryFn: () => pollingStationByIdQueryFn(visit.pollingStationId),
              staleTime: 5 * 60 * 1000,
            };
            const informations = {
              queryKey: pollingStationsKeys.pollingStationInformation(
                activeElectionRound?.id,
                visit.pollingStationId,
              ),
              queryFn: () =>
                pollingStationInformationQueryFn(activeElectionRound?.id, visit.pollingStationId),
              staleTime: 5 * 60 * 1000,
            };
            return [nodes, informations];
          })
          ?.flat() || [],
    },
    queryClient,
  );

  const error =
    ElectionRoundsError ||
    PollingStationsError ||
    NomenclatureError ||
    PollingStationNomenclatorNodeDBError;

  const isLoading =
    isLoadingRounds || isLoadingVisits || isLoadingNomenclature || isLoadingCurrentPS; // will be false while offline, because the queryFn is not running. isPending will be true

  const contextValues = useMemo(() => {
    return {
      error,
      isLoading,
      visits,
      electionRounds: rounds,
      activeElectionRound,
      selectedPollingStation: lastVisitedPollingStation,
      setSelectedPollingStationId,
    };
  }, [error, isLoading, visits, rounds, activeElectionRound, lastVisitedPollingStation]);

  if (error) {
    console.log(error);
    return <GenericErrorScreen />;
  }

  // console.log("👀👀👀 isLoading", isLoadingRounds || isLoadingVisits || isLoadingNomenclature);
  // console.log("✅ isLoadingRounds", isLoadingRounds);
  // console.log("✅ isLoadingVisits", isLoadingNomenclature);
  // console.log("✅ isLoadingNomenclature", isLoadingNomenclature);
  // console.log("✅ isLoadingCurrentPS", isLoadingCurrentPS);
  // console.log("🚀🚀🚀🚀🚀🚀🚀🚀🚀🚀🚀🚀🚀🚀🚀🚀🚀🚀🚀🚀🚀🚀🚀🚀🚀");

  if (isLoading) {
    return <LoadingScreen />;
  } else {
    if (rounds && !rounds.length) {
      return <NoElectionRounds />;
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
