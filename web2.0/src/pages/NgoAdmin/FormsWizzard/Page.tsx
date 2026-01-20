import { useState } from 'react'
import {
  FilePlus,
  History,
  LayoutTemplate,
  ArrowLeft,
  Check,
} from 'lucide-react'
import { cn } from '@/lib/utils'
import { Button } from '@/components/ui/button'
import { Card, CardHeader, CardTitle } from '@/components/ui/card'
import { CardDescription } from '@/components/ui/card'
import { H1 } from '@/components/ui/typography'
import { NewFormStep } from './components/NewFormStep'
import { PreviousFormStep } from './components/PreviousFormStep'
import { TemplateFormStep } from './components/TemplateFormStep'

export type WizardOption = 'new' | 'previous' | 'template' | null

function Page() {
  const [step, setStep] = useState(1)
  const [selectedOption, setSelectedOption] = useState<WizardOption>(null)

  const options = [
    {
      id: 'new' as const,
      title: 'Start a New Form',
      description: 'Create a brand new form from scratch',
      icon: FilePlus,
    },
    {
      id: 'previous' as const,
      title: 'Use a Previous Form',
      description: 'Copy settings from an existing form',
      icon: History,
    },
    {
      id: 'template' as const,
      title: 'Use a Form Template',
      description: 'Start with a pre-built template',
      icon: LayoutTemplate,
    },
  ]

  const handleOptionSelect = (option: WizardOption) => {
    setSelectedOption(option)
  }

  const handleNext = () => {
    if (selectedOption) {
      setStep(2)
    }
  }

  const handleBack = () => {
    setStep(1)
    setSelectedOption(null)
  }

  const handleComplete = (data: unknown) => {
    // console.log(data)
  }

  return (
    <>
      <div className='mx-auto w-full max-w-5xl space-y-6'>
        <H1 className='mb-2'>Create a Form</H1>
        {/* Progress indicator */}
        <div className='mb-8 flex items-center justify-center gap-2'>
          {[1, 2].map((s) => (
            <div key={s} className='flex items-center gap-2'>
              <div
                className={cn(
                  'flex h-8 w-8 items-center justify-center rounded-full text-sm font-medium transition-colors',
                  step >= s
                    ? 'bg-primary text-primary-foreground'
                    : 'bg-muted text-muted-foreground'
                )}
              >
                {step > s ? <Check className='h-4 w-4' /> : s}
              </div>
              {s < 2 && (
                <div
                  className={cn(
                    'h-0.5 w-12 transition-colors',
                    step > s ? 'bg-primary' : 'bg-muted'
                  )}
                />
              )}
            </div>
          ))}
        </div>

        {/* Step 1: Choose option */}
        {step === 1 && (
          <div className='space-y-6'>
            <div className='text-center'>
              <h2 className='text-foreground text-2xl font-semibold'>
                How would you like to create your form?
              </h2>
              <p className='text-muted-foreground mt-2'>
                Choose an option to get started
              </p>
            </div>

            <div className='grid gap-4 sm:grid-cols-2 lg:grid-cols-3'>
              {options.map((option) => {
                const Icon = option.icon
                return (
                  <Card
                    key={option.id}
                    className={cn(
                      'hover:border-primary/50 cursor-pointer transition-all',
                      selectedOption === option.id &&
                        'border-primary ring-primary/20 ring-2'
                    )}
                    onClick={() => handleOptionSelect(option.id)}
                  >
                    <CardHeader className='flex flex-row items-center gap-4 py-4'>
                      <div
                        className={cn(
                          'flex h-12 w-12 items-center justify-center rounded-lg transition-colors',
                          selectedOption === option.id
                            ? 'bg-primary text-primary-foreground'
                            : 'bg-muted text-muted-foreground'
                        )}
                      >
                        <Icon className='h-6 w-6' />
                      </div>
                      <div className='flex-1'>
                        <CardTitle className='text-lg'>
                          {option.title}
                        </CardTitle>
                        <CardDescription>{option.description}</CardDescription>
                      </div>
                      <div
                        className={cn(
                          'h-5 w-5 rounded-full border-2 transition-colors',
                          selectedOption === option.id
                            ? 'border-primary bg-primary'
                            : 'border-muted-foreground'
                        )}
                      >
                        {selectedOption === option.id && (
                          <Check className='text-primary-foreground h-full w-full p-0.5' />
                        )}
                      </div>
                    </CardHeader>
                  </Card>
                )
              })}
            </div>

            <div className='flex justify-end'>
              <Button onClick={handleNext} disabled={!selectedOption}>
                Continue
              </Button>
            </div>
          </div>
        )}

        {/* Step 2: Option-specific content */}
        {step === 2 && (
          <div className='space-y-6'>
            <Button variant='ghost' onClick={handleBack} className='gap-2'>
              <ArrowLeft className='h-4 w-4' />
              Back
            </Button>

            {selectedOption === 'new' && (
              <NewFormStep onBack={handleBack} />
            )}
            {selectedOption === 'previous' && (
              <PreviousFormStep
                onComplete={handleComplete}
                onBack={handleBack}
              />
            )}
            {selectedOption === 'template' && (
              <TemplateFormStep
                onComplete={handleComplete}
                onBack={handleBack}
              />
            )}
          </div>
        )}
      </div>
    </>
  )
}

export default Page
