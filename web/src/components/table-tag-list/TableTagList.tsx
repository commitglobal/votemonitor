import { stringToText } from '@/lib/utils';
import React from 'react';

interface TableTagListProps {
  tags: string[];
}

export default function TableTagList({ tags }: TableTagListProps) {
  return (
    <div className='flex flex-row gap-2'>
      {tags.map((tag) => (
        <span
          key={tag}
          style={{
            backgroundColor: stringToText(tag),
            filter: 'brightness(130%)',
            color: '#000',
          }}
          className='rounded-full py-0 px-2.5'>
          {tag}
        </span>
      ))}
    </div>
  );
}
