import AsyncStorage from "@react-native-async-storage/async-storage";
import { UseMutateFunction, useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { createContext, useContext, useEffect, useMemo } from "react";
import { useGetCitizenElectionRounds } from "../../services/queries/citizen.query";
import { ICitizenElectionRound } from "../../services/api/citizen/get-citizen-election-rounds";
import LoadingScreen from "../../components/LoadingScreen";

type CitizenUserContextType = {
  isLoading: boolean;
  selectedElectionRound: string | null;
  citizenElectionRounds: ICitizenElectionRound[];
  setSelectedElectionRound: UseMutateFunction<void, Error, string | null, unknown>;
};

const CITIZEN_USER_STORAGE_KEYS = {
  SELECTED_ELECTION_ROUND: "citizen:selectedElectionRound",
};

const useAsyncStorageCitizenElectionRound = () => {
  return useQuery({
    queryKey: ["citizen:selectedElectionRound"],
    queryFn: () => AsyncStorage.getItem(CITIZEN_USER_STORAGE_KEYS.SELECTED_ELECTION_ROUND),
    staleTime: 0,
    networkMode: "always",
  });
};

const useMutationAsyncStorageCitizenElectionRound = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationKey: ["citizen:selectedElectionRoundMutation"],
    mutationFn: (electionRoundId: string | null) => {
      if (!electionRoundId) {
        return AsyncStorage.removeItem(CITIZEN_USER_STORAGE_KEYS.SELECTED_ELECTION_ROUND);
      } else {
        return AsyncStorage.setItem(
          CITIZEN_USER_STORAGE_KEYS.SELECTED_ELECTION_ROUND,
          electionRoundId,
        );
      }
    },
    onSettled: async () => {
      await queryClient.invalidateQueries({ queryKey: ["citizen:selectedElectionRound"] });
    },
  });
};

export const CitizenUserContext = createContext<CitizenUserContextType | null>(null);

const CitizenUserContextProvider = ({ children }: React.PropsWithChildren) => {
  const { data: selectedElectionRound, isLoading: isLoadingSelectedElectionRoundStored } =
    useAsyncStorageCitizenElectionRound();

  const { mutate: setSelectedElectionRound } = useMutationAsyncStorageCitizenElectionRound();

  const { data: citizenElectionRounds, isLoading: isLoadingCitizenElectionRounds } =
    useGetCitizenElectionRounds();

  const isLoading = isLoadingSelectedElectionRoundStored || isLoadingCitizenElectionRounds;

  useEffect(() => {
    if (!isLoading && selectedElectionRound && citizenElectionRounds) {
      const isValidElectionRound = citizenElectionRounds.some(
        (round) => round.id === selectedElectionRound,
      );

      if (!isValidElectionRound) {
        setSelectedElectionRound(null);
      }
    }
  }, [isLoading, selectedElectionRound, citizenElectionRounds]);

  const contextValues = useMemo(() => {
    return {
      isLoading,
      selectedElectionRound: selectedElectionRound || null,
      setSelectedElectionRound,
      citizenElectionRounds: citizenElectionRounds || [],
    };
  }, [selectedElectionRound, citizenElectionRounds, isLoading]);

  if (isLoading) {
    return <LoadingScreen />;
  }

  return (
    <CitizenUserContext.Provider value={contextValues}>{children}</CitizenUserContext.Provider>
  );
};

export const useCitizenUserData = () => {
  const data = useContext(CitizenUserContext);

  if (!data) {
    throw new Error("No data in CitizenUserContext found. Was used outside the tree");
  }

  return data;
};

export default CitizenUserContextProvider;
