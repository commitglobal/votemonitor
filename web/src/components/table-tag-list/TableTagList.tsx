import { stringToText } from '@/lib/utils';
import { Badge } from '../ui/badge';

interface TableTagListProps {
  tags: string[];
}

export default function TableTagList({ tags }: TableTagListProps) {
  return (
    <div className='flex flex-row gap-2'>
      {tags.map((tag) => (
        <Badge key={tag} style={{ backgroundColor: stringToText(tag) }}>{tag}</Badge>
      ))}
    </div>
  );
}
