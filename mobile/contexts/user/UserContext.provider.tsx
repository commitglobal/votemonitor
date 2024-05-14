import { createContext, useContext, useMemo, useState } from "react";
import {
  electionRoundsKeys,
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
import { skipToken, useQueries } from "@tanstack/react-query";
import LoadingScreen from "../../components/LoadingScreen";
import GenericErrorScreen from "../../components/GenericErrorScreen";
import { getElectionRoundAllForms, getFormSubmissions } from "../../services/definitions.api";
import { formSubmissionsQueryFn } from "../../services/queries/form-submissions.query";

type UserContextType = {
  electionRounds: ElectionRoundVM[] | undefined;
  visits: PollingStationVisitVM[] | undefined;

  activeElectionRound?: ElectionRoundVM;
  selectedPollingStation?: PollingStationNomenclatorNodeVM | null;

  isLoading: boolean;

  error: Error | null;
  setSelectedPollingStationId: (pollingStationId: string | null) => void;
};

export const UserContext = createContext<UserContextType | null>(null);

const UserContextProvider = ({ children }: React.PropsWithChildren) => {
  const [selectedPollingStationId, setSelectedPollingStationId] = useState<string | null>();

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

  useQueries({
    queries: [
      ...(visits
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
          const submissions = {
            queryKey: pollingStationsKeys.formSubmissions(
              activeElectionRound?.id,
              visit.pollingStationId,
            ),
            queryFn: () => formSubmissionsQueryFn(activeElectionRound?.id, visit.pollingStationId),
          };
          return [nodes, informations, submissions];
        })
        ?.flat() || []),
      {
        queryKey: electionRoundsKeys.forms(activeElectionRound?.id),
        queryFn: activeElectionRound?.id
          ? () => getElectionRoundAllForms(activeElectionRound.id)
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
