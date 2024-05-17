import type { CellContext } from '@tanstack/react-table';
import type { FunctionComponent } from '@/common/types';
import { Dialog, DialogContent, DialogTrigger } from '@/components/ui/dialog';
import type { QuestionExtraData } from '../../types';

export function MediaFilesCell({ row }: CellContext<QuestionExtraData, unknown>): FunctionComponent {
  return (
    <div className='flex gap-2 flex-wrap'>
      {row.original.attachments.map((attachment) => (
        <Dialog>
          <DialogTrigger>
            <button type='button'>
              <img alt={attachment.fileName} className='w-10 h-10' src={attachment.presignedUrl} />
            </button>
          </DialogTrigger>
          <DialogContent className='max-w-5xl'>
            <div className='flex justify-center'>
              <img alt={attachment.fileName} src={attachment.presignedUrl} />
            </div>
          </DialogContent>
        </Dialog>
      ))}
    </div>
  );
}
