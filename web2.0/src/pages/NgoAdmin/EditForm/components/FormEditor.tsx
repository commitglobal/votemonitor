import { FormDescription } from '@/components/form-builder/FormDescription'
import { FormQuestions } from '@/components/form-builder/FormQuestions'
import { formEditOpts } from '@/components/form-builder/shared'
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
import { FieldGroup } from '@/components/ui/field'
import { Tabs, TabsContent, TabsList, TabsTrigger } from '@/components/ui/tabs'
import { withForm } from '@/hooks/form'
import { QuestionType } from '@/types/form'
import { createContext, ReactNode, useContext, useState } from 'react'

interface ActiveQuestionContextType {
  activeQuestionId: string | undefined
  setActiveQuestionId: (questionId: string | undefined) => void
}

const ActiveQuestionContext = createContext<ActiveQuestionContextType | undefined>(undefined)

function useActiveQuestion() {
  const context = useContext(ActiveQuestionContext)
  if (!context) {
    throw new Error('useActiveQuestion must be used within ActiveQuestionProvider')
  }
  return context
}

function ActiveQuestionProvider({ children }: { children: ReactNode }) {
  const [activeQuestionId, setActiveQuestionId] = useState<string | undefined>(undefined)

  return (
    <ActiveQuestionContext.Provider value={{ activeQuestionId, setActiveQuestionId }}>
      {children}
    </ActiveQuestionContext.Provider>
  )
}

export const FormEditor = withForm({
  ...formEditOpts,

  render: function Render({ form }) {
    return (<div className='h-[calc(100vh-12rem)]'>

      <Tabs defaultValue='details' className='h-full flex flex-col'>
        <TabsList>
          <TabsTrigger value='details'>Form Details</TabsTrigger>
          <TabsTrigger value='questions'>Questions</TabsTrigger>
        </TabsList>

        <TabsContent value='details' className='flex-1 overflow-y-auto mt-4'>
          <FormDescription form={form} />
        </TabsContent>

        <TabsContent value='questions' className='flex-1 overflow-hidden mt-4'>
          <FormQuestions form={form} />
        </TabsContent>
      </Tabs>

    </div>
  )},
});



function FormDetailsTab() {

  return (
    <Card>
      <CardContent className='pt-6'>

      </CardContent>
    </Card>
  )
}

function QuestionsTab() {


  return (
    <div className='flex h-full gap-4'>
      {/* Left Sidebar - Questions List */}
      <div className='w-1/2 flex flex-col'>
        <Card className='flex-1 flex flex-col overflow-hidden'>
          <CardHeader>
            <CardTitle>Questions</CardTitle>
          </CardHeader>
          <CardContent className='flex-1 overflow-y-auto'>
            <FieldGroup>
              {/* <form.AppField name="firstName">
              {(field) => {
                return (
                  <field.TextInput
                    id="firstName"
                    label="First Name"
                    description="Your First Name"
                  />
                );
              }}
            </form.AppField>

            <form.AppField name="lastName">
              {(field) => (
                <field.TextInput
                  id="lastName"
                  label="Last Name"
                  description="Your Last Name"
                />
              )}
            </form.AppField>

            <form.AppField name="address.street">
              {(field) => {
                return (
                  <field.TextInput
                    label="Street"
                    id="addressStreet"
                    description="Some Description"
                  />
                );
              }}
            </form.AppField>

            <PasswordFields
              form={form}
              fields={{
                password: "password",
                confirmPassword: "confirmPassword",
              }}
            /> */}

              {/* <FormQuestions form={form} /> */}

              {/* <form.AppField name="acceptTerms">
              {(field) => {
                return <field.Checkbox id="acceptTerms" label="Accept Terms" />;
              }}
            </form.AppField> */}
            </FieldGroup>

            {/* <form.SubmitButton /> */}



          </CardContent>
        </Card>
      </div>

      {/* Right Side - Preview */}
      <div className='w-1/2 flex flex-col'>
        <Card className='flex-1 flex flex-col overflow-hidden'>
          <CardHeader>
            <CardTitle>Preview</CardTitle>
          </CardHeader>
          <CardContent className='flex-1 overflow-y-auto'>
            <PreviewPanel />
          </CardContent>
        </Card>
      </div>
    </div>
  )
}

function PreviewPanel() {
  // const { activeQuestionId } = useActiveQuestion()


  return (
    <div className='space-y-4'>
      <div className='border-b pb-4'>
        <h2 className='text-2xl font-bold'>
          {/* {getTranslation(formData.name, formDisplayLanguage)} */}
        </h2>
        {/* {formData.description && (
          <P className='text-muted-foreground mt-2'>
            {getTranslation(formData.description, formDisplayLanguage)}
          </P>
        )} */}
      </div>

      {/* {!activeQuestionId ? (
        <P className='text-muted-foreground text-center py-8'>
          Select a question to preview it here.
        </P>
      ) : activeQuestion ? (
        <div className='space-y-2'>
          <div className='flex items-start gap-2'>
            <Badge variant='secondary'>{activeQuestion.code}</Badge>
            <div className='flex-1'>
              <label className='text-sm font-semibold'>
                {getTranslation(activeQuestion.text, formDisplayLanguage) ||
                  `Question`}
              </label>
              {activeQuestion.helptext && (
                <P className='text-xs text-muted-foreground mt-1'>
                  {getTranslation(activeQuestion.helptext, formDisplayLanguage)}
                </P>
              )}
            </div>
          </div>
          <div className='ml-12'>
            <div className='h-10 border rounded-md bg-muted/50 flex items-center px-3 text-sm text-muted-foreground'>
              {getQuestionTypePreview(activeQuestion.$questionType)}
            </div>
          </div>
        </div>
      ) : (
        <P className='text-muted-foreground text-center py-8'>
          Question not found.
        </P>
      )} */}
    </div>
  )
}



function getQuestionTypeLabel(type: QuestionType): string {
  const labels: Record<QuestionType, string> = {
    [QuestionType.TextQuestionType]: 'Text',
    [QuestionType.NumberQuestionType]: 'Number',
    [QuestionType.DateQuestionType]: 'Date',
    [QuestionType.SingleSelectQuestionType]: 'Single Select',
    [QuestionType.MultiSelectQuestionType]: 'Multi Select',
    [QuestionType.RatingQuestionType]: 'Rating',
  }
  return labels[type] || 'Unknown'
}

function getQuestionTypePreview(type: QuestionType): string {
  const previews: Record<QuestionType, string> = {
    [QuestionType.TextQuestionType]: 'Text input field',
    [QuestionType.NumberQuestionType]: 'Number input field',
    [QuestionType.DateQuestionType]: 'Date picker',
    [QuestionType.SingleSelectQuestionType]: 'Single select dropdown',
    [QuestionType.MultiSelectQuestionType]: 'Multi select checkboxes',
    [QuestionType.RatingQuestionType]: 'Rating scale',
  }
  return previews[type] || 'Input field'
}

