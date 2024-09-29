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
import { useIssueReportsColumnsVisibility, useIssueReportsToggleColumn } from '../../store/column-visibility';
import { issueReportsColumnVisibilityOptions, IssueReportsViewBy } from '../../utils/column-visibility-options';

type ColumnVisibilitySelectorProps = {
  byFilter: IssueReportsViewBy;
};

export function IssueReportsColumnsVisibilitySelector({ byFilter }: ColumnVisibilitySelectorProps): FunctionComponent {
  const columnsVisibility = useIssueReportsColumnsVisibility();
  const toggleColumn = useIssueReportsToggleColumn();

  return (
    <DropdownMenu>
      <DropdownMenuTrigger>
        <Cog8ToothIcon className='w-[20px] text-purple-900 cursor-pointer' />
      </DropdownMenuTrigger>
      <DropdownMenuContent>
        <DropdownMenuLabel>Table columns</DropdownMenuLabel>
        <DropdownMenuSeparator />
        {issueReportsColumnVisibilityOptions[byFilter].map((option) => (
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
