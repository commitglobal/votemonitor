import type { FunctionComponent } from '@/common/types';
import { useMemo } from 'react';
import { Badge } from './badge';
import { useLanguages } from '@/hooks/languages';


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
    <Badge className='gap-2 py-2 text-sm font-medium text-purple-600 bg-purple-50 hover:bg-purple-100'>
      <span>{label}</span>
    </Badge>
  );
}
export { LanguageBadge };

