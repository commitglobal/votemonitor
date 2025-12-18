import { LanguagesTranslationStatus, TranslationStatus } from '@/types/form'
import { Language } from '@/types/language'
import { cn } from '@/lib/utils'
import { Badge } from '@/components/ui/badge'

export interface LanguagesTranslationStatusProps {
  translationStatus: LanguagesTranslationStatus
  language: Language
}

function LanguagesTranslationStatusBadge({
  translationStatus,
  language,
}: LanguagesTranslationStatusProps) {
  return (
    <Badge
      className={cn({
        'bg-green-200 text-green-600':
          translationStatus[language] === TranslationStatus.Translated,
        'bg-yellow-200 text-yellow-600':
          translationStatus[language] === TranslationStatus.MissingTranslations,
        'bg-slate-200 text-slate-700':
          translationStatus[language] === undefined,
      })}
    >
      {translationStatus[language] === TranslationStatus.Translated
        ? 'Translated'
        : translationStatus[language] === TranslationStatus.MissingTranslations
          ? 'Missing translation'
          : 'Unknown'}
    </Badge>
  )
}

export default LanguagesTranslationStatusBadge
