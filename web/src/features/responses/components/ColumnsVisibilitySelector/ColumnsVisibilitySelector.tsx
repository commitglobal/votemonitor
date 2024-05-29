import { Cog8ToothIcon } from '@heroicons/react/24/outline';
import { FunctionComponent } from '@/common/types';
import {
  DropdownMenu,
  DropdownMenuCheckboxItem,
  DropdownMenuContent,
  DropdownMenuLabel,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from '@/components/ui/dropdown-menu';
import { useColumnsVisibility, useToggleColumn } from '../../store/column-visibility';
import { columnVisibilityOptions, type FilterBy } from '../../utils/column-visibility-options';

type ColumnVisibilitySelectorProps = {
  byFilter: FilterBy;
};

export function ColumnsVisibilitySelector({ byFilter }: ColumnVisibilitySelectorProps): FunctionComponent {
  const columnsVisibility = useColumnsVisibility();
  const toggleColumn = useToggleColumn();

  return (
    <DropdownMenu>
      <DropdownMenuTrigger>
        <Cog8ToothIcon className='w-[20px] text-purple-900 cursor-pointer' />
      </DropdownMenuTrigger>
      <DropdownMenuContent>
        <DropdownMenuLabel>Table columns</DropdownMenuLabel>
        <DropdownMenuSeparator />
        {columnVisibilityOptions[byFilter].map((option) => (
          <DropdownMenuCheckboxItem
            key={option.id}
            checked={columnsVisibility[byFilter][option.id]}
            disabled={!option.enableHiding}
            onCheckedChange={(checked) => {
              toggleColumn(byFilter, option.id, checked);
            }}>
            {option.label}
          </DropdownMenuCheckboxItem>
        ))}
      </DropdownMenuContent>
    </DropdownMenu>
  );
}
