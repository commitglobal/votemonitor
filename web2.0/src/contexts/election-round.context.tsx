import * as React from 'react'
import { useNavigate } from '@tanstack/react-router'
import { ElectionModel } from '@/types/election'

export interface CurrentElectionRoundContextType {
  electionRound: ElectionModel
  setElectionRound: (electionRound: ElectionModel) => void
}

const CurrentElectionRoundContext =
  React.createContext<CurrentElectionRoundContextType>({
    electionRound: undefined!,
    setElectionRound: () => {},
  })

export function CurrentElectionRoundProvider({
  children,
}: {
  children: React.ReactNode
}) {
  const [electionRound, setElectionRound] = React.useState<
    ElectionModel | undefined
  >(undefined)

  return (
    <CurrentElectionRoundContext.Provider
      value={{ electionRound: electionRound!, setElectionRound }}
    >
      {children}
    </CurrentElectionRoundContext.Provider>
  )
}

export function useCurrentElectionRound() {
  const context = React.useContext(CurrentElectionRoundContext)
  const navigate = useNavigate()

  React.useEffect(() => {
    if (!context) {
      navigate({ to: '/' })
    }
  }, [context, navigate])

  if (!context) {
    // Temporarily return a placeholder while redirect happens
    throw new Error(
      'useCurrentElectionRound must be used within a CurrentElectionRoundProvider with an active election round'
    )
  }

  return context
}
