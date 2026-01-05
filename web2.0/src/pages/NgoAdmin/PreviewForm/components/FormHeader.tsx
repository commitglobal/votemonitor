import FormStatusBadge from '@/components/badges/from-status-badge'
import LanguagesTranslationStatusBadge from '@/components/badges/form-translation-badge'
import { Badge } from '@/components/ui/badge'
import { H1 } from '@/components/ui/typography'
import { useSuspenseGetFormDetails } from '@/queries/forms'
import { Route } from '@/routes/(app)/elections/$electionRoundId/forms/$formId'
import { Language } from '@/types/language'
import { FormActionsMenu } from './FormActionsMenu'
import { LanguageSelector } from './LanguageSelector'

interface FormHeaderProps {
  formName: string
  onLanguageChange: (language: string) => void
}

export function FormHeader({
  formName,
  onLanguageChange,
}: FormHeaderProps) {
  const { electionRoundId, formId } = Route.useParams()
  const { formLanguage } = Route.useSearch()
  const { data: form } = useSuspenseGetFormDetails(electionRoundId, formId)

  const formDisplayLanguage = formLanguage ?? form.defaultLanguage
  const isDefaultLanguage = formDisplayLanguage === form.defaultLanguage

  return (
    <div className='mb-6'>
      <div className='flex items-center justify-between'>
        <div className='flex items-center gap-3'>
          <H1>{formName}</H1>
          <FormStatusBadge formStatus={form.status} />
          {isDefaultLanguage ? (
            <Badge variant='default'>Default Language</Badge>
          ) : (
            <LanguagesTranslationStatusBadge
              language={formDisplayLanguage as Language}
              translationStatus={form.languagesTranslationStatus}
            />
          )}
        </div>
        <div className='flex items-center gap-2'>
          <LanguageSelector
            languages={form.languages}
            currentLanguage={formDisplayLanguage}
            onLanguageChange={onLanguageChange}
          />

          <FormActionsMenu />
        </div>
      </div>
    </div>
  )
}

