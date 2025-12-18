import type { FormModel } from '@/types/form'
import { mapFormType, mapLanguageNameByCode } from '@/lib/i18n'
import { Badge } from '@/components/ui/badge'
import { Card, CardContent } from '@/components/ui/card'
import {
  Item,
  ItemContent,
  ItemDescription,
  ItemGroup,
  ItemTitle,
} from '@/components/ui/item'
import { P } from '@/components/ui/typography'
import LanguagesTranslationStatusBadge from '@/components/badges/form-translation-badge'

interface FormDetailsCardProps {
  form: FormModel
  formDescription?: string
}

function FormDetailsCard({ form, formDescription }: FormDetailsCardProps) {
  const hasMultipleLanguages = form.languages.length > 1
  const availableLanguages = form.languages.filter(
    (lang) => lang !== form.defaultLanguage
  )

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
                      <ItemTitle>Available Languages</ItemTitle>
                      <div className='flex flex-wrap gap-3'>
                        {availableLanguages.map((lang) => {
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
