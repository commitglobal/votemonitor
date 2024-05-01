import { ChevronDownIcon, ChevronUpIcon } from '@heroicons/react/24/outline';
import { useState } from 'react';
import type { FunctionComponent } from '@/common/types';
import { Button } from '@/components/ui/button';
import type { QuestionExtraData } from '../../types';
import { QuestionExtraDataTable } from '../QuestionExtraDataTable/QuestionExtraDataTable';

type QuestionExtraDataSectionProps = {
  attachmentCount: number;
  extraData: QuestionExtraData[];
  notesCount: number;
};

export function QuestionExtraDataSection({
  attachmentCount,
  extraData,
  notesCount,
}: QuestionExtraDataSectionProps): FunctionComponent {
  const [expanded, setExpanded] = useState(false);

  return (
    <>
      <Button
        className='text-purple-500 hover:no-underline max-w-fit pl-0'
        onClick={() => {
          setExpanded((prev) => !prev);
        }}
        variant='link'>
        {notesCount} notes & {attachmentCount} media files{' '}
        {expanded ? <ChevronUpIcon className='w-4 ml-2' /> : <ChevronDownIcon className='w-4 ml-2' />}
      </Button>
      {expanded && <QuestionExtraDataTable data={extraData} />}
    </>
  );
}
