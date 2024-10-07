import type { Editor } from '@tiptap/core'
import { LinkProps, LinkType } from './types'


export function getOutput(editor: Editor, format: string) {
  if (format === 'json') {
    return JSON.stringify(editor.getJSON())
  }

  if (format === 'html') {
    return editor.getText() ? String(editor.getHTML()) : ''
  }

  return editor.getText()
}

export function setLink(editor: Editor, { url, text, type }: LinkProps) {
  editor
    .chain()
    .extendMarkRange('link')
    .insertContent({
      type: 'text',
      text: text || url,
      marks: [
        {
          type: 'link',
          attrs: {
            href: type === LinkType.EMAIL ? 'mailto:' + url : type === LinkType.TELEPHONE ? 'tel:' + url : url,
          }
        }
      ]
    })
    .setLink({ href: url })
    .focus()
    .run()
}
