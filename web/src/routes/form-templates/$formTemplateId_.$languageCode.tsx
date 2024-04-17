import FormTemplateDetails from '@/features/form-templates/components/FormTemplateDetails/FormTemplateDetails';
import { formTemplateDetailsQueryOptions } from '@/features/form-templates/queries';
import { createFileRoute } from '@tanstack/react-router';


export const Route = createFileRoute('/form-templates/$formTemplateId/$languageCode')({
  component: Details,
  loader: ({ context: { queryClient }, params: { formTemplateId } }) =>
    queryClient.ensureQueryData(formTemplateDetailsQueryOptions(formTemplateId)),
});


function Details() {
  return (
    <div className='p-2'>
      <FormTemplateDetails />
    </div>
  );
}




// function EditFormTemplate() {
//   const formTemplate = Route.useLoaderData();
//   const [localForm, setLocalForm] = useState<FormTemplateFull | null>();
//   const [localQuestions, setLocalQuestions] = useState<BaseQuestion[]>([]);
//   const [activeQuestionId, setActiveQuestionId] = useState<string | undefined>(undefined);
//   const [invalidQuestions, setInvalidQuestions] = useState<string[] | null>(null);

//   useEffect(() => {
//     if (formTemplate) {
//       if (localForm) return;
//       setLocalForm(JSON.parse(JSON.stringify(formTemplate)) as FormTemplateFull);
//       setLocalQuestions(JSON.parse(JSON.stringify(formTemplate.questions ?? [])) as BaseQuestion[]);

//       if (formTemplate.questions.length > 0) {
//         setActiveQuestionId(formTemplate.questions[0]!.id!);
//       }
//     }
//   }, [formTemplate]);

//   return (
//     <>
//       <FormTemplateActions formTemplate={formTemplate} />
//       <FormTemplateHeader formTemplate={formTemplate} />
//       <FormQuestionsEditor
//         languageCode={formTemplate.defaultLanguage}
//         localQuestions={localQuestions}
//         setLocalQuestions={setLocalQuestions}
//         activeQuestionId={activeQuestionId}
//         setActiveQuestionId={setActiveQuestionId}
//         invalidQuestions={invalidQuestions}
//         setInvalidQuestions={setInvalidQuestions}
//       />
//     </>
//   );
// }
