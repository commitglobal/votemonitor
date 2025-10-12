'use client'

import * as React from 'react'
import { CheckIcon, ClipboardIcon } from 'lucide-react'
import { cn } from '@/lib/utils'
import { Button } from './button'
import { Tooltip, TooltipContent, TooltipTrigger } from './tooltip'

export function copyToClipboardWithMeta(value: string) {
  navigator.clipboard.writeText(value)
}

export function CopyButton({
  value,
  className,
  variant = 'ghost',
  ...props
}: React.ComponentProps<typeof Button> & {
  value: string
}) {
  const [hasCopied, setHasCopied] = React.useState(false)

  React.useEffect(() => {
    setTimeout(() => {
      setHasCopied(false)
    }, 2000)
  }, [hasCopied])

  return (
    <Tooltip>
      <TooltipTrigger asChild>
        <Button
          size='icon'
          variant='ghost'
          className={cn(
            '[&_svg]-h-3.5 h-7 w-7 rounded-[6px] [&_svg]:w-3.5',
            className
          )}
          onClick={() => {
            navigator.clipboard.writeText(value)

            setHasCopied(true)
          }}
          {...props}
        >
          <span className='sr-only'>Copy</span>
          {hasCopied ? <CheckIcon /> : <ClipboardIcon />}
        </Button>
      </TooltipTrigger>
      <TooltipContent className='bg-black text-white'>Copy code</TooltipContent>
    </Tooltip>
  )
}
