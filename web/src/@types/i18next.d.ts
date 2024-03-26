import 'i18next';
import type { Dict } from '../i18n';

declare module 'i18next' {
  interface CustomTypeOptions {
    resources: Dict;
    defaultNs: 'translation';
    contextSeparator: '|';
  }
}
