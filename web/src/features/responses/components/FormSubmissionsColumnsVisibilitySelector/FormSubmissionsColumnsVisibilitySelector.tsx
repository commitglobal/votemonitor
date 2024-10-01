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
import { useFormSubmissionsColumnsVisibility, useFormSubmissionsToggleColumn } from '../../store/column-visibility';
import { formSubmissionsColumnVisibilityOptions, type FormSubmissionsViewBy } from '../../utils/column-visibility-options';

type ColumnVisibilitySelectorProps = {
  byFilter: FormSubmissionsViewBy;
};

export function FormSubmissionsColumnsVisibilitySelector({ byFilter }: ColumnVisibilitySelectorProps): FunctionComponent {
  const columnsVisibility = useFormSubmissionsColumnsVisibility();
  const toggleColumn = useFormSubmissionsToggleColumn();

  return (
    <DropdownMenu>
      <DropdownMenuTrigger>
        <Cog8ToothIcon className='w-[20px] text-purple-900 cursor-pointer' />
      </DropdownMenuTrigger>
      <DropdownMenuContent>
        <DropdownMenuLabel>Table columns</DropdownMenuLabel>
        <DropdownMenuSeparator />
        {formSubmissionsColumnVisibilityOptions[byFilter].map((option) => (
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
