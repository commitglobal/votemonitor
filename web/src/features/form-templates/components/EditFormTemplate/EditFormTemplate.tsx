import { useLoaderData, useNavigate } from '@tanstack/react-router';
import Layout from '@/components/layout/Layout';
import { Card, CardContent, CardFooter, CardHeader, CardTitle } from '@/components/ui/card';
import { Separator } from '@/components/ui/separator';
import { useForm } from 'react-hook-form';
import { z } from 'zod';
import { zodResolver } from '@hookform/resolvers/zod';
import { authApi } from '@/common/auth-api';
import { useMutation } from '@tanstack/react-query';
import { useToast } from '@/components/ui/use-toast';
import { FormTemplateFull } from '../../models/formTemplate';
import { Route as FormTemplatesListRoute } from '@/routes/form-templates/'

export default function EditFormTemplate() {
  const navigate = useNavigate({ from: FormTemplatesListRoute.fullPath })

  const formTemplate: FormTemplateFull = useLoaderData({ strict: false });
  const { toast } = useToast();

  const editFormTemplateFormSchema = z.object({
    status: z.string(),
  });

  const form = useForm<z.infer<typeof editFormTemplateFormSchema>>({
    resolver: zodResolver(editFormTemplateFormSchema),
    defaultValues: {
      status: formTemplate.status,
    },
  });

  function onSubmit(values: z.infer<typeof editFormTemplateFormSchema>) {
    const updatedFormTemplate: FormTemplateFull = {
      ...formTemplate
    };

    editMutation.mutate(updatedFormTemplate);
  }

  const deleteMutation = useMutation({
    mutationFn: (formTemplateId: string) => {
      return authApi.delete<void>(`/form-templates/${formTemplateId}`);
    },
    onSuccess: () => {
      navigate({ search: (prev) => (prev) });
    },
  });

  const editMutation = useMutation({
    mutationFn: (obj: FormTemplateFull) => {

      return authApi.post<void>(
        `/form-templates/${formTemplate.id}`,
        obj
      );
    },

    onSuccess: () => {
      toast({
        title: 'Success',
        description: 'Observer successfully updated',
      });
    },
  });

  const handleDelete = () => {
    deleteMutation.mutate(formTemplate.id);
  };


  const { setValue } = form;

  return (
    <Layout title={`Edit ${formTemplate.code} - ${formTemplate.name[formTemplate.defaultLanguage]}`}>
      <Card className='w-[800px] pt-0'>
        <CardHeader className='flex flex-column gap-2'>
          <div className='flex flex-row justify-between items-center'>
            <CardTitle className='text-xl'>Edit form template</CardTitle>
          </div>
          <Separator />
        </CardHeader>
        <CardContent className='flex flex-col gap-6 items-baseline'>
          <div className='flex flex-col gap-1'>
            <p className='text-gray-700 font-bold'>Code</p>
            <p className='text-gray-900 font-normal'>{formTemplate.code}</p>
          </div>

        </CardContent>
        <CardFooter className='flex justify-between'></CardFooter>
      </Card>
    </Layout>
  );
}
