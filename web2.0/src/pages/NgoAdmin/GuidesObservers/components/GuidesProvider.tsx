import React, { useState } from 'react'
import type { GuidesObserverModel, GuideType } from '@/types/guides-observer'
import useDialogState from '@/hooks/use-dialog-state'

type GuidesDialogType = 'create' | 'update' | 'delete'

type GuidesContextType = {
  open: GuidesDialogType | null
  setOpen: (str: GuidesDialogType | null) => void
  /** The guide the update and delete dialogs act upon. */
  currentRow: GuidesObserverModel | null
  setCurrentRow: React.Dispatch<
    React.SetStateAction<GuidesObserverModel | null>
  >
  /**
   * Type of the guide being created. Picked from the upload menu before the
   * dialog opens, since it decides which fields the form shows and can never
   * be changed afterwards.
   */
  newGuideType: GuideType | null
  setNewGuideType: React.Dispatch<React.SetStateAction<GuideType | null>>
}

const GuidesContext = React.createContext<GuidesContextType | null>(null)

/**
 * Holds which guide dialog is open and what it works on, so the page header,
 * the table rows and the dialogs all read the same state.
 */
export function GuidesProvider({ children }: { children: React.ReactNode }) {
  const [open, setOpen] = useDialogState<GuidesDialogType>(null)
  const [currentRow, setCurrentRow] = useState<GuidesObserverModel | null>(null)
  const [newGuideType, setNewGuideType] = useState<GuideType | null>(null)

  return (
    <GuidesContext
      value={{
        open,
        setOpen,
        currentRow,
        setCurrentRow,
        newGuideType,
        setNewGuideType,
      }}
    >
      {children}
    </GuidesContext>
  )
}

// eslint-disable-next-line react-refresh/only-export-components
export const useGuides = () => {
  const guidesContext = React.useContext(GuidesContext)

  if (!guidesContext) {
    throw new Error('useGuides has to be used within <GuidesContext>')
  }

  return guidesContext
}
