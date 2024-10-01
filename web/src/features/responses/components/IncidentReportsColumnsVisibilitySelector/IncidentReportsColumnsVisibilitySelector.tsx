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
import { useIncidentReportsColumnsVisibility, useIncidentReportsToggleColumn } from '../../store/column-visibility';
import { incidentReportsColumnVisibilityOptions, IncidentReportsViewBy } from '../../utils/column-visibility-options';

type ColumnVisibilitySelectorProps = {
  byFilter: IncidentReportsViewBy;
};

export function IncidentReportsColumnsVisibilitySelector({ byFilter }: ColumnVisibilitySelectorProps): FunctionComponent {
  const columnsVisibility = useIncidentReportsColumnsVisibility();
  const toggleColumn = useIncidentReportsToggleColumn();

  return (
    <DropdownMenu>
      <DropdownMenuTrigger>
        <Cog8ToothIcon className='w-[20px] text-purple-900 cursor-pointer' />
      </DropdownMenuTrigger>
      <DropdownMenuContent>
        <DropdownMenuLabel>Table columns</DropdownMenuLabel>
        <DropdownMenuSeparator />
        {incidentReportsColumnVisibilityOptions[byFilter].map((option) => (
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
