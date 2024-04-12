import { createContext, useContext, useMemo, useState } from "react";
import {
  useElectionRoundsQuery,
  usePollingStationById,
  usePollingStationsNomenclatorQuery,
  usePollingStationsVisits,
} from "../../services/queries.service";
import { ElectionRoundVM } from "../../services/definitions.api";
import {
  PollingStationNomenclatorNodeVM,
  PollingStationVisitVM,
} from "../../common/models/polling-station.model";

type UserContextType = {
  electionRounds: ElectionRoundVM[];
  visits: PollingStationVisitVM[];

  activeElectionRound?: ElectionRoundVM;
  selectedPollingStation?: PollingStationNomenclatorNodeVM | null;

  isLoading: boolean;
  error: Error | null;
  setSelectedPollingStationId: (pollingStationId: string) => void;
};

export const UserContext = createContext<UserContextType>({
  electionRounds: [],
  visits: [],
  isLoading: false,
  error: null,
  setSelectedPollingStationId: (_pollingStationId: string) => {},
});

const UserContextProvider = ({ children }: React.PropsWithChildren) => {
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

  const { isFetching: isLoadingNomenclature, error: NomenclatureError } =
    usePollingStationsNomenclatorQuery(activeElectionRound?.id);

  const { data: lastVisitedPollingStation } = usePollingStationById(
    currentSelectedPollingStationId,
  );

  return (
    <UserContext.Provider
      value={{
        error: ElectionRoundsError || PollingStationsError || NomenclatureError,
        isLoading: isLoadingRounds || isLoadingVisits || isLoadingNomenclature,
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
