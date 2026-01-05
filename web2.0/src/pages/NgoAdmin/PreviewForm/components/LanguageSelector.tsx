import { Languages } from 'lucide-react'
import { Button } from '@/components/ui/button'
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from '@/components/ui/dropdown-menu'
import { mapLanguageNameByCode } from '@/lib/i18n'

interface LanguageSelectorProps {
  languages: string[]
  currentLanguage: string
  onLanguageChange: (language: string) => void
}

export function LanguageSelector({
  languages,
  currentLanguage,
  onLanguageChange,
}: LanguageSelectorProps) {
  if (languages.length <= 1) {
    return null
  }

  return (
    <DropdownMenu>
      <DropdownMenuTrigger asChild>
        <Button variant='outline' className='gap-2'>
          <Languages className='h-5 w-5' />
          <span>{mapLanguageNameByCode(currentLanguage)}</span>
        </Button>
      </DropdownMenuTrigger>
      <DropdownMenuContent align='end' className='w-48'>
        {languages.map((language) => (
          <DropdownMenuItem
            key={language}
            onClick={() => onLanguageChange(language)}
            className='flex cursor-pointer items-center gap-2'
          >
            <span className='flex-1'>{mapLanguageNameByCode(language)}</span>
            {currentLanguage === language && (
              <span className='text-primary'>✓</span>
            )}
          </DropdownMenuItem>
        ))}
      </DropdownMenuContent>
    </DropdownMenu>
  )
}

