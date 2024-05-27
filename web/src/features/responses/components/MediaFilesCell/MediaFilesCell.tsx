import type { FunctionComponent } from '@/common/types';
import { Dialog, DialogContent, DialogTrigger } from '@/components/ui/dialog';
import { FilmIcon, MusicalNoteIcon, PaperClipIcon, PhotoIcon } from '@heroicons/react/24/outline';
import { useMemo } from 'react';
import ReactPlayer from 'react-player';
import { Attachment } from '../../models/form-submission';

export interface MediaFilesCellProps {
  attachment: Attachment
}

const getFileCategory = (mimeType: string): 'image' | 'video' | 'audio' | 'unknown' => {
  if (mimeType.startsWith('image/')) {
    return 'image'
  } else if (mimeType.startsWith('video/')) {
    return 'video'
  } else if (mimeType.startsWith('audio/')) {
    return 'audio'
  } else {
    return 'unknown'
  }
};

export function MediaFilesCell({ attachment }: MediaFilesCellProps): FunctionComponent {
  const attachmentType = useMemo(() => {
    return getFileCategory(attachment.mimeType)
  }, [attachment.mimeType]);

  return (
    <div className='flex gap-2 flex-wrap'>
      <Dialog>
        <DialogTrigger asChild>
          <button type='button'>
            {attachmentType === 'image' ? <PhotoIcon className='w-10 h-10' />
              : attachmentType === 'video' ? <FilmIcon className='w-10 h-10' />
                : attachmentType === 'audio' ? <MusicalNoteIcon className='w-10 h-10' /> : <PaperClipIcon className='w-10 h-10' />}
          </button>
        </DialogTrigger>
        <DialogContent className='max-w-5xl'>
          <div className='flex justify-center'>
            {attachmentType === 'image'
              ? <img alt={attachment.fileName} src={attachment.presignedUrl} />
              : <ReactPlayer alt={attachment.fileName} url={attachment.presignedUrl} />}
          </div>
        </DialogContent>
      </Dialog>
    </div>
  );
}
