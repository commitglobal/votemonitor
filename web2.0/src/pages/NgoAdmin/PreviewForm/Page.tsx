import { FormAnswersProvider } from '@/contexts/form-answers.context'
import { getTranslatedStringOrDefault } from '@/lib/translated-string'
import { useSuspenseGetFormDetails } from '@/queries/forms'
import { Route } from '@/routes/(app)/elections/$electionRoundId/forms/$formId'
import { PreviewFormDialogs } from './components/Dialogs'
import { FormBreadcrumb } from './components/FormBreadcrumb'
import { FormHeader } from './components/FormHeader'
import { FormTabs } from './components/FormTabs'
import { PreviewFormProvider } from './components/PreviewFormProvider'
import { Language } from '@/types/language'

function PageContent() {
  const { electionRoundId, formId } = Route.useParams()
  const { formLanguage, from } = Route.useSearch()
  const navigate = Route.useNavigate()
  const { data: form } = useSuspenseGetFormDetails(electionRoundId, formId)

  const formName = getTranslatedStringOrDefault(
    form.name,
    form.defaultLanguage,
   ( formLanguage ?? form.defaultLanguage) as Language
  )

  const handleLanguageChange = (language: string) => {
    navigate({
      to: '.',
      search: (prev) => ({
        ...prev,
        formLanguage: language,
      }),
    })
  }

  return (
    <>
      <FormBreadcrumb
        electionRoundId={electionRoundId}
        formName={formName}
        from={from}
      />

      <FormHeader
        formName={formName}
        onLanguageChange={handleLanguageChange}
      />

      <FormTabs />
    </>
  )
}

function Page() {
  return (
    <PreviewFormProvider>
      <FormAnswersProvider>
        <PageContent />
        <PreviewFormDialogs />
      </FormAnswersProvider>
    </PreviewFormProvider>
  )
}

export default Page
