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

/**
 * Creates a new Translated String containing all available languages
 * @param availableLanguages available translations list
 * @param languageCode language code for which to add value
 * @param value value to set for required languageCode
 * @returns new instance of @see {@link TranslatedString}
 */
export const newTranslatedString = (
  availableLanguages: string[],
  languageCode: string,
  value: string = ''
): TranslatedString => {
  const translatedString: TranslatedString = {}
  availableLanguages.forEach((language) => {
    translatedString[language] = ''
  })

  translatedString[languageCode] = value

  return translatedString
}
