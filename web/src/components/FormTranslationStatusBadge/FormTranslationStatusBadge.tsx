import { LanguagesTranslationStatus, TranslationStatus } from '@/common/types';
import { Badge } from '@/components/ui/badge';
import { cn } from '@/lib/utils';

export interface FormTranslationStatusBadgeProps {
  translationStatus: LanguagesTranslationStatus;
  defaultLanguage: string;
}

function FormTranslationStatusBadge({ translationStatus, defaultLanguage }: FormTranslationStatusBadgeProps) {
  return (
    <Badge
      className={cn({
        'text-green-600 bg-green-200': translationStatus[defaultLanguage] === TranslationStatus.Translated,
        'text-yellow-600 bg-yellow-200': translationStatus[defaultLanguage] === TranslationStatus.MissingTranslations,
        'text-slate-700 bg-slate-200': translationStatus[defaultLanguage] === undefined,
      })}>
      {translationStatus[defaultLanguage] === TranslationStatus.Translated
        ? 'Translated'
        : translationStatus[defaultLanguage] === TranslationStatus.MissingTranslations
          ? 'Missing translation'
          : 'Unknown'}
    </Badge>
  );
}

export default FormTranslationStatusBadge;
