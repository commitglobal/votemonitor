import type { Editor } from '@tiptap/core'
import type { EditorView } from '@tiptap/pm/view'
import type { EditorState } from '@tiptap/pm/state'

export enum LinkType {
  URL = 'url',
  EMAIL = 'email',
  TELEPHONE = 'telephone'
}

export interface LinkProps {
  url: string
  text?: string
  type: LinkType
}

export interface ShouldShowProps {
  editor: Editor
  view: EditorView
  state: EditorState
  oldState?: EditorState
  from: number
  to: number
}
