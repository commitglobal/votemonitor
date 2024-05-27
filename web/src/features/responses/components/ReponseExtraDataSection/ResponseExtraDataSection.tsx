import type { FunctionComponent } from '@/common/types';
import { Button } from '@/components/ui/button';
import { ChevronDownIcon, ChevronUpIcon } from '@heroicons/react/24/outline';
import { useState } from 'react';
import type { Attachment, Note } from '../../models/form-submission';
import { ReponseExtraDataTable } from '../ReponseExtraDataTable/ReponseExtraDataTable';
import { QuestionExtraData } from '../../types';

type ResponseExtraDataSectionProps = {
  attachments: Attachment[];
  notes: Note[];
};

export function ResponseExtraDataSection({ attachments, notes }: ResponseExtraDataSectionProps): FunctionComponent {
  const [expanded, setExpanded] = useState(false);

  const extraData: QuestionExtraData[] = [
    ...notes.map(n => ({ ...n, type: "Note" } as QuestionExtraData)),
    ...attachments.map(n => ({ ...n, type: "Attachment" } as QuestionExtraData))
  ];

  return (
    <>
      <Button
        className='text-purple-500 hover:no-underline max-w-fit pl-0'
        onClick={() => {
          setExpanded((prev) => !prev);
        }}
        variant='link'>
        {notes.length} notes & {attachments.length} media files{' '}
        {expanded ? <ChevronUpIcon className='w-4 ml-2' /> : <ChevronDownIcon className='w-4 ml-2' />}
      </Button>
      {expanded && <ReponseExtraDataTable data={extraData} />}
    </>
  );
}
