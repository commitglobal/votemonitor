import type { ReactElement } from 'react';
import { ChevronUp, ChevronDown } from 'lucide-react';
import type { Column } from '@tanstack/react-table';
import { cn } from '@/lib/utils';
import { Button } from '@/components/ui/button';

interface DataTableColumnHeaderProps<TData, TValue> extends React.HTMLAttributes<HTMLDivElement> {
  column: Column<TData, TValue>;
  title: string;
}

export function DataTableColumnHeader<TData, TValue>({
  column,
  title,
  className,
}: DataTableColumnHeaderProps<TData, TValue>): ReactElement {
  if (!column.columnDef.enableSorting) {
    return <div className={cn(className)}>{title}</div>;
  }

  /**
   * false = desc
   * true = asc
   */
  const isCurrentSortAsc = column.getIsSorted() === 'asc';

  const onClick = (): void => {
    column.toggleSorting(isCurrentSortAsc);
  };

  const iconClassName = cn(
    'w-4 h-4 text-gray-500',
    column.getIsSorted() ? '' : 'text-muted-foreground/50 group-hover:text-gray-500 group-focus:text-gray-500'
  );

  return (
    <Button type='button' className='gap-2 group' variant='ghost' size='none' onClick={onClick}>
      <span>{title}</span>
      {isCurrentSortAsc ? <ChevronUp className={iconClassName} /> : <ChevronDown className={iconClassName} />}
    </Button>
  );
}
