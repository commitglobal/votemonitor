import { ChevronDownIcon, ChevronUpIcon } from '@heroicons/react/24/outline';
import { useState } from 'react';
import type { FunctionComponent } from '@/common/types';
import { Button } from '@/components/ui/button';
import type { QuestionExtraData } from '../../types';
import { QuestionExtraDataTable } from '../QuestionExtraDataTable/QuestionExtraDataTable';
import type { Attachment, Note } from '../../models/form-submission';
import { isEqual } from 'date-fns';

type QuestionExtraDataSectionProps = {
  attachments: Attachment[];
  notes: Note[];
};

export function QuestionExtraDataSection({ attachments, notes }: QuestionExtraDataSectionProps): FunctionComponent {
  const [expanded, setExpanded] = useState(false);

  const groupedAttachments = attachments.reduce<Record<string, Attachment[]>>(
    (grouped, attachment) => ({
      ...grouped,
      [attachment.timeSubmitted]: [...(grouped[attachment.timeSubmitted] ?? []), attachment],
    }),
    {}
  );

  const mediaFiles: QuestionExtraData[] = Object.entries(groupedAttachments)
    .filter(([key]) => !notes.some((note) => isEqual(note.timeSubmitted, key)))
    .flatMap(([_, attachments]) => ({
      questionId: attachments[0]?.questionId ?? '',
      text: '',
      monitoringObserverId: attachments[0]?.monitoringObserverId ?? '',
      timeSubmitted: attachments[0]?.timeSubmitted ?? '',
      attachments,
    }));

  const extraData = [
    ...notes.map((note) => {
      return {
        ...note,
        attachments: groupedAttachments[note.timeSubmitted] ?? [],
      };
    }),
    ...mediaFiles,
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
      {expanded && <QuestionExtraDataTable data={extraData} />}
    </>
  );
}
