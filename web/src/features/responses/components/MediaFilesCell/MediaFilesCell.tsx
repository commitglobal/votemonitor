import { AspectRatio } from '@/components/ui/aspect-ratio';
import { Button } from '@/components/ui/button';
import { Dialog, DialogContent, DialogTrigger } from '@/components/ui/dialog';
import { cn, getFileCategory } from '@/lib/utils';
import { ArrowDownTrayIcon, DocumentIcon, FilmIcon, MusicalNoteIcon, PhotoIcon } from '@heroicons/react/24/outline';
import { useMemo } from 'react';
import ReactPlayer from 'react-player';
import { Attachment } from '../../models/common';

export interface MediaFilesCellProps {
  attachment: Attachment;
  className?: string;
}

export default function MediaFilesCell({ attachment, className }: MediaFilesCellProps) {
  const attachmentType = useMemo(() => {
    return getFileCategory(attachment.mimeType);
  }, [attachment.mimeType]);

  // const handleDownload = async (e: React.MouseEvent) => {
  //   e.stopPropagation();
  //   try {
  //     // throw new Error("uncomment this line to mock failure of API");
  //     const response = await axios.get(attachment.presignedUrl, {
  //       responseType: 'blob',
  //       headers: {
  //         'Access-Control-Allow-Origin': '*',
  //       },
  //     });

  //     // Create download link
  //     const url = window.URL.createObjectURL(new Blob([response.data]));
  //     const link = document.createElement('a');
  //     link.href = url;
  //     link.download = attachment.fileName;
  //     document.body.appendChild(link);
  //     link.click();
  //     document.body.removeChild(link);
  //     window.URL.revokeObjectURL(url);
  //   } catch (error) {
  //     console.error('Download failed:', error);
  //   }
  // };

  const renderPreview = (isInDialog = false) => {
    const baseClasses = 'object-cover rounded-md transition-opacity duration-200';
    const dialogClasses = isInDialog ? 'cursor-default' : 'cursor-zoom-in hover:opacity-90';

    switch (attachmentType) {
      case 'image':
        return (
          <img
            alt={attachment.fileName}
            src={attachment.presignedUrl}
            className={cn(baseClasses, dialogClasses)}
            loading='lazy'
            style={{ height: '100%', width: '100%' }}
          />
        );

      case 'video':
        return (
          <div className={cn('relative bg-black rounded-md overflow-hidden', dialogClasses)}>
            <ReactPlayer
              url={attachment.presignedUrl}
              controls={true}
              width='100%'
              height='100%'
              playing={isInDialog}
            />
            {!isInDialog && (
              <div className='absolute inset-0 flex items-center justify-center bg-black bg-opacity-30'>
                <FilmIcon className='w-8 h-8 text-white' />
              </div>
            )}
          </div>
        );

      case 'audio':
        return (
          <div className={cn('flex items-center justify-center p-4 border rounded-md bg-gray-50', dialogClasses)}>
            <MusicalNoteIcon className='w-8 h-8 mr-3 text-gray-600' />
            <div className='flex-1'>
              <ReactPlayer url={attachment.presignedUrl} controls={true} width='100%' height='50px' />
            </div>
          </div>
        );

      default:
        return (
          <div
            className={cn(
              'flex flex-col items-center justify-center p-6 border-2 border-dashed border-gray-300 rounded-md bg-gray-50 hover:bg-gray-100 transition-colors',
              dialogClasses
            )}>
            <DocumentIcon className='w-12 h-12 mb-2 text-gray-500' />
            <span className='text-sm font-medium text-gray-700 text-center break-all'>{attachment.fileName}</span>
            <span className='text-xs text-gray-500 mt-1'>{attachment.mimeType}</span>
          </div>
        );
    }
  };

  const getFileIcon = () => {
    switch (attachmentType) {
      case 'image':
        return <PhotoIcon className='w-4 h-4' />;
      case 'video':
        return <FilmIcon className='w-4 h-4' />;
      case 'audio':
        return <MusicalNoteIcon className='w-4 h-4' />;
      default:
        return <DocumentIcon className='w-4 h-4' />;
    }
  };

  return (
    <Dialog>
      <DialogTrigger asChild>
        <div className={cn('group relative', className)}>
          <AspectRatio ratio={16 / 9}>{renderPreview(false)}</AspectRatio>

          {/* Overlay on hover for non-dialog state */}
          {attachmentType !== 'audio' && (
            <div className='absolute inset-0 flex items-center justify-center bg-black bg-opacity-0 group-hover:bg-opacity-10 transition-all rounded-md'>
              <div className='opacity-0 group-hover:opacity-100 transition-opacity'>{getFileIcon()}</div>
            </div>
          )}
        </div>
      </DialogTrigger>

      <DialogContent className='max-w-[90vw] max-h-[90vh] w-auto p-0 border-0 bg-transparent'>
        <div className='flex flex-col items-center justify-center w-full h-full'>
          <div className='relative bg-white rounded-lg overflow-hidden shadow-2xl w-full max-w-4xl'>
            {/* Preview content */}
            <div className='flex items-center justify-center w-full h-full bg-gray-100 p-8 max-h-[70vh] overflow-auto'>
              {renderPreview(true)}
            </div>

            {/* Close button footer */}
            <div className='flex items-center justify-center p-4 bg-white border-t'>
              <div className='flex-1 min-w-0'>
                <p className='text-lg font-semibold text-gray-900 truncate'>{attachment.fileName}</p>
                <p className='text-sm text-gray-500 capitalize mt-1'>
                  {attachmentType} • {attachment.mimeType}
                </p>
              </div>

              <Button
                // onClick={handleDownload}
                variant='outline'
                size='sm'
                className='flex items-center gap-2 shrink-0 ml-4'>
                <ArrowDownTrayIcon className='w-4 h-4' />
                <a href={attachment.presignedUrl} target='_blank' rel='noopener noreferrer'>
                  Download
                </a>
              </Button>
            </div>
          </div>
        </div>
      </DialogContent>
    </Dialog>
  );
}
