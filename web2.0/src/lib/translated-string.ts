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
export function getTranslation(
  translatedString: TranslatedString,
  language: string
) {
  if (translatedString[language]) {
    return translatedString[language]
  }
  return translatedString[Object.keys(translatedString)[0]]
}
