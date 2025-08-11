import * as React from "react";

export interface CurrentElectionRoundContextType {
  electionRoundId: string;
  setElectionRoundId: (electionRoundId: string) => void;
}

const CurrentElectionRoundContext =
  React.createContext<CurrentElectionRoundContextType>({
    electionRoundId: "not-set",
    setElectionRoundId: (_: string) => {},
  });

export function CurrentElectionRoundProvider({
  children,
}: {
  children: React.ReactNode;
}) {
  const [electionRoundId, setElectionRoundId] =
    React.useState<string>("not-set");

  return (
    <CurrentElectionRoundContext.Provider
      value={{ electionRoundId, setElectionRoundId }}
    >
      {children}
    </CurrentElectionRoundContext.Provider>
  );
}

export function useCurrentElectionRound() {
  const context = React.useContext(CurrentElectionRoundContext);
  if (!context) {
    throw new Error(
      "useCurrentElectionRound must be used within a CurrentElectionRoundProvider"
    );
  }
  return context;
}
