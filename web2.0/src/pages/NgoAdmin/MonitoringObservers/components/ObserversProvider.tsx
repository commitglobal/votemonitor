import React, { useState } from 'react'
import type { MonitoringObserverModel } from '@/types/monitoring-observer'
import useDialogState from '@/hooks/use-dialog-state'

type ObserversDialogType = 'resendInvite'

type ObserversContextType = {
  open: ObserversDialogType | null
  setOpen: (str: ObserversDialogType | null) => void
  /** The observer the confirmation dialog acts upon. */
  currentRow: MonitoringObserverModel | null
  setCurrentRow: React.Dispatch<
    React.SetStateAction<MonitoringObserverModel | null>
  >
}

const ObserversContext = React.createContext<ObserversContextType | null>(null)

/**
 * Holds which confirmation is open and on which observer, so the row menus and
 * the dialogs read the same state instead of each row owning its own copy.
 */
export function ObserversProvider({ children }: { children: React.ReactNode }) {
  const [open, setOpen] = useDialogState<ObserversDialogType>(null)
  const [currentRow, setCurrentRow] = useState<MonitoringObserverModel | null>(
    null
  )

  return (
    <ObserversContext value={{ open, setOpen, currentRow, setCurrentRow }}>
      {children}
    </ObserversContext>
  )
}

// eslint-disable-next-line react-refresh/only-export-components
export const useObservers = () => {
  const observersContext = React.useContext(ObserversContext)

  if (!observersContext) {
    throw new Error('useObservers has to be used within <ObserversContext>')
  }

  return observersContext
}
