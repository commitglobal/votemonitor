import type { FunctionComponent } from '@/common/types';
import { cn } from '@/lib/utils';
import { XMarkIcon } from '@heroicons/react/24/outline';
import { cva, type VariantProps } from 'class-variance-authority';
import type * as React from 'react';

const badgeVariants = cva(
  'inline-flex items-center rounded-full border px-2.5 py-0.5 text-xs font-semibold transition-colors focus:outline-none focus:ring-2 focus:ring-ring focus:ring-offset-2',
  {
    variants: {
      variant: {
        default: 'border-transparent bg-primary text-primary-foreground',
        secondary:
          'border border-input bg-background hover:bg-purple-50 text-purple-900 border-purple-900 hover:text-accent-foreground',
        destructive: 'border-transparent bg-destructive text-destructive-foreground hover:bg-destructive/80',
        outline:
          'border border-input bg-purple-100 hover:bg-purple-50 text-purple-900 border-purple-900 hover:text-accent-foreground',
      },
    },
    defaultVariants: {
      variant: 'default',
    },
  }
);

export interface BadgeProps extends React.HTMLAttributes<HTMLDivElement>, VariantProps<typeof badgeVariants> {}

function Badge({ className, variant, ...props }: BadgeProps): FunctionComponent {
  return <div className={cn(badgeVariants({ variant }), className)} {...props} />;
}

interface FilterBadgeProps {
  label: string;
  onClear: VoidFunction;
}

function FilterBadge({ label, onClear }: FilterBadgeProps): FunctionComponent {
  return (
    <Badge className='bg-purple-50 text-purple-600 font-medium hover:bg-purple-100 py-2 text-sm gap-2'>
      <span>{label}</span>
      <button title='Remove filter' onClick={onClear}>
        <XMarkIcon className='w-4' />
      </button>
    </Badge>
  );
}

export { Badge, badgeVariants, FilterBadge };
