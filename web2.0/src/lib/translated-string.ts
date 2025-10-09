import { TranslatedString } from '@/types/common'

export function getTranslatedStringOrDefault(
  translatedString: TranslatedString,
  defaultLanguage: string,
  language?: string
) {
  if (language) {
    return translatedString[language] ?? translatedString[defaultLanguage]
  }
  return translatedString[defaultLanguage]
}
