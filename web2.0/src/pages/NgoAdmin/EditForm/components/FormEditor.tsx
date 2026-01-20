import { FormDescription } from '@/components/form-builder/FormDescription'
import { FormQuestions } from '@/components/form-builder/FormQuestions'
import { formEditOpts } from '@/components/form-builder/shared'
import { Tabs, TabsContent, TabsList, TabsTrigger } from '@/components/ui/tabs'
import { withForm } from '@/hooks/form'



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

