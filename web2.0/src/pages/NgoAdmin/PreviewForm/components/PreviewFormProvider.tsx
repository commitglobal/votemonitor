import useDialogState from '@/hooks/use-dialog-state'
import React from 'react'

type PreviewFormDialogType = 'publish' | 'obsolete' | 'delete' | 'duplicate' | 'addTranslations' | 'deleteTranslation'

type PreviewFormContextType = {
  open: PreviewFormDialogType | null
  setOpen: (str: PreviewFormDialogType | null) => void
}

const PreviewFormContext = React.createContext<PreviewFormContextType | null>(null)

export function PreviewFormProvider({ children }: { children: React.ReactNode }) {
  const [open, setOpen] = useDialogState<PreviewFormDialogType>(null)

  return (
    <PreviewFormContext.Provider value={{ open, setOpen }}>
      {children}
    </PreviewFormContext.Provider>
  )
}

// eslint-disable-next-line react-refresh/only-export-components
export const usePreviewForm = () => {
  const formsContext = React.useContext(PreviewFormContext)

  if (!formsContext) {
    throw new Error('useForms has to be used within <PreviewFormContext>')
  }

  return formsContext
}
