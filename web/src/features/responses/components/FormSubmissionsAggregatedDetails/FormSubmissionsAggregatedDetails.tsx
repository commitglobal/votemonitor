import { usePrevSearch } from '@/common/prev-search-store';
import type { FunctionComponent } from '@/common/types';
import Layout from '@/components/layout/Layout';
import { NavigateBack } from '@/components/NavigateBack/NavigateBack';
import { useCurrentElectionRoundStore } from '@/context/election-round.store';
import { mapFormType } from '@/lib/utils';
import { formAggregatedDetailsQueryOptions, Route } from '@/routes/responses/form-submissions/$formId.aggregated';
import { useSuspenseQuery } from '@tanstack/react-query';
import { Link } from '@tanstack/react-router';
import { SubmissionType } from '../../models/common';
import type { Responder } from '../../models/form-submissions-aggregated';
import { AggregateCard } from '../AggregateCard/AggregateCard';
import { Card, CardContent, CardTitle } from '@/components/ui/card';
import { Select, SelectContent, SelectGroup, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select';
import { useState } from 'react';
import { LanguageBadge } from '@/components/ui/language-badge';

export default function FormSubmissionsAggregatedDetails(): FunctionComponent {
  const { formId } = Route.useParams();
  const prevSearch = usePrevSearch();
  const params = Route.useSearch();

  const currentElectionRoundId = useCurrentElectionRoundStore((s) => s.currentElectionRoundId);

  const {
    data: {
      submissionsAggregate: { defaultLanguage, formCode, formType, aggregates, responders, languages },
      attachments,
      notes,
    },
  } = useSuspenseQuery(formAggregatedDetailsQueryOptions(currentElectionRoundId, formId, params));

  const respondersAggregated = responders.reduce<Record<string, Responder>>(
    (grouped, responder) => ({
      ...grouped,
      [responder.responderId]: responder,
    }),
    {}
  );
  const [selectedLanguage, setSelectedLanguage] = useState<string>(defaultLanguage);

  return (
    <Layout
      backButton={<NavigateBack to='/responses' search={prevSearch} />}
      breadcrumbs={<></>}
      title={`${formCode} - ${mapFormType(formType)}`}>
      <div className='flex flex-col gap-10'>
        {languages.length > 1 && (
          <Card>
            <CardTitle className='p-6'>Form language</CardTitle>
            <CardContent>
              <div className='w-64'>
                <Select onValueChange={setSelectedLanguage} defaultValue={selectedLanguage} value={selectedLanguage}>
                  <SelectTrigger>
                    <SelectValue placeholder='Language' />
                  </SelectTrigger>
                  <SelectContent>
                    <SelectGroup>
                      {languages.map((language) => (
                        <SelectItem key={language} value={language}>
                          {LanguageBadge({ languageCode: language, variant: 'unstyled', displayMode: 'english' })}
                        </SelectItem>
                      ))}
                    </SelectGroup>
                  </SelectContent>
                </Select>
              </div>
            </CardContent>
          </Card>
        )}
        {Object.values(aggregates).map((aggregate) => {
          return (
            <AggregateCard
              key={aggregate.questionId}
              submissionType={SubmissionType.FormSubmission}
              aggregate={aggregate}
              language={selectedLanguage}
              responders={respondersAggregated}
              attachments={attachments}
              notes={notes}
            />
          );
        })}
      </div>
    </Layout>
  );
}
