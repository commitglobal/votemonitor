import { createContext, useContext, useState, type ReactNode } from 'react'
import { Language } from '@/types/language'

type DialogType = 'archive' | 'delete' | 'addLanguages' | null

interface PreviewFormContextType {
  open: DialogType
  setOpen: (type: DialogType) => void
  selectedLanguages: Language[]
  setSelectedLanguages: (languages: Language[]) => void
  toggleLanguage: (lang: Language) => void
}

const PreviewFormContext = createContext<PreviewFormContextType | undefined>(
  undefined
)

export function PreviewFormProvider({ children }: { children: ReactNode }) {
  const [open, setOpen] = useState<DialogType>(null)
  const [selectedLanguages, setSelectedLanguages] = useState<Language[]>([])

  const toggleLanguage = (lang: Language) => {
    setSelectedLanguages((prev) =>
      prev.includes(lang) ? prev.filter((l) => l !== lang) : [...prev, lang]
    )
  }

  return (
    <PreviewFormContext.Provider
      value={{
        open,
        setOpen,
        selectedLanguages,
        setSelectedLanguages,
        toggleLanguage,
      }}
    >
      {children}
    </PreviewFormContext.Provider>
  )
}

export function usePreviewForm() {
  const context = useContext(PreviewFormContext)
  if (context === undefined) {
    throw new Error('usePreviewForm must be used within a PreviewFormProvider')
  }
  return context
}
