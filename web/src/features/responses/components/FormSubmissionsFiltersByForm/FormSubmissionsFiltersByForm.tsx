import { FilteringContainer } from '@/features/filtering/components/FilteringContainer';
import { FC } from 'react';
import { FormSubmissionsFlaggedAnswersSelect } from '../../filtering/FormSubmissionsFlaggedAnswersSelect';
import { FormSubmissionsMediaFilesSelect } from '../../filtering/FormSubmissionsMediaFilesSelect';
import { FormSubmissionsQuestionNotesSelect } from '../../filtering/FormSubmissionsQuestionNotesSelect';

export const FormSubmissionsFiltersByForm: FC = () => {
  return (
    <FilteringContainer>
      <FormSubmissionsFlaggedAnswersSelect />
      <FormSubmissionsQuestionNotesSelect />
      <FormSubmissionsMediaFilesSelect />
    </FilteringContainer>
  );
};
