import { Button } from '@/components/ui/button';

export interface EditFormFooterProps {
  onSaveProgress: () => void;
  onSaveAndExit: () => void;
}

function EditFormFooter({ onSaveProgress, onSaveAndExit }: EditFormFooterProps) {
  return (
    <footer className="fixed left-0 bottom-0 h-[64px] w-full bg-white">
      <div className='flex justify-end items-center h-full container gap-4'>
        <Button type='button' variant='outline' onClick={onSaveProgress}>Save</Button>
        <Button type='button' variant='default' onClick={onSaveAndExit}>Save and exit form editor</Button>
      </div>
    </footer>
  )
}


export default EditFormFooter
