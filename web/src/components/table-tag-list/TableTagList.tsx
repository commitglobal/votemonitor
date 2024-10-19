import { getTagColor } from '@/lib/utils';
import { FC } from 'react';
import { Badge } from '../ui/badge';
import { Tooltip, TooltipContent, TooltipProvider, TooltipTrigger } from '../ui/tooltip';

interface TableTagListProps {
  tags: string[];
}

const TableTag: FC<{ tag: string }> = ({ tag }) => {
  return (
    <Badge key={tag} style={{ backgroundColor: getTagColor(tag) }} className='text-slate-600'>
      {tag}
    </Badge>
  );
};

export default function TableTagList({ tags }: TableTagListProps) {
  const firstTag = tags[0] as string;
  const remainingTags = tags.slice(1);

  if (tags.length == 0) return;

  if (tags.length == 1) return <TableTag tag={firstTag} />;

  return (
    <TooltipProvider>
      <div className='flex flex-row flex-wrap gap-2'>
        <TableTag tag={firstTag} />

        <Tooltip>
          <TooltipTrigger>+{tags.length - 1}</TooltipTrigger>
          <TooltipContent>
            {remainingTags.map((tag) => (
              <p>{tag}</p>
            ))}
          </TooltipContent>
        </Tooltip>
      </div>
    </TooltipProvider>
  );
}
