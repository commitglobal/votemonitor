import { getRouteApi } from '@tanstack/react-router';
import type { FunctionComponent } from '@/common/types';
import { Input } from '@/components/ui/input';
import { Select, SelectContent, SelectGroup, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select';
import { FormType } from '../../models/form-submission';

const routeApi = getRouteApi('/responses/');

export function FormsFiltersByEntry(): FunctionComponent {
  const navigate = routeApi.useNavigate();
  const search = routeApi.useSearch();

  return (
    <>
      <Select
        onValueChange={(value) => {
          void navigate({ search: (prev) => ({ ...prev, formTypeFilter: value }) });
        }}
        value={search.formTypeFilter ?? ''}>
        <SelectTrigger>
          <SelectValue placeholder='Form type' />
        </SelectTrigger>
        <SelectContent>
          <SelectGroup>
            {Object.values(FormType).map((value) => (
              <SelectItem value={value}>{value}</SelectItem>
            ))}
          </SelectGroup>
        </SelectContent>
      </Select>

      <Input
        defaultValue={search.pollingStationNumberFilter}
        placeholder='Station number'
        onChange={(e) => {
          void navigate({ search: (prev) => ({ ...prev, pollingStationNumberFilter: e.target.value }) });
        }}
        value={search.pollingStationNumberFilter ?? ''}
      />

      <Select
        onValueChange={(value) => {
          void navigate({ search: (prev) => ({ ...prev, hasFlaggedAnswers: value }) });
        }}
        value={search.hasFlaggedAnswers ?? ''}>
        <SelectTrigger>
          <SelectValue placeholder='Flagged answers' />
        </SelectTrigger>
        <SelectContent>
          <SelectGroup>
            <SelectItem value='true'>Yes</SelectItem>
            <SelectItem value='false'>No</SelectItem>
          </SelectGroup>
        </SelectContent>
      </Select>

      <Input
        placeholder='Location - L1'
        onChange={(e) => {
          void navigate({ search: (prev) => ({ ...prev, level1Filter: e.target.value }) });
        }}
        value={search.level1Filter ?? ''}
      />

      <Input
        placeholder='Location - L2'
        onChange={(e) => {
          void navigate({ search: (prev) => ({ ...prev, level2Filter: e.target.value }) });
        }}
        value={search.level2Filter ?? ''}
      />

      <Input
        placeholder='Location - L3'
        onChange={(e) => {
          void navigate({ search: (prev) => ({ ...prev, level3Filter: e.target.value }) });
        }}
        value={search.level3Filter ?? ''}
      />
    </>
  );
}
