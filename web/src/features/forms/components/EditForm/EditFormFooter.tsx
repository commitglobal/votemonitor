import { Button } from '@/components/ui/button'

export interface EditFormFooterProps {
}

function EditFormFooter({ }: EditFormFooterProps) {
  return (
    <footer className="fixed left-0 bottom-0 h-[64px] w-full bg-white">
      <div className='flex justify-end items-center h-full container'>
        <Button type='submit' variant='default'>Save</Button>
      </div>
    </footer>
  )
}


export default EditFormFooter
