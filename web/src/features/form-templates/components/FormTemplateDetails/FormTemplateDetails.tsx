import { useLoaderData, useNavigate } from '@tanstack/react-router';
import Layout from '@/components/layout/Layout';
import { Card, CardContent, CardFooter, CardHeader, CardTitle } from '@/components/ui/card';
import { Tabs, TabsContent, TabsList, TabsTrigger } from '@/components/ui/tabs';
import { Badge } from '@/components/ui/badge';
import { Separator } from '@/components/ui/separator';
import { Button } from '@/components/ui/button';
import { PencilIcon } from '@heroicons/react/24/outline';
import { FormTemplateFull, mapFormTemplateType } from '../../models/formTemplate';
import PreviewQuestionFactory from '@/components/questionsEditor/preview/PreviewQuestionFactory';
import { BaseAnswer } from '@/common/types';

export default function FormTemplateDetails() {
  const formTemplate: FormTemplateFull = useLoaderData({ strict: false });
  const navigate = useNavigate();
  const navigateToEdit = () => {
    navigate({ to: '/form-templates/$formTemplateId/edit', params: { formTemplateId: formTemplate.id } });
  };

  return (
    <Layout title={`${formTemplate.code} - ${formTemplate.name[formTemplate.defaultLanguage]}`}>
      <Tabs defaultValue='form-details'>
        <TabsList className='grid grid-cols-2 bg-gray-200 w-[400px] mb-4'>
          <TabsTrigger value='form-details'>Template form details</TabsTrigger>
          <TabsTrigger value='questions'>Questions</TabsTrigger>
        </TabsList>
        <TabsContent value='form-details'>
          <Card className='w-[800px] pt-0'>
            <CardHeader className='flex flex-column gap-2'>
              <div className='flex flex-row justify-between items-center'>
                <CardTitle className='text-xl'>Template form details</CardTitle>
                <Button onClick={navigateToEdit} variant='ghost-primary'>
                  <PencilIcon className='w-[18px] mr-2 text-purple-900' />
                  <span className='text-base text-purple-900'>Edit</span>
                </Button>
              </div>
              <Separator />
            </CardHeader>
            <CardContent className='flex flex-col gap-6 items-baseline	'>
              <div className='flex flex-col gap-1'>
                <p className='text-gray-700 font-bold'>Code</p>
                <p className='text-gray-900 font-normal'>{formTemplate.code}</p>
              </div>
              <div className='flex flex-col gap-1'>
                <p className='text-gray-700 font-bold'>Name</p>
                <p className='text-gray-900 font-normal'>{formTemplate.name[formTemplate.defaultLanguage]}</p>
              </div>
              <div className='flex flex-col gap-1'>
                <p className='text-gray-700 font-bold'>Description</p>
                <p className='text-gray-900 font-normal'>{formTemplate.description[formTemplate.defaultLanguage]}</p>
              </div>
              <div className='flex flex-col gap-1'>
                <p className='text-gray-700 font-bold'>Form template type</p>
                <p className='text-gray-900 font-normal'>{mapFormTemplateType(formTemplate.formTemplateType)}</p>
              </div>
              <div className='flex flex-col gap-1'>
                <p className='text-gray-700 font-bold'>Languages</p>
                <p className='text-gray-900 font-normal'>{formTemplate.languages.join(',')}</p>
              </div>
              <div className='flex flex-col gap-1'>
                <p className='text-gray-700 font-bold'>Status</p>
                <Badge className={'badge-' + formTemplate.status}>{formTemplate.status}</Badge>
              </div>
            </CardContent>
            <CardFooter className='flex justify-between'></CardFooter>
          </Card>
        </TabsContent>
        <TabsContent value='questions'>{
          formTemplate.questions.map(q => <PreviewQuestionFactory
            languageCode={formTemplate.defaultLanguage}
            question={q}
            key={q.id}
            answer={undefined}
            isFirstQuestion={false}
            isLastQuestion={false}
            onSubmitAnswer={(answer: BaseAnswer) => { }}
            onBackButtonClicked={() => { }} />)
        }
        </TabsContent>
      </Tabs>
    </Layout>
  );
}
