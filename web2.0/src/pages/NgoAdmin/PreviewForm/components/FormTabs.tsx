import { Tabs, TabsContent, TabsList, TabsTrigger } from '@/components/ui/tabs'
import FormDetailsCard from './FormDetailsCard'
import QuestionsCard from './QuestionsCard'

export function FormTabs() {
  return (
    <Tabs defaultValue='details' className='w-full'>
      <TabsList>
        <TabsTrigger value='details'>Form Details</TabsTrigger>
        <TabsTrigger value='questions'>Questions</TabsTrigger>
      </TabsList>

      <TabsContent value='details'>
        <FormDetailsCard />
      </TabsContent>

      <TabsContent value='questions'>
        <QuestionsCard />
      </TabsContent>
    </Tabs>
  )
}

