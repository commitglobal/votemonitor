import { createContext, useContext, useEffect, useState } from "react";

type CitizenUserContextType = {
  selectedElectionRound: string;
  setSelectedElectionRound: (electionRoundId: string) => void;
};

export const CitizenUserContext = createContext<CitizenUserContextType | null>(null);

const CitizenUserContextProvider = ({ children }: React.PropsWithChildren) => {
  const [selectedElectionRound, setSelectedElectionRound] = useState<string>("");

  useEffect(() => {
    console.log("👀 [Context] selectedElectionRound", selectedElectionRound);
  }, [selectedElectionRound]);

  return (
    <CitizenUserContext.Provider value={{ selectedElectionRound, setSelectedElectionRound }}>
      {children}
    </CitizenUserContext.Provider>
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
