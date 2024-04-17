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
import { getPollingStationInformationForm } from "../../services/definitions.api";

type UserContextType = {
  electionRounds: ElectionRoundVM[];
  visits: PollingStationVisitVM[];

  activeElectionRound?: ElectionRoundVM;
  selectedPollingStation?: PollingStationNomenclatorNodeVM | null;

  enoughDataForOffline: boolean;
  isLoading: boolean;

  error: Error | null;
  setSelectedPollingStationId: (pollingStationId: string) => void;
};

export const UserContext = createContext<UserContextType>({
  electionRounds: [],
  visits: [],
  isLoading: false,
  enoughDataForOffline: true,
  error: null,
  setSelectedPollingStationId: (_pollingStationId: string) => {},
});

const UserContextProvider = ({ children }: React.PropsWithChildren) => {
  const queryClient = useQueryClient();

  const [selectedPollingStationId, setSelectedPollingStationId] = useState<string>();

  const {
    data: rounds = [],
    isFetching: isLoadingRounds,
    error: ElectionRoundsError,
  } = useElectionRoundsQuery();

  const activeElectionRound = useMemo(
    () => rounds.find((round) => round.status === "Started"),
    [rounds],
  );

  const {
    data: visits,
    isFetching: isLoadingVisits,
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
    data: nomenclatorExists,
    isFetching: isLoadingNomenclature,
    error: NomenclatureError,
  } = usePollingStationsNomenclatorQuery(activeElectionRound?.id);

  const { data: lastVisitedPollingStation, error: PollingStationNomenclatorNodeDBError } =
    usePollingStationById(currentSelectedPollingStationId);

  // Prefetch some data for the polling stations in advance
  // TODO: Revisit this, sometimes it seems the data is imediately stale and refetching when chaning the station
  useQueries(
    {
      queries:
        visits
          ?.map((visit) => {
            const nodes = {
              queryKey: [pollingStationsKeys.one(visit.pollingStationId)],
              queryFn: () => pollingStationByIdQueryFn(visit.pollingStationId),
              staleTime: 5 * 60 * 1000,
            };
            const informations = {
              queryKey: [
                pollingStationsKeys.pollingStationInformation(
                  activeElectionRound?.id,
                  visit.pollingStationId,
                ),
              ],
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

  return (
    <UserContext.Provider
      value={{
        error:
          ElectionRoundsError ||
          PollingStationsError ||
          NomenclatureError ||
          PollingStationNomenclatorNodeDBError,
        isLoading: isLoadingRounds || isLoadingVisits || isLoadingNomenclature,
        enoughDataForOffline: !!rounds?.length && !!nomenclatorExists,
        visits: visits || [],
        electionRounds: rounds || [],
        activeElectionRound,
        selectedPollingStation: lastVisitedPollingStation,
        setSelectedPollingStationId,
      }}
    >
      {children}
    </UserContext.Provider>
  );
};

export const useUserData = () => useContext(UserContext);

export default UserContextProvider;
