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
  selectedPollingStation?: PollingStationNomenclatorNodeVM;
  isLoading: boolean;
  error: Error | null;
  setSelectedPollingStation: (pollingStation: PollingStationNomenclatorNodeVM) => void;
};

export const UserContext = createContext<UserContextType>({
  electionRounds: [],
  visits: [],
  isLoading: false,
  error: null,
  isAssignedToEllectionRound: false,
  setSelectedPollingStation: (_pollingStation: PollingStationNomenclatorNodeVM) => {},
});

const UserContextProvider = ({ children }: React.PropsWithChildren) => {
  const [selectedPollingStation, setSelectedPollingStation] =
    useState<PollingStationNomenclatorNodeVM>();

  const {
    data: rounds,
    isFetching: isLoadingRounds,
    error: ElectionRoundsError,
  } = useElectionRoundsQuery();
  const {
    data: visits,
    isFetching: isLoadingVisits,
    error: PollingStationsError,
  } = usePollingStationsVisits(rounds ? rounds.electionRounds[0].id : "");
  const { isFetching: isLoadingNomenclature, error: NomenclatureError } =
    usePollingStationsNomenclatorQuery(rounds ? rounds.electionRounds[0].id : "");

  const lastVisit = useMemo(
    () =>
      visits?.visits.sort(
        (a, b) => new Date(b.visitedAt).getTime() - new Date(a.visitedAt).getTime(),
      )[0],
    [visits],
  );

  console.log("lastVisit", lastVisit);

  const { data: currentPollingStation } = usePollingStationById(lastVisit?.pollingStationId || "");
  console.log(currentPollingStation);

  // TODO: Prefetch query for details for the active one

  return (
    <UserContext.Provider
      value={{
        error: ElectionRoundsError || PollingStationsError || NomenclatureError,
        isLoading: isLoadingRounds || isLoadingVisits || isLoadingNomenclature,
        visits: visits?.visits || [],
        electionRounds: rounds?.electionRounds || [],
        isAssignedToEllectionRound:
          (rounds?.electionRounds && rounds?.electionRounds?.length > 0) || false,
        selectedPollingStation: currentPollingStation as PollingStationNomenclatorNodeVM,
        setSelectedPollingStation,
      }}
    >
      {children}
    </UserContext.Provider>
  );
};

export const useUserData = () => useContext(UserContext);

export default UserContextProvider;
