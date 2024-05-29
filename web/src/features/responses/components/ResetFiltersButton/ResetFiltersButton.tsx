import { useNavigate } from '@tanstack/react-router';
import type { FunctionComponent } from '@/common/types';
import { Button } from '@/components/ui/button';
import { useSetPrevSearch } from '@/common/prev-search-store';

type ResetFiltersButtonProps = {
  disabled: boolean;
};
export function ResetFiltersButton({ disabled }: ResetFiltersButtonProps): FunctionComponent {
  const navigate = useNavigate();
  const setPrevSearch = useSetPrevSearch();

  return (
    <Button
      disabled={disabled}
      onClick={() => {
        setPrevSearch({});
        void navigate({});
      }}
      variant='ghost-primary'>
      Reset filters
    </Button>
  );
}
