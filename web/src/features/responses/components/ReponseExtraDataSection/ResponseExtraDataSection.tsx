import type { FunctionComponent } from '@/common/types';
import { Button } from '@/components/ui/button';
import { ChevronDownIcon, ChevronUpIcon } from '@heroicons/react/24/outline';
import { useMemo, useState } from 'react';
import { ResponseExtraDataTable } from '../ResponseExtraDataTable/ResponseExtraDataTable';
import { QuestionExtraData } from '../../types';
import { aggregatedAnswerExtraInfoColumnDefs, answerExtraInfoColumnDefs } from '../../utils/column-defs';
import { Note, Attachment } from '../../models/common';

type ResponseExtraDataSectionProps = {
  aggregateDisplay: boolean;
  attachments: Attachment[];
  notes: Note[];
};

export function ResponseExtraDataSection({ attachments, notes, aggregateDisplay = false }: ResponseExtraDataSectionProps): FunctionComponent {
  const [expanded, setExpanded] = useState(false);

  const columns = useMemo(() => {
    if (aggregateDisplay) {
      return aggregatedAnswerExtraInfoColumnDefs
    }

    return answerExtraInfoColumnDefs;
  }, [aggregateDisplay]);

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
      {expanded && <ResponseExtraDataTable columns={columns} data={extraData} />}
    </>
  );
}
