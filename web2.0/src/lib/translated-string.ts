import { TranslatedString } from '@/types/common'
import { Language } from '@/types/language'

export function getTranslatedStringOrDefault(
  translatedString: TranslatedString | undefined,
  defaultLanguage: Language,
  language?: Language
) {
  if (!translatedString) {
    return ''
  }
  
  if (language) {
    return translatedString[language] ?? translatedString[defaultLanguage]
  }
  return translatedString[defaultLanguage]
}
export function getTranslation(
  translatedString: TranslatedString,
  language: Language
) {
  if (translatedString[language]) {
    return translatedString[language]
  }
  return translatedString[Object.keys(translatedString)[0]]
}

/**
 * Creates a new Translated String containing all available languages
 * @param availableLanguages available translations list
 * @param languageCode language code for which to add value
 * @param value value to set for required languageCode
 * @returns new instance of @see {@link TranslatedString}
 */
export const newTranslatedString = (
  availableLanguages: Language[],
  languageCode: Language,
  value: string = ''
): TranslatedString => {
  const translatedString: TranslatedString = {}
  availableLanguages.forEach((language) => {
    translatedString[language] = ''
  })

  translatedString[languageCode] = value

  return translatedString
}
