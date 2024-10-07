import { Separator } from '@/components/ui/separator';
import { cn } from '@/lib/utils';
import {
  AlignLeftIcon,
  EraserIcon,
  FontBoldIcon,
  FontItalicIcon,
  ListBulletIcon,
  StrikethroughIcon,
} from '@radix-ui/react-icons';
import type { Editor as TiptapEditor } from '@tiptap/core';
import CharacterCount from '@tiptap/extension-character-count';
import { Placeholder } from '@tiptap/extension-placeholder';
import { TextStyle } from '@tiptap/extension-text-style';
import { Typography } from '@tiptap/extension-typography';
import { EditorContent, useEditor } from '@tiptap/react';
import StarterKit from '@tiptap/starter-kit';
import * as React from 'react';
import { LinkBubbleMenu } from './components/bubble-menu/link-bubble-menu';
import { LinkEditPopover } from './components/link/link-edit-popover';
import ToolbarButton from './components/toolbar-button';
import { Link } from './extensions/link';
import { Selection } from './extensions/selection';
import './styles/index.css';

export interface RichTextProps extends Omit<React.HTMLAttributes<HTMLDivElement>, 'onChange'> {
  value?: string | null;
  placeholder?: string;
  disabled?: boolean;
  contentClass?: string;
  characterLimit?: number;
  onValueChange: (value: string) => void;
}

const useRichTextEditor = (props: RichTextProps) => {
  const { value, placeholder, disabled, characterLimit, onValueChange } = props;

  return useEditor({
    extensions: [
      StarterKit.configure({
        horizontalRule: false,
        codeBlock: false,
        paragraph: {
          HTMLAttributes: {
            class: 'text-node',
          },
        },
        bulletList: {
          HTMLAttributes: {
            class: 'list-node',
          },
        },
        orderedList: {
          HTMLAttributes: {
            class: 'list-node',
          },
        },
        dropcursor: {
          width: 2,
          class: 'ProseMirror-dropcursor border',
        },
      }),
      Link,
      TextStyle,
      Selection,
      Typography,
      Placeholder.configure({
        placeholder: () => placeholder || '',
      }),
      CharacterCount.configure({
        limit: characterLimit,
      }),
    ],
    editorProps: {
      attributes: {
        class: 'focus:outline-none',
      },
    },
    onUpdate: ({ editor }) => {
      const html = editor.getHTML();
      onValueChange(html);
    },
    content: value,
    editable: !disabled,
    onCreate: ({ editor }) => {
      if (value) {
        editor.chain().setContent(value).run();
      }
    },
  });
};

const Toolbar = ({ editor }: { editor: TiptapEditor }) => (
  <div className='border-b border-border p-2'>
    <div className='flex w-full flex-wrap items-center'>
      <ToolbarButton
        key='Bold'
        onClick={() => editor.chain().focus().toggleBold().run()}
        disabled={!editor.can().chain().focus().toggleBold().run()}
        isActive={editor.isActive('bold')}
        tooltip='Bold'
        aria-label='Bold'>
        <FontBoldIcon className='size-5' />
      </ToolbarButton>

      <ToolbarButton
        key='Italic'
        onClick={() => editor.chain().focus().toggleItalic().run()}
        disabled={!editor.can().chain().focus().toggleItalic().run()}
        isActive={editor.isActive('italic')}
        tooltip='Italic'
        aria-label='Italic'>
        <FontItalicIcon className='size-5' />
      </ToolbarButton>

      <ToolbarButton
        key='Strikethrough'
        onClick={() => editor.chain().focus().toggleStrike().run()}
        disabled={!editor.can().chain().focus().toggleStrike().run()}
        isActive={editor.isActive('strike')}
        tooltip='Strikethrough'
        aria-label='Strikethrough'>
        <StrikethroughIcon className='size-5' />
      </ToolbarButton>

      <ToolbarButton
        key='Clear formatting'
        onClick={() => editor.chain().focus().unsetAllMarks().run()}
        disabled={!editor.can().chain().focus().unsetAllMarks().run()}
        isActive={false}
        tooltip='Clear formatting'
        aria-label='Clear formatting'>
        <EraserIcon className='size-5' />
      </ToolbarButton>
      <Separator orientation='vertical' className='mx-2 h-7' />
      <ToolbarButton
        key='Numbered list'
        onClick={() => editor.chain().focus().toggleOrderedList().run()}
        isActive={editor.isActive('orderedList')}
        tooltip='Numbered list'
        aria-label='Numbered list'>
        <ListBulletIcon />
      </ToolbarButton>

      <ToolbarButton
        key='Bullet list'
        onClick={() => editor.chain().focus().toggleBulletList().run()}
        isActive={editor.isActive('bulletList')}
        tooltip='Bullet list'
        aria-label='Bullet list'>
        <AlignLeftIcon />
      </ToolbarButton>
      <Separator orientation='vertical' className='mx-2 h-7' />
      <LinkEditPopover editor={editor} />
    </div>
  </div>
);

export const RichTextEditor = React.forwardRef<HTMLDivElement, RichTextProps>(
  ({ value, disabled, characterLimit, contentClass, onValueChange, placeholder, className, ...props }, ref) => {
    const editor = useRichTextEditor({ value, placeholder, disabled, characterLimit, onValueChange });

    return (
      <div
        className={cn(
          'flex h-auto min-h-72 w-full flex-col rounded-md border border-input shadow-sm focus-within:border-primary',
          className
        )}
        {...props}
        ref={ref}>
        {editor && (
          <>
            <LinkBubbleMenu editor={editor} />
            <Toolbar editor={editor} />
          </>
        )}
        <div className='h-full grow' onClick={() => editor?.chain().focus().run()}>
          <EditorContent editor={editor} className={cn('minimal-tiptap-editor p-5', contentClass)} />
        </div>
      </div>
    );
  }
);

RichTextEditor.displayName = 'RichTextEditor';

export default RichTextEditor;
