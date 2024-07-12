import { authApi } from '@/common/auth-api';
import { TranslatedString } from '@/common/types';
import { CreateDialogFooter } from '@/components/dialogs/CreateDialog';
import { ErrorMessage, Field, FieldGroup, Fieldset, Label } from '@/components/ui/fieldset';
import { Form, FormControl, FormField } from '@/components/ui/form';
import { Input } from '@/components/ui/input';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select';
import { Textarea } from '@/components/ui/textarea';
import { toast } from '@/components/ui/use-toast';
import LanguageSelect from '@/containers/LanguageSelect';
import { queryClient } from '@/main';
import { zodResolver } from '@hookform/resolvers/zod';
import { useMutation } from '@tanstack/react-query';
import { useNavigate } from '@tanstack/react-router';
import { useForm } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import { z } from 'zod';
import { FormBase, FormType, NewFormRequest, mapFormType } from '../../models/form';
import { formsKeys } from '../../queries';

function CreateForm() {
    const { t } = useTranslation();
    const navigate = useNavigate();

    const newFormFormSchema = z.object({
        code: z.string().nonempty('Form code is required'),
        name: z.string().nonempty('Form name is required'),
        description: z.string().optional(),
        defaultLanguage: z.string().nonempty(),
        formType: z.enum([FormType.Opening, FormType.Voting, FormType.ClosingAndCounting, FormType.Other]).catch(FormType.Opening)
    });

    const form = useForm<z.infer<typeof newFormFormSchema>>({
        resolver: zodResolver(newFormFormSchema),
        defaultValues: {
            formType: FormType.Opening
        }
    });

    function onSubmit(values: z.infer<typeof newFormFormSchema>) {
        const name: TranslatedString = {
            [values.defaultLanguage]: values.name!
        };

        const description: TranslatedString = {
            [values.defaultLanguage]: values.description ?? ''
        };

        const newForm: NewFormRequest = {
            ...values,
            description,
            name,
            languages: [values.defaultLanguage]
        };

        newFormMutation.mutate(newForm);
    }

    const newFormMutation = useMutation({
        mutationFn: (newForm: NewFormRequest) => {
            const electionRoundId: string | null = localStorage.getItem('electionRoundId');

            return authApi.post<FormBase>(`/election-rounds/${electionRoundId}/forms`, newForm);
        },

        onSuccess: ({ data: form }) => {
            toast({
                title: 'Success',
                description: 'Form created',
            });

            queryClient.invalidateQueries({ queryKey: formsKeys.all });
            navigate({ to: `/forms/$formId/edit`, params: { formId: form.id } });
        },
    });

    return (
        <Form {...form}>
            <form onSubmit={form.handleSubmit(onSubmit)}>
                <Fieldset className='grid grid-cols-2 gap-12'>
                    <FieldGroup className='!mt-0'>
                        <FormField
                            control={form.control}
                            name="formType"
                            render={({ field }) => (
                                <Field>
                                    <Label>{t('form.field.formType')}</Label>
                                    <Select onValueChange={field.onChange} defaultValue={field.value}>
                                        <FormControl>
                                            <SelectTrigger>
                                                <SelectValue placeholder="Select a form type" />
                                            </SelectTrigger>
                                        </FormControl>
                                        <SelectContent>
                                            <SelectItem value={FormType.Opening}>{mapFormType(FormType.Opening)}</SelectItem>
                                            <SelectItem value={FormType.Voting}>{mapFormType(FormType.Voting)}</SelectItem>
                                            <SelectItem value={FormType.ClosingAndCounting}>{mapFormType(FormType.ClosingAndCounting)}</SelectItem>
                                            <SelectItem value={FormType.Other}>{mapFormType(FormType.Other)}</SelectItem>
                                        </SelectContent>
                                    </Select>
                                </Field>
                            )}
                        />
                        <FormField
                            control={form.control}
                            name='code'
                            render={({ field, fieldState }) => (
                                <Field>
                                    <Label>{t('form.field.code')}</Label>
                                    <Input placeholder={t('form.placeholder.code')} {...field}  {...fieldState} />
                                    {fieldState.invalid && <ErrorMessage>{fieldState?.error?.message}</ErrorMessage>}
                                </Field>
                            )} />
                        <FormField
                            control={form.control}
                            name='name'
                            render={({ field, fieldState }) => (
                                <Field>
                                    <Label>{t('form.field.name')}</Label>
                                    <Input placeholder={t('form.placeholder.name')} {...field}  {...fieldState} />
                                    {fieldState.invalid && <ErrorMessage>{fieldState?.error?.message}</ErrorMessage>}
                                </Field>)} />

                        <FormField
                            control={form.control}
                            name='defaultLanguage'
                            render={({ field, fieldState }) => (
                                <Field>
                                    <Label>{t('form.field.defaultLanguage')}</Label>
                                    <LanguageSelect
                                        languageCode={field.value}
                                        onSelect={field.onChange}
                                    />
                                    {fieldState.invalid && <ErrorMessage>{fieldState?.error?.message}</ErrorMessage>}
                                </Field>
                            )}
                        />
                    </FieldGroup>
                    <FieldGroup className='!mt-0'>
                        <FormField
                            control={form.control}
                            name='description'
                            render={({ field }) => (
                                <Field>
                                    <Label>{t('form.field.description')}</Label>
                                    <Textarea
                                        resizable={false}
                                        rows={10}
                                        cols={100}
                                        {...field}
                                        placeholder={t('form.placeholder.description')} />
                                </Field>
                            )}
                        />
                    </FieldGroup>
                </Fieldset>
                <CreateDialogFooter />
            </form>
        </Form>
    )
}

export default CreateForm