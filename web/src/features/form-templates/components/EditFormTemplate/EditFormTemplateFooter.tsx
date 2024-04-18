import { Button } from '@/components/ui/button'
import React from 'react'

export interface EditFormTemplateFooterProps {
}

function EditFormTemplateFooter({ }: EditFormTemplateFooterProps) {
  return (
    <footer className="fixed left-0 bottom-0 h-[100px] w-full bg-white">
      <div className='flex justify-end items-center h-full container'>
        <Button type='submit' variant='default'>Save</Button>
      </div>
    </footer>
  )
}


export default EditFormTemplateFooter
