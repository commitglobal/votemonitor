import { useSetPrevSearch } from '@/common/prev-search-store';
import type { FunctionComponent } from '@/common/types';
import { Button } from '@/components/ui/button';
import { useNavigate } from '@tanstack/react-router';

type ResetFiltersButtonProps = {
  disabled: boolean;
  params: Record<string, string>;
};
export function ResetFiltersButton({ disabled, params }: ResetFiltersButtonProps): FunctionComponent {
  const navigate = useNavigate();
  const setPrevSearch = useSetPrevSearch();

  return (
    <Button
      disabled={disabled}
      onClick={() => {
        setPrevSearch(params);
        navigate({});
      }}
      variant='ghost-primary'>
      Reset filters
    </Button>
  );
}
