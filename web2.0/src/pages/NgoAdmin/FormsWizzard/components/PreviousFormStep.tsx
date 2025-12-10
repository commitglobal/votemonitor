'use client'

import { useState } from 'react'
import { ChevronDown, FileText, Calendar, Check, ArrowLeft } from 'lucide-react'
import { cn } from '@/lib/utils'
import { Button } from '@/components/ui/button'
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from '@/components/ui/card'
import {
  Collapsible,
  CollapsibleContent,
  CollapsibleTrigger,
} from '@/components/ui/collapsible'

interface PreviousFormStepProps {
  onComplete: (data: unknown) => void
  onBack: () => void
}

// Mock data - replace with actual data from your backend
const electionRounds = [
  {
    id: 'round-2024',
    name: '2024 General Election',
    date: 'November 2024',
    forms: [
      {
        id: 'form-1',
        name: 'Voter Registration Form',
        code: 'VRF-2024',
        lastModified: '2024-10-15',
      },
      {
        id: 'form-2',
        name: 'Absentee Ballot Request',
        code: 'ABR-2024',
        lastModified: '2024-10-10',
      },
      {
        id: 'form-3',
        name: 'Poll Worker Application',
        code: 'PWA-2024',
        lastModified: '2024-09-20',
      },
    ],
  },
  {
    id: 'round-2023',
    name: '2023 Local Elections',
    date: 'May 2023',
    forms: [
      {
        id: 'form-4',
        name: 'Candidate Registration',
        code: 'CR-2023',
        lastModified: '2023-04-01',
      },
      {
        id: 'form-5',
        name: 'Observer Accreditation',
        code: 'OA-2023',
        lastModified: '2023-04-15',
      },
    ],
  },
  {
    id: 'round-2022',
    name: '2022 Midterm Elections',
    date: 'November 2022',
    forms: [
      {
        id: 'form-6',
        name: 'Voter Registration Form',
        code: 'VRF-2022',
        lastModified: '2022-10-01',
      },
      {
        id: 'form-7',
        name: 'Early Voting Request',
        code: 'EVR-2022',
        lastModified: '2022-09-15',
      },
    ],
  },
]

export function PreviousFormStep({
  onComplete,
  onBack,
}: PreviousFormStepProps) {
  const [openRounds, setOpenRounds] = useState<string[]>([electionRounds[0].id])
  const [selectedForm, setSelectedForm] = useState<string | null>(null)

  const toggleRound = (roundId: string) => {
    setOpenRounds((prev) =>
      prev.includes(roundId)
        ? prev.filter((id) => id !== roundId)
        : [...prev, roundId]
    )
  }

  const handleSubmit = () => {
    if (selectedForm) {
      const form = electionRounds
        .flatMap((round) => round.forms)
        .find((f) => f.id === selectedForm)
      onComplete({ type: 'previous', data: form })
    }
  }

  return (
    <Card>
      <CardHeader>
        <CardTitle>Select a Previous Form</CardTitle>
        <CardDescription>
          Choose a form from a previous election round to use as a starting
          point
        </CardDescription>
      </CardHeader>
      <CardContent className='space-y-4'>
        {electionRounds.map((round) => (
          <Collapsible
            key={round.id}
            open={openRounds.includes(round.id)}
            onOpenChange={() => toggleRound(round.id)}
          >
            <CollapsibleTrigger asChild>
              <Button
                variant='ghost'
                className='hover:bg-muted h-auto w-full justify-between px-4 py-3'
              >
                <div className='flex items-center gap-3'>
                  <Calendar className='text-muted-foreground h-5 w-5' />
                  <div className='text-left'>
                    <div className='font-medium'>{round.name}</div>
                    <div className='text-muted-foreground text-sm'>
                      {round.date}
                    </div>
                  </div>
                </div>
                <div className='flex items-center gap-2'>
                  <span className='text-muted-foreground text-sm'>
                    {round.forms.length} forms
                  </span>
                  <ChevronDown
                    className={cn(
                      'h-4 w-4 transition-transform',
                      openRounds.includes(round.id) && 'rotate-180'
                    )}
                  />
                </div>
              </Button>
            </CollapsibleTrigger>
            <CollapsibleContent>
              <div className='mt-2 ml-8 space-y-2'>
                {round.forms.map((form) => (
                  <div
                    key={form.id}
                    className={cn(
                      'flex cursor-pointer items-center justify-between rounded-lg border p-3 transition-colors',
                      selectedForm === form.id
                        ? 'border-primary bg-primary/5'
                        : 'border-border hover:bg-muted/50'
                    )}
                    onClick={() => setSelectedForm(form.id)}
                  >
                    <div className='flex items-center gap-3'>
                      <FileText className='text-muted-foreground h-4 w-4' />
                      <div>
                        <div className='text-sm font-medium'>{form.name}</div>
                        <div className='text-muted-foreground text-xs'>
                          {form.code} • Last modified: {form.lastModified}
                        </div>
                      </div>
                    </div>
                    {selectedForm === form.id && (
                      <div className='bg-primary flex h-5 w-5 items-center justify-center rounded-full'>
                        <Check className='text-primary-foreground h-3 w-3' />
                      </div>
                    )}
                  </div>
                ))}
              </div>
            </CollapsibleContent>
          </Collapsible>
        ))}

        <div className='flex items-center justify-end gap-2 pt-4'>
          <Button variant='ghost' onClick={onBack} className='gap-2'>
            <ArrowLeft className='h-4 w-4' />
            Back
          </Button>
          <Button onClick={handleSubmit} disabled={!selectedForm}>
            Use Selected Form
          </Button>
        </div>
      </CardContent>
    </Card>
  )
}
