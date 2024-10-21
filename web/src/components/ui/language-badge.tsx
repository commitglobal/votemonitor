import type { FunctionComponent } from '@/common/types';
import { useMemo } from 'react';
import { Badge } from './badge';
import { useLanguages } from '@/hooks/languages';
import { cva, VariantProps } from 'class-variance-authority';
import { cn } from '@/lib/utils';

const badgeVariants = cva('', {
  variants: {
    variant: {
      unstyled: 'gap-2 py-2 text-sm font-medium text-slate-700 bg-transparent',
      purple: 'gap-2 py-2 text-sm font-medium text-purple-600 bg-purple-50 hover:bg-purple-100',
    },
  },
  defaultVariants: {
    variant: 'purple',
  },
});

interface LanguageBadgeProps {
  languageCode: string;
  displayMode?: 'full' | 'native' | 'native+code' | 'english' | 'english+code';
}

function LanguageBadge({
  languageCode,
  variant,
  displayMode,
}: LanguageBadgeProps & VariantProps<typeof badgeVariants>): FunctionComponent {
  const { data: languages } = useLanguages();

  const label = useMemo<string>(() => {
    const language = languages?.find((l) => l.code === languageCode);
    if (language === undefined) {
      return 'Unknown language';
    }

    if (displayMode === 'native') {
      return `${language?.nativeName}`;
    }

    if (displayMode === 'native+code') {
      return `${language?.nativeName} / ${language?.code}`;
    }

    if (displayMode === 'english') {
      return `${language?.name}`;
    }

    if (displayMode === 'english+code') {
      return `${language?.name} / ${language?.code}`;
    }

    return `${language?.name} / ${language?.nativeName} / ${language?.code}`;
  }, [languageCode, languages, displayMode]);

  return (
    <Badge className={cn(badgeVariants({ variant }))}>
      <span>{label}</span>
    </Badge>
  );
}
export { LanguageBadge };
