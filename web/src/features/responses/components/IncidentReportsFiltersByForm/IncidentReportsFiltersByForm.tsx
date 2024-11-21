import { FunctionComponent } from '@/common/types';
import { PollingStationsFilters } from '@/components/PollingStationsFilters/PollingStationsFilters';
import { FilteringContainer } from '@/features/filtering/components/FilteringContainer';
import { FormSubmissionsCompletionFilter } from '@/features/filtering/components/FormSubmissionsCompletionFilter';
import { FormSubmissionsFlaggedAnswersFilter } from '@/features/filtering/components/FormSubmissionsFlaggedAnswersFilter';
import { FormSubmissionsFollowUpFilter } from '@/features/filtering/components/FormSubmissionsFollowUpFilter';
import { FormSubmissionsFormFilter } from '@/features/filtering/components/FormSubmissionsFormFilter';
import { FormSubmissionsMediaFilesFilter } from '@/features/filtering/components/FormSubmissionsMediaFilesFilter';
import { FormSubmissionsQuestionNotesFilter } from '@/features/filtering/components/FormSubmissionsQuestionNotesFilter';
import { FormSubmissionsQuestionsAnsweredFilter } from '@/features/filtering/components/FormSubmissionsQuestionsAnsweredFilter';
import { IncidentReportsLocationTypeFilter } from '@/features/filtering/components/LocationTypeFilters';

export function IncidentReportsFiltersByForm(): FunctionComponent {
  return (
    <>
      <FilteringContainer>
        <FormSubmissionsFormFilter />
        <FormSubmissionsCompletionFilter />
        <FormSubmissionsFlaggedAnswersFilter />
        <FormSubmissionsFollowUpFilter />
        <IncidentReportsLocationTypeFilter />
        <FormSubmissionsQuestionsAnsweredFilter />
        <FormSubmissionsQuestionNotesFilter />
        <FormSubmissionsMediaFilesFilter />
        <PollingStationsFilters />
      </FilteringContainer>
    </>
  );
}
