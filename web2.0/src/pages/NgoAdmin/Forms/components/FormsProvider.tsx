import React, { useState } from 'react'
import { FormModel } from '@/types/form'
import useDialogState from '@/hooks/use-dialog-state'

type FormsDialogType = 'create' | 'update' | 'delete' | 'import'

type FormsContextType = {
  open: FormsDialogType | null
  setOpen: (str: FormsDialogType | null) => void
  currentRow: FormModel | null
  setCurrentRow: React.Dispatch<React.SetStateAction<FormModel | null>>
}

const FormsContext = React.createContext<FormsContextType | null>(null)

export function FormsProvider({ children }: { children: React.ReactNode }) {
  const [open, setOpen] = useDialogState<FormsDialogType>(null)
  const [currentRow, setCurrentRow] = useState<FormModel | null>(null)

  return (
    <FormsContext value={{ open, setOpen, currentRow, setCurrentRow }}>
      {children}
    </FormsContext>
  )
}

// eslint-disable-next-line react-refresh/only-export-components
export const useForms = () => {
  const formsContext = React.useContext(FormsContext)

  if (!formsContext) {
    throw new Error('useForms has to be used within <FormsContext>')
  }

  return formsContext
}
