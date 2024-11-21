import { FunctionComponent } from '@/common/types';
import {
  DropdownMenu,
  DropdownMenuCheckboxItem,
  DropdownMenuContent,
  DropdownMenuLabel,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from '@/components/ui/dropdown-menu';
import { Cog8ToothIcon } from '@heroicons/react/24/outline';
import { useCitizenReportsColumnsVisibility, useCitizenReportsToggleColumn } from '../../store/column-visibility';
import { citizenReportsColumnVisibilityOptions } from '../../utils/column-visibility-options';

export function CitizenReportsColumnVisibilitySelector(): FunctionComponent {
  const columnsVisibility = useCitizenReportsColumnsVisibility();
  const toggleColumn = useCitizenReportsToggleColumn();

  return (
    <DropdownMenu>
      <DropdownMenuTrigger>
        <Cog8ToothIcon className='w-[20px] text-purple-900 cursor-pointer' />
      </DropdownMenuTrigger>
      <DropdownMenuContent>
        <DropdownMenuLabel>Table columns</DropdownMenuLabel>
        <DropdownMenuSeparator />
        {citizenReportsColumnVisibilityOptions.map((option) => (
          <DropdownMenuCheckboxItem
            key={option.id}
            checked={columnsVisibility[option.id]}
            disabled={!option.enableHiding}
            onCheckedChange={(checked) => {
              toggleColumn(option.id, checked);
            }}>
            {option.label}
          </DropdownMenuCheckboxItem>
        ))}
      </DropdownMenuContent>
    </DropdownMenu>
  );
}
