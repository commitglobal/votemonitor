import { createContext, useContext, useMemo, useState } from "react";
import {
  useElectionRoundsQuery,
  usePollingStationById,
  usePollingStationsNomenclatorQuery,
  usePollingStationsVisits,
} from "../../services/queries.service";
import { ElectionRoundVM, PollingStationVisitVM } from "../../services/definitions.api";
import { PollingStationNomenclatorNodeVM } from "../../common/models/polling-station.model";

type UserContextType = {
  electionRounds: ElectionRoundVM[];
  visits: PollingStationVisitVM[];
  isAssignedToEllectionRound: boolean;

  activeElectionRound?: ElectionRoundVM;
  selectedPollingStation?: PollingStationNomenclatorNodeVM;

  isLoading: boolean;
  error: Error | null;
  setSelectedPollingStationId: (pollingStationId: string) => void;
};

export const UserContext = createContext<UserContextType>({
  electionRounds: [],
  visits: [],
  isLoading: false,
  error: null,
  isAssignedToEllectionRound: false,
  setSelectedPollingStationId: (_pollingStationId: string) => {},
});

const UserContextProvider = ({ children }: React.PropsWithChildren) => {
  const [selectedPollingStationId, setSelectedPollingStationId] = useState<string>();

  const {
    data: rounds,
    isFetching: isLoadingRounds,
    error: ElectionRoundsError,
  } = useElectionRoundsQuery();

  const activeElectionRound = useMemo(
    () => rounds?.electionRounds?.find((round) => round.status === "Started"),
    [rounds],
  );

  const {
    data: visits,
    isFetching: isLoadingVisits,
    error: PollingStationsError,
  } = usePollingStationsVisits(activeElectionRound?.id);

  const currentSelectedpollingStationId = useMemo(() => {
    return (
      selectedPollingStationId ||
      visits?.visits.sort(
        (a, b) => new Date(b.visitedAt).getTime() - new Date(a.visitedAt).getTime(),
      )[0]?.pollingStationId
    );
  }, [visits, selectedPollingStationId]);

  const { isFetching: isLoadingNomenclature, error: NomenclatureError } =
    usePollingStationsNomenclatorQuery(activeElectionRound?.id);

  const { data: lastVisitedPollingStation } = usePollingStationById(
    currentSelectedpollingStationId,
  );

  // TODO: Prefetch query for details for the active one

  return (
    <UserContext.Provider
      value={{
        error: ElectionRoundsError || PollingStationsError || NomenclatureError,
        isLoading: isLoadingRounds || isLoadingVisits || isLoadingNomenclature,
        visits: visits?.visits || [],
        electionRounds: rounds?.electionRounds || [],
        activeElectionRound,
        isAssignedToEllectionRound:
          (rounds?.electionRounds && rounds?.electionRounds?.length > 0) || false,
        selectedPollingStation: lastVisitedPollingStation as PollingStationNomenclatorNodeVM,
        setSelectedPollingStationId,
      }}
    >
      {children}
    </UserContext.Provider>
  );
};

export const useUserData = () => useContext(UserContext);

export default UserContextProvider;
