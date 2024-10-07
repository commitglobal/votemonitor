import type { Editor } from '@tiptap/core'
import * as React from 'react'
import { Button } from '@/components/ui/button'
import { Label } from '@/components/ui/label'
import { Input } from '@/components/ui/input'
import { LinkProps, LinkType } from '../../types'
import { cn } from '@/lib/utils'
import { Toggle } from '@/components/ui/toggle'
import { Link2Icon, MobileIcon, EnvelopeClosedIcon } from '@radix-ui/react-icons'

interface LinkEditBlockProps extends React.HTMLAttributes<HTMLDivElement> {
  editor: Editor
  onSetLink: ({ url, text, type }: LinkProps) => void
  close?: () => void
}

const LinkEditBlock = ({ editor, onSetLink, close, className, ...props }: LinkEditBlockProps) => {
  const formRef = React.useRef<HTMLDivElement>(null)

  const [field, setField] = React.useState<LinkProps>({
    url: '',
    text: '',
    type: LinkType.URL
  })

  const data = React.useMemo(() => {
    const { href } = editor.getAttributes('link')
    const { from, to } = editor.state.selection
    const text = editor.state.doc.textBetween(from, to, ' ')

    const isTelephone = href?.startsWith('tel:');
    const isEmail = href?.startsWith('mailto:');

    let url: string = href;

    if (isTelephone) {
      url = url.replace('tel:', '')
    }

    if (isEmail) {
      url = url.replace('mailto:', '')
    }

    return {
      url,
      text,
      type: isTelephone ? LinkType.TELEPHONE : isEmail ? LinkType.EMAIL : LinkType.URL
    }
  }, [editor])

  React.useEffect(() => {
    console.log('effect');
    setField(data)
  }, [data])

  const handleClick = (e: React.FormEvent) => {
    e.preventDefault()

    if (formRef.current) {
      const isValid = Array.from(formRef.current.querySelectorAll('input')).every(input => input.checkValidity())

      if (isValid) {
        onSetLink(field)
        close?.()
      } else {
        formRef.current.querySelectorAll('input').forEach(input => {
          if (!input.checkValidity()) {
            input.reportValidity()
          }
        })
      }
    }
  }

  return (
    <div ref={formRef}>
      <div className={cn('space-y-4', className)} {...props}>
        <div>
          <Toggle aria-label="Url" onPressedChange={() => setField({ ...field, type: LinkType.URL })} pressed={field.type === LinkType.URL}>
            <Link2Icon className="h-4 w-4" />
          </Toggle>
          <Toggle aria-label="Email" onPressedChange={() => setField({ ...field, type: LinkType.EMAIL })} pressed={field.type === LinkType.EMAIL} >
            <EnvelopeClosedIcon className="h-4 w-4" />
          </Toggle>
          <Toggle aria-label="Telephone" onPressedChange={() => setField({ ...field, type: LinkType.TELEPHONE })} pressed={field.type === LinkType.TELEPHONE}>
            <MobileIcon className="h-4 w-4" />
          </Toggle>
        </div>
        <div className="space-y-1">
          <Label>{field.type === LinkType.EMAIL ? 'Email' : field.type === LinkType.URL ? 'Link' : 'Telephone'}</Label>
          <Input
            type={field.type === LinkType.EMAIL ? 'email' : field.type === LinkType.URL ? 'url' : 'tel'}
            required
            placeholder={field.type === LinkType.EMAIL ? 'Paste an email' : field.type === LinkType.URL ? 'Paste a link' : 'Paste a phone number'}
            value={field.url ?? ''}
            onChange={e => setField({ ...field, url: e.target.value })}
          />
        </div>

        <div className="space-y-1">
          <Label>Display text (optional)</Label>
          <Input
            type="text"
            placeholder="Text to display"
            value={field.text ?? ''}
            onChange={e => setField({ ...field, text: e.target.value })}
          />
        </div>

        <div className="flex justify-end space-x-2">
          {close && (
            <Button variant="ghost" type="button" onClick={close}>
              Cancel
            </Button>
          )}

          <Button type="button" onClick={handleClick}>
            Insert
          </Button>
        </div>
      </div>
    </div>
  )
}

export { LinkEditBlock }
