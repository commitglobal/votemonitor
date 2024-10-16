import type { FunctionComponent } from '@/common/types';
import { Button } from '@/components/ui/button';
import { Dialog, DialogContent, DialogTrigger } from '@/components/ui/dialog';
import { FilmIcon, MusicalNoteIcon, PaperClipIcon, PhotoIcon } from '@heroicons/react/24/outline';
import { useMemo } from 'react';
import ReactPlayer from 'react-player';
import { Attachment } from '../../models/common';
import { getFileCategory } from '@/lib/utils';

export interface MediaFilesCellProps {
  attachment: Attachment;
}

export function MediaFilesCell({ attachment }: MediaFilesCellProps): FunctionComponent {
  const attachmentType = useMemo(() => {
    return getFileCategory(attachment.mimeType)
  }, [attachment.mimeType]);

  return (
    <div className='flex flex-wrap gap-2'>
      <Dialog>
        <DialogTrigger asChild>
          <Button type='button' variant='link' className='text-purple-900 hover:text-purple-600'>
            {attachmentType === 'image' ? <PhotoIcon className='w-10 h-10' />
              : attachmentType === 'video' ? <FilmIcon className='w-10 h-10' />
                : attachmentType === 'audio' ? <MusicalNoteIcon className='w-10 h-10' /> : <PaperClipIcon className='w-10 h-10' />}
          </Button>
        </DialogTrigger>
        <DialogContent className='max-w-5xl'>
          <div className='flex justify-center'>
            {attachmentType === 'image'
              ? <img alt={attachment.fileName} src={attachment.presignedUrl} />
              : <ReactPlayer alt={attachment.fileName} url={attachment.presignedUrl} controls={true}/>}
          </div>
        </DialogContent>
      </Dialog>
    </div>
  );
}
