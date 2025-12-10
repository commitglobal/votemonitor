'use client'

import { useState } from 'react'
import { Check, ArrowLeft } from 'lucide-react'
import { cn } from '@/lib/utils'
import { Badge } from '@/components/ui/badge'
import { Button } from '@/components/ui/button'
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from '@/components/ui/card'
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from '@/components/ui/table'

interface TemplateFormStepProps {
  onComplete: (data: unknown) => void
  onBack: () => void
}

// Mock data - replace with actual data from your backend
const templates = [
  {
    id: 'template-1',
    name: 'Voter Registration',
    category: 'Registration',
    fields: 12,
    description: 'Standard voter registration form with ID verification',
  },
  {
    id: 'template-2',
    name: 'Absentee Ballot Request',
    category: 'Voting',
    fields: 8,
    description: 'Request form for mail-in/absentee ballots',
  },
  {
    id: 'template-3',
    name: 'Poll Worker Application',
    category: 'Staffing',
    fields: 15,
    description: 'Application for election day poll workers',
  },
  {
    id: 'template-4',
    name: 'Observer Accreditation',
    category: 'Observers',
    fields: 10,
    description: 'Accreditation form for election observers',
  },
  {
    id: 'template-5',
    name: 'Candidate Registration',
    category: 'Candidates',
    fields: 20,
    description: 'Comprehensive candidate registration and nomination form',
  },
  {
    id: 'template-6',
    name: 'Complaint Form',
    category: 'Complaints',
    fields: 6,
    description: 'General election complaint submission form',
  },
]

const categoryColors: Record<string, string> = {
  Registration: 'bg-blue-100 text-blue-800 dark:bg-blue-900 dark:text-blue-300',
  Voting: 'bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-300',
  Staffing:
    'bg-orange-100 text-orange-800 dark:bg-orange-900 dark:text-orange-300',
  Observers:
    'bg-purple-100 text-purple-800 dark:bg-purple-900 dark:text-purple-300',
  Candidates: 'bg-pink-100 text-pink-800 dark:bg-pink-900 dark:text-pink-300',
  Complaints: 'bg-red-100 text-red-800 dark:bg-red-900 dark:text-red-300',
}

export function TemplateFormStep({
  onComplete,
  onBack,
}: TemplateFormStepProps) {
  const [selectedTemplate, setSelectedTemplate] = useState<string | null>(null)

  const handleSubmit = () => {
    if (selectedTemplate) {
      const template = templates.find((t) => t.id === selectedTemplate)
      onComplete({ type: 'template', data: template })
    }
  }

  return (
    <Card>
      <CardHeader>
        <CardTitle>Choose a Template</CardTitle>
        <CardDescription>
          Select a pre-built template to quickly create your form
        </CardDescription>
      </CardHeader>
      <CardContent className='space-y-4'>
        <div className='overflow-hidden rounded-lg border'>
          <Table>
            <TableHeader>
              <TableRow>
                <TableHead className='w-12'></TableHead>
                <TableHead>Template Name</TableHead>
                <TableHead>Category</TableHead>
                <TableHead className='text-center'>Fields</TableHead>
                <TableHead className='hidden md:table-cell'>
                  Description
                </TableHead>
              </TableRow>
            </TableHeader>
            <TableBody>
              {templates.map((template) => (
                <TableRow
                  key={template.id}
                  className={cn(
                    'cursor-pointer transition-colors',
                    selectedTemplate === template.id && 'bg-primary/5'
                  )}
                  onClick={() => setSelectedTemplate(template.id)}
                >
                  <TableCell>
                    <div
                      className={cn(
                        'flex h-5 w-5 items-center justify-center rounded-full border-2 transition-colors',
                        selectedTemplate === template.id
                          ? 'border-primary bg-primary'
                          : 'border-muted-foreground'
                      )}
                    >
                      {selectedTemplate === template.id && (
                        <Check className='text-primary-foreground h-3 w-3' />
                      )}
                    </div>
                  </TableCell>
                  <TableCell className='font-medium'>{template.name}</TableCell>
                  <TableCell>
                    <Badge
                      variant='secondary'
                      className={categoryColors[template.category]}
                    >
                      {template.category}
                    </Badge>
                  </TableCell>
                  <TableCell className='text-center'>
                    {template.fields}
                  </TableCell>
                  <TableCell className='text-muted-foreground hidden text-sm md:table-cell'>
                    {template.description}
                  </TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        </div>

        <div className='flex items-center justify-end gap-2 pt-4'>
          <Button variant='ghost' onClick={onBack} className='gap-2'>
            <ArrowLeft className='h-4 w-4' />
            Back
          </Button>
          <Button onClick={handleSubmit} disabled={!selectedTemplate}>
            Use Selected Template
          </Button>
        </div>
      </CardContent>
    </Card>
  )
}
