import { useState } from 'react'
import { mapFormType, mapLanguageNameByCode } from '@/lib/i18n'
import { getTranslatedStringOrDefault } from '@/lib/translated-string'
import { Badge } from '@/components/ui/badge'
import { Button } from '@/components/ui/button'
import { Card, CardContent } from '@/components/ui/card'
import {
  Collapsible,
  CollapsibleContent,
  CollapsibleTrigger,
} from '@/components/ui/collapsible'
import {
  Item,
  ItemContent,
  ItemDescription,
  ItemGroup,
  ItemTitle,
} from '@/components/ui/item'
import { P } from '@/components/ui/typography'
import { ChevronDownIcon } from 'lucide-react'
import LanguagesTranslationStatusBadge from '@/components/badges/form-translation-badge'
import { useSuspenseGetFormDetails } from '@/queries/forms'
import { Route } from '@/routes/(app)/elections/$electionRoundId/forms/$formId'

function FormDetailsCard() {
  const { electionRoundId, formId } = Route.useParams()
  const { formLanguage } = Route.useSearch()
  const { data: form } = useSuspenseGetFormDetails(electionRoundId, formId)

  const formDescription = form.description
    ? getTranslatedStringOrDefault(
        form.description,
        form.defaultLanguage,
        formLanguage
      )
    : undefined
  const hasMultipleLanguages = form.languages.length > 1
  const availableLanguages = form.languages.filter(
    (lang) => lang !== form.defaultLanguage
  )
  const [isLanguagesOpen, setIsLanguagesOpen] = useState(false)
  const firstThreeLanguages = availableLanguages.slice(0, 3)
  const remainingLanguages = availableLanguages.slice(3)

  return (
    <Card>
      <CardContent className='pt-6'>
        <div className='grid grid-cols-1 gap-6 lg:grid-cols-2'>
          {/* Left Column - Form Metadata */}
          <div className='space-y-2'>
            <ItemGroup>
              <Item>
                <ItemContent>
                  <ItemTitle>Form Code</ItemTitle>
                  <ItemDescription>{form.code}</ItemDescription>
                </ItemContent>
              </Item>

              <Item>
                <ItemContent>
                  <ItemTitle>Form Type</ItemTitle>
                  <ItemDescription>
                    {mapFormType(form.formType)}
                  </ItemDescription>
                </ItemContent>
              </Item>

              {hasMultipleLanguages ? (
                <>
                  <Item>
                    <ItemContent>
                      <ItemTitle>Default Language</ItemTitle>
                      <ItemDescription>
                        {mapLanguageNameByCode(form.defaultLanguage)}
                      </ItemDescription>
                    </ItemContent>
                  </Item>

                  <Item>
                    <ItemContent>
                      <ItemTitle>Available Languages ({availableLanguages.length})</ItemTitle>
                      <div className='flex flex-col gap-3'>
                        {firstThreeLanguages.map((lang) => {
                          return (
                            <div key={lang} className='flex items-center gap-2'>
                              <span className='text-sm font-medium'>
                                {mapLanguageNameByCode(lang)}
                              </span>
                              <LanguagesTranslationStatusBadge
                                language={lang}
                                translationStatus={
                                  form.languagesTranslationStatus
                                }
                              />
                            </div>
                          )
                        })}
                        {remainingLanguages.length > 0 && (
                          <Collapsible
                            open={isLanguagesOpen}
                            onOpenChange={setIsLanguagesOpen}
                          >
                            {!isLanguagesOpen && (
                              <CollapsibleTrigger asChild>
                                <Button
                                  variant='ghost'
                                  size='sm'
                                  className='w-fit'
                                >
                                  {`Show ${remainingLanguages.length} more`}
                                  <ChevronDownIcon className='ml-1 size-4' />
                                </Button>
                              </CollapsibleTrigger>
                            )}
                            <CollapsibleContent>
                              <div className='flex flex-col gap-3 pt-1'>
                                {remainingLanguages.map((lang) => {
                                  return (
                                    <div key={lang} className='flex items-center gap-2'>
                                      <span className='text-sm font-medium'>
                                        {mapLanguageNameByCode(lang)}
                                      </span>
                                      <LanguagesTranslationStatusBadge
                                        language={lang}
                                        translationStatus={
                                          form.languagesTranslationStatus
                                        }
                                      />
                                    </div>
                                  )
                                })}
                                <CollapsibleTrigger asChild>
                                  <Button
                                    variant='ghost'
                                    size='sm'
                                    className='w-fit'
                                  >
                                    Show less
                                    <ChevronDownIcon className='ml-1 size-4 rotate-180' />
                                  </Button>
                                </CollapsibleTrigger>
                              </div>
                            </CollapsibleContent>
                          </Collapsible>
                        )}
                      </div>
                    </ItemContent>
                  </Item>
                </>
              ) : (
                <Item>
                  <ItemContent>
                    <ItemTitle>Form Language</ItemTitle>
                    <div className='flex flex-wrap gap-2'>
                      <Badge variant='default'>
                        {mapLanguageNameByCode(form.defaultLanguage)}
                      </Badge>
                    </div>
                  </ItemContent>
                </Item>
              )}

              <Item>
                <ItemContent>
                  <ItemTitle>Number of Questions</ItemTitle>
                  <ItemDescription>{form.numberOfQuestions}</ItemDescription>
                </ItemContent>
              </Item>

              <Item>
                <ItemContent>
                  <ItemTitle>Last Modified</ItemTitle>
                  <ItemDescription>
                    {/* {format(form.lastModifiedOn, DateTimeFormat)} by{' '}
                    {form.lastModifiedBy} */}
                  </ItemDescription>
                </ItemContent>
              </Item>
            </ItemGroup>
          </div>

          {/* Right Column - Form Description */}
          <div>
            <h3 className='mb-3 text-lg font-semibold'>Description</h3>
            {formDescription ? (
              <P className='text-muted-foreground'>{formDescription}</P>
            ) : (
              <P className='text-muted-foreground italic'>-</P>
            )}
          </div>
        </div>
      </CardContent>
    </Card>
  )
}

export default FormDetailsCard
