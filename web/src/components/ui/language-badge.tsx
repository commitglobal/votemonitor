import type { FunctionComponent } from '@/common/types';
import { useLanguages } from '@/features/languages/queries';
import { useMemo } from 'react';
import { Badge } from './badge';


interface LanguageBadgeProps {
  languageCode: string;
}

function LanguageBadge({ languageCode }: LanguageBadgeProps): FunctionComponent {
  const { data: languages } = useLanguages();

  const label = useMemo<string>(() => {
    const language = languages?.find(l => l.code === languageCode);

    return language ? `${language?.name} / ${language?.nativeName} / ${language?.code}` : 'Unknown';
  }, [languageCode, languages]);

  return (
    <Badge className='bg-purple-50 text-purple-600 font-medium hover:bg-purple-100 py-2 text-sm gap-2'>
      <span>{label}</span>
    </Badge>
  );
}
export { LanguageBadge };

