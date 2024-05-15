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
import { FormTemplateBase, FormTemplateType, NewFormTemplateRequest, mapFormTemplateType } from '../../models/formTemplate';
import { formTemplatesKeys } from '../../queries';

function CreateTemplateForm() {
    const { t } = useTranslation();
    const navigate = useNavigate();

    const newFormTemplateFormSchema = z.object({
        code: z.string().nonempty(),
        name: z.string().nonempty(),
        description: z.string().optional(),
        defaultLanguage: z.string().nonempty(),
        formTemplateType: z.enum([FormTemplateType.Opening, FormTemplateType.Voting, FormTemplateType.ClosingAndCounting]).catch(FormTemplateType.Opening)
    });

    const form = useForm<z.infer<typeof newFormTemplateFormSchema>>({
        resolver: zodResolver(newFormTemplateFormSchema),
        defaultValues: {
            formTemplateType: FormTemplateType.Opening
        }
    });

    function onSubmit(values: z.infer<typeof newFormTemplateFormSchema>) {
        const name: TranslatedString = {
            [values.defaultLanguage]: values.name!
        };

        const description: TranslatedString = {
            [values.defaultLanguage]: values.description ?? ''
        };

        const newFormTemplate: NewFormTemplateRequest = {
            ...values,
            description,
            name,
            languages: [values.defaultLanguage]
        };

        newFormTemplateMutation.mutate(newFormTemplate);
    }

    const newFormTemplateMutation = useMutation({
        mutationFn: (newFormTemplate: NewFormTemplateRequest) => {
            return authApi.post<FormTemplateBase>(`/form-templates`, newFormTemplate);
        },

        onSuccess: ({ data: formTemplate }) => {
            toast({
                title: 'Success',
                description: 'Form created',
            });

            queryClient.invalidateQueries({ queryKey: formTemplatesKeys.all });
            navigate({ to: `/form-templates/$formTemplateId/edit`, params: { formTemplateId: formTemplate.id } });
        },
    });

    return (
        <Form {...form}>
            <form onSubmit={form.handleSubmit(onSubmit)}>
                <Fieldset className='grid grid-cols-2 gap-12'>
                    <FieldGroup className='!mt-0'>
                        <FormField
                            control={form.control}
                            name='code'
                            render={({ field, fieldState }) => (
                                <Field>
                                    <Label>{t('form-template.field.code')}</Label>
                                    <Input placeholder={t('form-template.placeholder.code')} {...field}  {...fieldState} />
                                    {fieldState.invalid && <ErrorMessage>{fieldState?.error?.message}</ErrorMessage>}
                                </Field>
                            )} />

                        <FormField
                            control={form.control}
                            name="formTemplateType"
                            render={({ field }) => (
                                <Field>
                                    <Label>{t('form-template.field.formTemplateType')}</Label>
                                    <Select onValueChange={field.onChange} defaultValue={field.value}>
                                        <FormControl>
                                            <SelectTrigger>
                                                <SelectValue placeholder="Select a form template type" />
                                            </SelectTrigger>
                                        </FormControl>
                                        <SelectContent>
                                            <SelectItem value={FormTemplateType.Opening}>{mapFormTemplateType(FormTemplateType.Opening)}</SelectItem>
                                            <SelectItem value={FormTemplateType.Voting}>{mapFormTemplateType(FormTemplateType.Voting)}</SelectItem>
                                            <SelectItem value={FormTemplateType.ClosingAndCounting}>{mapFormTemplateType(FormTemplateType.ClosingAndCounting)}</SelectItem>
                                        </SelectContent>
                                    </Select>
                                </Field>
                            )}
                        />
                        <FormField
                            control={form.control}
                            name='name'
                            render={({ field, fieldState }) => (
                                <Field>
                                    <Label>{t('form-template.field.name')}</Label>
                                    <Input placeholder={t('form-template.placeholder.name')} {...field}  {...fieldState} />
                                    {fieldState.invalid && <ErrorMessage>{fieldState?.error?.message}</ErrorMessage>}
                                </Field>)} />

                        <FormField
                            control={form.control}
                            name='defaultLanguage'
                            render={({ field, fieldState }) => (
                                <Field>
                                    <Label>{t('form-template.field.defaultLanguage')}</Label>
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
                                    <Label>{t('form-template.field.description')}</Label>
                                    <Textarea rows={10} cols={100} {...field} placeholder={t('form-template.placeholder.description')} />
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

export default CreateTemplateForm