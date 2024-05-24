import { Badge } from '@/components/ui/badge';
import { useLanguages } from '@/features/languages/queries';
import { useMemo } from 'react';

export interface LanguageBadgeProps {
    languageCode: string;
}
function LanguageBadge({ languageCode }: LanguageBadgeProps) {

    const { data: languages } = useLanguages();

    const language = useMemo(() => {
        const language = languages?.find(l => l.code === languageCode);

        return language;
    }, [languageCode, languages])
    return (
        <Badge variant='outline'>{language?.name} / {language?.nativeName} ({language?.code})</Badge>
    )
}

export default LanguageBadge