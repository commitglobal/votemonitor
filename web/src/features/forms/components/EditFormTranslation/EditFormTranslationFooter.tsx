import { authApi } from '@/common/auth-api';
import { DateQuestion, MultiSelectQuestion, NumberQuestion, QuestionType, RatingQuestion, SingleSelectQuestion, TextQuestion } from '@/common/types';
import { Button } from '@/components/ui/button';
import { toast } from '@/components/ui/use-toast';
import { useCurrentElectionRoundStore } from '@/context/election-round.store';
import { queryClient } from '@/main';
import { useMutation } from '@tanstack/react-query';
import { useNavigate } from '@tanstack/react-router';
import { useFormContext } from 'react-hook-form';
import { UpdateFormRequest } from '../../models/form';
import { formsKeys } from '../../queries';
import { EditFormType } from '../EditForm/EditForm';

function EditFormTranslationFooter() {
  const form = useFormContext<EditFormType>();
  const navigate = useNavigate();
    const currentElectionRoundId = useCurrentElectionRoundStore(s => s.currentElectionRoundId);

  const editMutation = useMutation({
    mutationKey: formsKeys.all,
    mutationFn: ({ electionRoundId, form }: { electionRoundId: string; form: UpdateFormRequest, shouldExitEditor: boolean }) => {

      return authApi.put<void>(`/election-rounds/${electionRoundId}/forms/${form.id}`, {
        ...form,
      });
    },

    onSuccess: (_, { shouldExitEditor }) => {
      toast({
        title: 'Success',
        description: 'Form updated successfully',
      });

      void queryClient.invalidateQueries({ queryKey: formsKeys.all });

      if (shouldExitEditor) {
        void navigate({ to: '/election-event/$tab', params: { tab: 'observer-forms' } });
      }
    },

    onError: () => {
      toast({
        title: 'Error saving the form',
        description: 'Please contact tech support',
        variant: 'destructive'
      });
    },
  });

  function saveForm(shouldExitEditor: boolean = false) {
    const values = form.getValues();
debugger;
    const updatedForm: UpdateFormRequest = {
      id: values.formId,
      code: values.code,
      name: values.name,
      defaultLanguage: values.defaultLanguage,
      description: values.description,
      formType: values.formType,
      languages: values.languages,
      questions: values.questions.map(q => {
        if (q.$questionType === QuestionType.NumberQuestionType) {
          const numberQuestion: NumberQuestion = {
            $questionType: QuestionType.NumberQuestionType,
            code: q.code,
            id: q.questionId,
            text: q.text,
            helptext: q.helptext,
            inputPlaceholder: q.inputPlaceholder,
            displayLogic: q.hasDisplayLogic ? { condition: q.condition!, parentQuestionId: q.parentQuestionId!, value: q.value! } : undefined
          };

          return numberQuestion;
        }

        if (q.$questionType === QuestionType.TextQuestionType) {
          const textQuestion: TextQuestion = {
            $questionType: QuestionType.TextQuestionType,
            code: q.code,
            id: q.questionId,
            text: q.text,
            helptext: q.helptext,
            inputPlaceholder: q.inputPlaceholder,
            displayLogic: q.hasDisplayLogic ? { condition: q.condition!, parentQuestionId: q.parentQuestionId!, value: q.value! } : undefined
          };

          return textQuestion;
        }

        if (q.$questionType === QuestionType.RatingQuestionType) {
          const ratingQuestion: RatingQuestion = {
            $questionType: QuestionType.RatingQuestionType,
            code: q.code,
            id: q.questionId,
            text: q.text,
            helptext: q.helptext,
            scale: q.scale,
            lowerLabel: q.lowerLabel,
            upperLabel: q.upperLabel,
            displayLogic: q.hasDisplayLogic ? { condition: q.condition!, parentQuestionId: q.parentQuestionId!, value: q.value! } : undefined
          };

          return ratingQuestion;
        }

        if (q.$questionType === QuestionType.DateQuestionType) {
          const dateQuestion: DateQuestion = {
            $questionType: QuestionType.DateQuestionType,
            code: q.code,
            id: q.questionId,
            text: q.text,
            helptext: q.helptext,
            displayLogic: q.hasDisplayLogic ? { condition: q.condition!, parentQuestionId: q.parentQuestionId!, value: q.value! } : undefined
          };

          return dateQuestion;
        }

        if (q.$questionType === QuestionType.SingleSelectQuestionType) {
          const singleSelectQuestion: SingleSelectQuestion = {
            $questionType: QuestionType.SingleSelectQuestionType,
            code: q.code,
            id: q.questionId,
            text: q.text,
            helptext: q.helptext,
            options: q.options.map(o => ({ id: o.optionId, isFlagged: o.isFlagged, isFreeText: o.isFreeText, text: o.text })),
            displayLogic: q.hasDisplayLogic ? { condition: q.condition!, parentQuestionId: q.parentQuestionId!, value: q.value! } : undefined
          };

          return singleSelectQuestion;
        }

        if (q.$questionType === QuestionType.MultiSelectQuestionType) {
          const multiSelectQuestion: MultiSelectQuestion = {
            $questionType: QuestionType.MultiSelectQuestionType,
            code: q.code,
            id: q.questionId,
            text: q.text,
            helptext: q.helptext,
            options: q.options.map(o => ({ id: o.optionId, isFlagged: o.isFlagged, isFreeText: o.isFreeText, text: o.text })),
            displayLogic: q.hasDisplayLogic ? { condition: q.condition!, parentQuestionId: q.parentQuestionId!, value: q.value! } : undefined
          };

          return multiSelectQuestion;
        }

        throw new Error('unknown question type');
      })
    };

    editMutation.mutate({ electionRoundId: currentElectionRoundId, form: updatedForm, shouldExitEditor });
  }

  return (
    <footer className="fixed left-0 bottom-0 h-[64px] w-full bg-white">
      <div className='container flex items-center justify-end h-full gap-4'>
        <Button type='button' variant='outline' onClick={() => void saveForm()} disabled={!form.formState.isValid}>Save</Button>
        <Button type='button' variant='default' onClick={async () => { saveForm(true); }} disabled={!form.formState.isValid}>Save and exit form editor</Button>
      </div>
    </footer>
  )
}


export default EditFormTranslationFooter
