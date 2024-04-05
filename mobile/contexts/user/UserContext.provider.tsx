import { createContext, useContext } from "react";
import {
  useElectionRoundsQuery,
  usePollingStationsNomenclatorQuery,
  usePollingStationsVisits,
} from "../../services/queries.service";
import { ElectionRoundVM, PollingStationVisitVM } from "../../services/definitions.api";

type UserContextType = {
  electionRounds: ElectionRoundVM[];
  visits: PollingStationVisitVM[];
  isAssignedToEllectionRound: boolean;
  isLoading: boolean;
  error: Error | null;
};

export const UserContext = createContext<UserContextType>({
  electionRounds: [],
  visits: [],
  isLoading: false,
  error: null,
  isAssignedToEllectionRound: false,
});

const UserContextProvider = ({ children }: React.PropsWithChildren) => {
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

  // usePollingStationById(25902);
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
      }}
    >
      {children}
    </UserContext.Provider>
  );
};

export const useUserData = () => useContext(UserContext);

export default UserContextProvider;
