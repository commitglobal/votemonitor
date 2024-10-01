import { FormType, RatingScaleType, TranslatedString, UserPayload, ZFormType } from '@/common/types';
import i18n from '@/i18n';
import { redirect } from '@tanstack/react-router';
import { type ClassValue, clsx } from 'clsx';
import { twMerge } from 'tailwind-merge';

export function cn(...inputs: ClassValue[]) {
  return twMerge(clsx(inputs));
}

export function valueOrDefault(value: number | null | undefined, fallbackValue: number): number {
  if (value === null || value === undefined || isNaN(value)) {
    return fallbackValue;
  }

  return value;
}

// https://colorhunt.co/palettes/pastel
const colors = [
  '#618264',
  '#79ac78',
  '#b0d9b1',
  '#d0e7d2',
  '#ecee81',
  '#8ddfcb',
  '#82a0d8',
  '#edb7ed',
  '#ef9595',
  '#efb495',
  '#efd595',
  '#ebef95',
  '#94a684',
  '#aec3ae',
  '#e4e4d0',
  '#ffeef4',
  '#fff3da',
  '#dfccfb',
  '#d0bfff',
  '#beadfa',
  '#96b6c5',
  '#adc4ce',
  '#eee0c9',
  '#f1f0e8',
  '#c8e4b2',
  '#9ed2be',
  '#7eaa92',
  '#ffd9b7',
  '#ffc6ac',
  '#fff6dc',
  '#c4c1a4',
  '#9e9fa5',
  '#faf3f0',
  '#d4e2d4',
  '#ffcacc',
  '#dbc4f0',
  '#a1ccd1',
  '#f4f2de',
  '#e9b384',
  '#7c9d96',
  '#aac8a7',
  '#c3edc0',
  '#e9ffc2',
  '#fdffae',
  '#ff9b9b',
  '#ffd6a5',
  '#fffec4',
  '#cbffa9',
  '#f1c27b',
  '#ffd89c',
  '#a2cdb0',
  '#85a389',
  '#a0c49d',
  '#c4d7b2',
  '#e1ecc8',
  '#f7ffe5',
  '#c2dedc',
  '#ece5c7',
  '#cdc2ae',
  '#116a7b',
  '#9babb8',
  '#eee3cb',
  '#d7c0ae',
  '#967e76',
  '#f2d8d8',
  '#5c8984',
  '#545b77',
  '#374259',
  '#f9f5f6',
  '#f8e8ee',
  '#fdcedf',
  '#f2bed1',
  '#c4dfdf',
  '#d2e9e9',
  '#e3f4f4',
  '#f8f6f4',
  '#f5f0bb',
  '#dbdfaa',
  '#b3c890',
  '#73a9ad',
  '#537188',
  '#cbb279',
  '#e1d4bb',
  '#eeeeee',
  '#8294c4',
  '#acb1d6',
  '#dbdfea',
  '#ffead2',
  '#bfccb5',
  '#7c96ab',
  '#b7b7b7',
  '#edc6b1',
  '#fdf4f5',
  '#e8a0bf',
  '#ba90c6',
  '#c0dbea',
  '#ddffbb',
  '#c7e9b0',
  '#b3c99c',
  '#a4bc92',
  '#b2a4ff',
  '#ffb4b4',
  '#ffdeb4',
  '#fdf7c3',
  '#fff2cc',
  '#ffd966',
  '#f4b183',
  '#dfa67b',
  '#d5b4b4',
  '#e4d0d0',
  '#f5ebeb',
  '#bbd6b8',
  '#aec2b6',
  '#94af9f',
  '#dbe4c6',
  '#ccd5ae',
  '#e9edc9',
  '#fefae0',
  '#faedcd',
  '#a86464',
  '#b3e5be',
  '#f5ffc9',
  '#f7c8e0',
  '#dfffd8',
  '#b4e4ff',
  '#95bdff',
  '#b9f3e4',
  '#ea8fea',
  '#ffaacf',
  '#f6e6c2',
  '#b5f1cc',
  '#e5fdd1',
  '#c9f4aa',
  '#fcc2fc',
  '#6096b4',
  '#93bfcf',
  '#bdcdd6',
  '#eee9da',
  '#a7727d',
  '#eddbc7',
  '#f8ead8',
  '#f9f5e7',
  '#aae3e2',
  '#d9acf5',
  '#ffcefe',
  '#fdebed',
  '#b9f3fc',
  '#aee2ff',
  '#93c6e7',
  '#fedeff',
  '#7286d3',
  '#8ea7e9',
  '#e5e0ff',
  '#fff2f2',
  '#eac7c7',
  '#a0c3d2',
  '#f7f5eb',
  '#eae0da',
];

export function getTagColor(tag: string) {
  tag = tag.toLocaleLowerCase();
  let hash = 0;
  for (let i = 0; i < tag.length; i++) {
    hash = tag.charCodeAt(i) + ((hash << 5) - hash);
  }

  return colors[Math.abs(hash) % colors.length];
}

export function redirectIfNotAuth(): void {
  const token = localStorage.getItem('token');
  if (!token) {
    throw redirect({
      to: '/login',
    });
  }
}

export function parseJwt(token: string | undefined): UserPayload {
  let base64Url: string | undefined = token!.split('.')[1];
  let base64 = base64Url!.replace(/-/g, '+').replace(/_/g, '/');
  let jsonPayload = decodeURIComponent(
    window
      .atob(base64)
      .split('')
      .map(function (c) {
        return '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2);
      })
      .join('')
  );

  return JSON.parse(jsonPayload) as UserPayload;
}

export function ratingScaleToNumber(scale: RatingScaleType): number {
  switch (scale) {
    case RatingScaleType.OneTo3: {
      return 3;
    }
    case RatingScaleType.OneTo4: {
      return 4;
    }
    case RatingScaleType.OneTo5: {
      return 5;
    }
    case RatingScaleType.OneTo6: {
      return 6;
    }
    case RatingScaleType.OneTo7: {
      return 7;
    }
    case RatingScaleType.OneTo8: {
      return 8;
    }
    case RatingScaleType.OneTo9: {
      return 9;
    }
    case RatingScaleType.OneTo10: {
      return 10;
    }
    default: {
      return 5;
    }
  }
}

export function buildURLSearchParams(data: any) {
  const params = new URLSearchParams();

  Object.entries(data).forEach(([key, value]) => {
    if (Array.isArray(value)) {
      // @ts-ignore
      value.forEach((value) => params.append(key, value.toString()));
    } else {
      // @ts-ignore
      params.append(key, value.toString());
    }
  });

  return params;
}

export function round(value: number, decimals: number): number {
  //@ts-ignore
  return Number(Math.round(value + 'e' + decimals) + 'e-' + decimals);
}

export function isQueryFiltered(queryParams: Record<string, string>): boolean {
  return Object.entries(queryParams).some(
    ([key, value]) => !['PageNumber', 'PageSize', 'SortColumnName', 'SortOrder'].includes(key) && Boolean(value)
  );
}

export const isNotNilOrWhitespace = (input?: string | null) => (input?.trim()?.length || 0) > 0;

export const isNilOrWhitespace = (input?: string | null) => (input?.trim()?.length || 0) === 0;

export function takewhile<T>(arr: T[], predicate: (value: T) => boolean): T[] {
  const result: T[] = [];
  for (let i = 0; i < arr.length; i++) {
    if (!predicate(arr[i]!)) {
      break;
    }
    result.push(arr[i]!);
  }
  return result;
}

export function mapFormType(formType: FormType): string {
  switch (formType) {
    case ZFormType.Values.Opening:
      return i18n.t('formType.opening');
    case ZFormType.Values.Voting:
      return i18n.t('formType.voting');
    case ZFormType.Values.ClosingAndCounting:
      return i18n.t('formType.closingAndCounting');
    case ZFormType.Values.CitizenReporting:
      return i18n.t('formType.citizenReporting');
    case ZFormType.Values.IncidentReporting:
        return i18n.t('formType.incidentReporting');
    case ZFormType.Values.PSI:
      return i18n.t('formType.psi');
    case ZFormType.Values.Other:
      return i18n.t('formType.other');
    default:
      return 'Unknown';
  }
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
  const translatedString: TranslatedString = {};
  availableLanguages.forEach((language) => {
    translatedString[language] = '';
  });

  translatedString[languageCode] = value;

  return translatedString;
};

/**
 * Creates a new Translated String containing all available languages
 * @param availableLanguages available translations list
 * @param value value to set for required languageCode
 * @returns new instance of @see {@link TranslatedString}
 */
export const emptyTranslatedString = (availableLanguages: string[], value: string = ''): TranslatedString => {
  const translatedString: TranslatedString = {};
  availableLanguages.forEach((language) => {
    translatedString[language] = value;
  });

  return translatedString;
};

export const updateTranslationString = (
  translatedString: TranslatedString | undefined,
  availableLanguages: string[],
  languageCode: string,
  value: string
): TranslatedString => {
  if (translatedString === undefined) {
    translatedString = newTranslatedString(availableLanguages, languageCode);
  }

  translatedString[languageCode] = value;

  return translatedString;
};

/**
 * Clones translation from a language code to a language code in @see {@link TranslatedString} instance
 * @param translatedString a instance of @see {@link TranslatedString}
 * @param fromLanguageCode language code from which to borrow translation
 * @param toLanguageCode destination
 * @param defaultValue default value
 * @returns new instance of @see {@link TranslatedString}
 */
export const cloneTranslation = (
  translatedString: TranslatedString | undefined,
  fromLanguageCode: string,
  toLanguageCode: string,
  defaultValue: string = ''
): TranslatedString | undefined => {
  if (translatedString) {
    translatedString[toLanguageCode] = translatedString[fromLanguageCode] ?? defaultValue;
  }

  return translatedString;
};

/**
 * Changes language code to another in @see {@link TranslatedString} instance
 * @param translatedString a instance of @see {@link TranslatedString}
 * @param fromLanguageCode language code from which to borrow translation
 * @param toLanguageCode destination
 * @param defaultValue default value
 * @returns new instance of @see {@link TranslatedString}
 */
export const changeLanguageCode = (
  translatedString: TranslatedString | undefined,
  fromLanguageCode: string,
  toLanguageCode: string,
  defaultValue: string = ''
): TranslatedString => {
  if (translatedString === undefined) {
    return {};
  }

  const text = translatedString[fromLanguageCode];
  delete translatedString[fromLanguageCode];

  return {
    ...translatedString,
    [toLanguageCode]: text ?? defaultValue,
  };
};

/**
 * Gets translation from a translated string.
 * If translation string is undefined or it does not contain translation for the requested language code then it will return a default value
 * @param translatedString a instance of @see {@link TranslatedString}
 * @param languageCode language code for which to get translation
 * @param value value to set for required languageCode
 * @returns translation or a default value
 */
export const getTranslationOrDefault = (
  translatedString: TranslatedString | undefined,
  languageCode: string,
  value: string = ''
): string => {
  if (translatedString === undefined) {
    return value;
  }

  const translation = translatedString[languageCode];
  if (translation === undefined) {
    return value;
  }

  return translation;
};

export function formatBytes(
  bytes: number,
  opts: {
    decimals?: number
    sizeType?: "accurate" | "normal"
  } = {}
) {
  const { decimals = 0, sizeType = "normal" } = opts

  const sizes = ["Bytes", "KB", "MB", "GB", "TB"]
  const accurateSizes = ["Bytes", "KiB", "MiB", "GiB", "TiB"]
  if (bytes === 0) return "0 Byte"
  const i = Math.floor(Math.log(bytes) / Math.log(1024))
  return `${(bytes / Math.pow(1024, i)).toFixed(decimals)} ${
    sizeType === "accurate" ? accurateSizes[i] ?? "Bytest" : sizes[i] ?? "Bytes"
  }`
}



/**
 * Ensures translated string contains a value for every available languages
 * @param availableLanguages available translations list
 * @param value value to set for required languageCode
 * @returns new instance of @see {@link TranslatedString}
 */
export const ensureTranslatedStringCorrectness = (translatedString:TranslatedString | null | undefined, availableLanguages: string[], value: string = ''): TranslatedString => {
  if(translatedString === undefined || translatedString === null) return emptyTranslatedString(availableLanguages);

  availableLanguages.forEach((language) => {
    if(translatedString[language] === undefined){
      translatedString[language] = value;
    }
  });

  return translatedString;
};