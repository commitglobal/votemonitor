import { questionSchema, formEditSchema } from '@/components/form-builder/shared'
import { useAppForm } from "@/hooks/form"
import { useSuspenseGetFormDetails } from "@/queries/forms"
import { Route } from "@/routes/(app)/elections/$electionRoundId/forms/$formId/edit.$languageCode"
import { FormType } from "@/types/form"
import { Language, LanguageList } from "@/types/language"
import { revalidateLogic } from "@tanstack/react-form"
import z from "zod"
import { FormBreadcrumb } from "./components/FormBreadcrumb"
import { FormEditor } from "./components/FormEditor"
import { FormHeader } from "./components/FormHeader"

export const editFormSchema = z.object({
    code: z
      .string()
      .min(1, 'Form code is required')
      .max(256, 'Form code must be less than 256 characters'),
    name: z.string().min(2, 'Form name is required'),
    description: z.string().optional(),
    defaultLanguage: z.enum(LanguageList, { message: 'Language is required' }),
    formType: z.enum(
      [
        FormType.Opening,
        FormType.Voting,
        FormType.ClosingAndCounting,
        FormType.CitizenReporting,
        FormType.IncidentReporting,
        FormType.Other,
        FormType.PSI,
      ],
      { message: 'Form type is required' }
    ),
  })
  
function Page() {

  // const { activeQuestionId, setActiveQuestionId } = useActiveQuestion()
  const { electionRoundId, formId, languageCode } = Route.useParams()
  const { data: formData } = useSuspenseGetFormDetails(electionRoundId, formId)

  const form = useAppForm({
    defaultValues: {
      formType: formData.formType,
      defaultLanguage: formData.defaultLanguage,
      code: formData.code,
      name: formData.name[languageCode as Language],
      description: formData.description?.[languageCode as Language] ?? "",
      languages: formData.languages,
      icon: formData.icon,
      questions: [] as z.infer<typeof questionSchema>[],
      // questions: formData.questions.map((question) => ({
      //   questionId: question.id,
      //   text: question.text[languageCode as Language],
      //   hasDisplayLogic: !!question.displayLogic,
      //   code: question.code,
      //   $questionType: question.$questionType,
      //   lowerLabel: question.lowerLabel?.[languageCode as Language],
      //   upperLabel: question.upperLabel?.[languageCode as Language],
      //   scale: question.scale,
      //   inputPlaceholder: question.inputPlaceholder?.[languageCode as Language],
      //   options: question.options.map((option) => ({
      //     optionId: option.id,
      //     text: option.text[languageCode as Language],
      //     isFlagged: option.isFlagged,
      //     isFreeText: option.isFreeText,
      //   })),
      // })) as z.infer<typeof questionSchema>[]
    },
    onSubmit: ({ value, meta }) => {
      console.log(value);
      console.log(formEditSchema.parse(value));
      console.log(meta);
    },
    validators: {
      onDynamic: formEditSchema,
    },
    validationLogic: revalidateLogic({
      mode: "submit",
      modeAfterSubmission: "blur",
    }),
  });



  return (
      <form.AppForm>
        <form
          onSubmit={(e) => {
            e.preventDefault();
            form.handleSubmit({ key: "value" });
          }}
        >
      <FormBreadcrumb />
      <FormHeader />
      <FormEditor form={form} />
    </form>
    </form.AppForm>
  )
}

export default Page