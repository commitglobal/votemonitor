import * as React from 'react'
import { ElectionModel } from '@/types/election'

export interface CurrentElectionRoundContextType {
  electionRound: ElectionModel | undefined
  setElectionRound: (electionRound: ElectionModel | undefined) => void
}

const CurrentElectionRoundContext =
  React.createContext<CurrentElectionRoundContextType>({
    electionRound: undefined,
    setElectionRound: (_: ElectionModel | undefined) => {},
  })

export function CurrentElectionRoundProvider({
  children,
}: {
  children: React.ReactNode
}) {
  const [electionRound, setElectionRound] = React.useState<
    ElectionModel | undefined
  >()

  return (
    <CurrentElectionRoundContext.Provider
      value={{ electionRound, setElectionRound }}
    >
      {children}
    </CurrentElectionRoundContext.Provider>
  )
}

export function useCurrentElectionRound() {
  const context = React.useContext(CurrentElectionRoundContext)
  if (!context) {
    throw new Error(
      'useCurrentElectionRound must be used within a CurrentElectionRoundProvider'
    )
  }
  return context
}
