import { Link } from '@tanstack/react-router'
import { FormAnswersProvider } from '@/contexts/form-answers.context'
import { useGetFormDetails } from '@/queries/forms'
import { Route } from '@/routes/(app)/elections/$electionRoundId/forms/$formId'
import { FormStatus } from '@/types/form'
import {
  Archive,
  Languages,
  MoreVertical,
  Plus,
  Send,
  Trash2,
} from 'lucide-react'
import { getTranslatedStringOrDefault } from '@/lib/translated-string'
import {
  Breadcrumb,
  BreadcrumbItem,
  BreadcrumbLink,
  BreadcrumbList,
  BreadcrumbPage,
  BreadcrumbSeparator,
} from '@/components/ui/breadcrumb'
import { Button } from '@/components/ui/button'
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from '@/components/ui/dropdown-menu'
import { Tabs, TabsContent, TabsList, TabsTrigger } from '@/components/ui/tabs'
import { H1 } from '@/components/ui/typography'
import FormStatusBadge from '@/components/badges/from-status-badge'
import { PreviewFormDialogs } from './components/Dialogs'
import FormDetailsCard from './components/FormDetailsCard'
import {
  PreviewFormProvider,
  usePreviewForm,
} from './components/PreviewFormProvider'
import QuestionsCard from './components/QuestionsCard'

function PageContent() {
  const { electionRoundId, formId } = Route.useParams()
  const { formLanguage, from } = Route.useSearch()
  const navigate = Route.useNavigate()
  const { data: form } = useGetFormDetails(electionRoundId, formId)
  const { setOpen } = usePreviewForm()

  if (!form) {
    return null
  }

  const formDisplayLanguage = formLanguage ?? form.defaultLanguage

  const formName = getTranslatedStringOrDefault(
    form.name,
    form.defaultLanguage,
    formLanguage
  )

  const formDescription = form.description
    ? getTranslatedStringOrDefault(
        form.description,
        form.defaultLanguage,
        formLanguage
      )
    : undefined

  const handlePublish = () => {
    console.log('Publishing form:', formId)
  }

  return (
    <>
      <Breadcrumb className='mb-4'>
        <BreadcrumbList>
          <BreadcrumbItem>
            <BreadcrumbLink asChild>
              <Link
                to='/elections/$electionRoundId/forms'
                params={{ electionRoundId }}
                search={from}
                className='text-muted-foreground line-clamp-2 text-sm leading-normal font-normal text-balance underline'
              >
                Forms
              </Link>
            </BreadcrumbLink>
          </BreadcrumbItem>
          <BreadcrumbSeparator />
          <BreadcrumbItem>
            <BreadcrumbPage>{formName}</BreadcrumbPage>
          </BreadcrumbItem>
        </BreadcrumbList>
      </Breadcrumb>

      <div className='mb-6'>
        <div className='flex items-center justify-between'>
          <div className='flex items-center gap-3'>
            <H1>{formName}</H1>
            <FormStatusBadge formStatus={form.status} />
          </div>
          <div className='flex items-center gap-2'>
            {form.languages.length > 1 && (
              <DropdownMenu>
                <DropdownMenuTrigger asChild>
                  <Button variant='outline' className='gap-2'>
                    <Languages className='h-5 w-5' />
                    <span>{formDisplayLanguage}</span>
                  </Button>
                </DropdownMenuTrigger>
                <DropdownMenuContent align='end' className='w-48'>
                  {form.languages.map((language) => (
                    <DropdownMenuItem
                      key={language}
                      onClick={() =>
                        navigate({
                          to: '.',
                          search: (prev) => ({
                            ...prev,
                            formLanguage: language,
                          }),
                        })
                      }
                      className='flex cursor-pointer items-center gap-2'
                    >
                      <span className='flex-1'>{language}</span>
                      {formDisplayLanguage === language && (
                        <span className='text-primary'>✓</span>
                      )}
                    </DropdownMenuItem>
                  ))}
                </DropdownMenuContent>
              </DropdownMenu>
            )}

            {/* Primary action button based on status */}
            {form.status === FormStatus.Drafted && (
              <Button onClick={handlePublish} className='gap-2'>
                <Send className='h-4 w-4' />
                Publish
              </Button>
            )}

            {/* Actions menu */}
            <DropdownMenu>
              <DropdownMenuTrigger asChild>
                <Button variant='outline' size='icon'>
                  <MoreVertical className='h-5 w-5' />
                </Button>
              </DropdownMenuTrigger>
              <DropdownMenuContent align='end' className='w-48'>
                <DropdownMenuItem
                  onClick={() => setOpen('addLanguages')}
                  className='gap-2'
                >
                  <Plus className='h-4 w-4' />
                  Add Languages
                </DropdownMenuItem>
                {form.status === FormStatus.Published && (
                  <>
                    <DropdownMenuSeparator />
                    <DropdownMenuItem
                      onClick={() => setOpen('archive')}
                      className='gap-2'
                    >
                      <Archive className='h-4 w-4' />
                      Archive
                    </DropdownMenuItem>
                  </>
                )}
                <DropdownMenuSeparator />
                <DropdownMenuItem
                  onClick={() => setOpen('delete')}
                  className='text-destructive focus:text-destructive gap-2'
                >
                  <Trash2 className='h-4 w-4' />
                  Delete
                </DropdownMenuItem>
              </DropdownMenuContent>
            </DropdownMenu>
          </div>
        </div>
      </div>

      <Tabs defaultValue='details' className='w-full'>
        <TabsList>
          <TabsTrigger value='details'>Form Details</TabsTrigger>
          <TabsTrigger value='questions'>Questions</TabsTrigger>
        </TabsList>

        <TabsContent value='details'>
          <FormDetailsCard form={form} formDescription={formDescription} />
        </TabsContent>

        <TabsContent value='questions'>
          <QuestionsCard
            form={form}
            formDisplayLanguage={formDisplayLanguage}
          />
        </TabsContent>
      </Tabs>
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
