import { getRouteApi } from '@tanstack/react-router';
import { useMemo } from 'react';
import type { FunctionComponent } from '@/common/types';
import { Select, SelectContent, SelectGroup, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select';
import { usePollingStations } from '@/common/polling-stations';

const routeApi = getRouteApi('/responses/');

export function PollingStationsFilters(): FunctionComponent {
  const navigate = routeApi.useNavigate();
  const search = routeApi.useSearch();
  const { data } = usePollingStations();

  const selectedLevel1Node = useMemo(
    () => data?.[1]?.find((node) => node.name === search.level1Filter),
    [data, search.level1Filter]
  );

  const selectedLevel2Node = useMemo(
    () => data?.[2]?.find((node) => node.name === search.level2Filter),
    [data, search.level2Filter]
  );

  const selectedLevel3Node = useMemo(
    () => data?.[3]?.find((node) => node.name === search.level3Filter),
    [data, search.level3Filter]
  );

  const selectedLevel4Node = useMemo(
    () => data?.[4]?.find((node) => node.name === search.level4Filter),
    [data, search.level4Filter]
  );

  const filteredLevel2Nodes = useMemo(
    () => data?.[2]?.filter((node) => node.parentId === selectedLevel1Node?.id),
    [data, selectedLevel1Node?.id]
  );

  const filteredLevel3Nodes = useMemo(
    () => data?.[3]?.filter((node) => node.parentId === selectedLevel2Node?.id),
    [data, selectedLevel2Node?.id]
  );

  const filteredLevel4Nodes = useMemo(
    () => data?.[4]?.filter((node) => node.parentId === selectedLevel3Node?.id),
    [data, selectedLevel3Node?.id]
  );

  const filteredLevel5Nodes = useMemo(
    () => data?.[5]?.filter((node) => node.parentId === selectedLevel4Node?.id),
    [data, selectedLevel4Node?.id]
  );

  return (
    <>
      <Select
        onValueChange={(value) => {
          void navigate({
            search: (prev) => ({
              ...prev,
              level1Filter: value,
              level2Filter: undefined,
              level3Filter: undefined,
              level4Filter: undefined,
              level5Filter: undefined,
            }),
          });
        }}
        value={search.level1Filter ?? ''}>
        <SelectTrigger>
          <SelectValue placeholder='Location - L1' />
        </SelectTrigger>
        <SelectContent>
          <SelectGroup>
            {data?.[1]?.map((node) => (
              <SelectItem key={node.id} value={node.name}>
                {node.name}
              </SelectItem>
            ))}
          </SelectGroup>
        </SelectContent>
      </Select>

      <Select
        disabled={!search.level1Filter || !filteredLevel2Nodes?.length}
        onValueChange={(value) => {
          void navigate({
            search: (prev) => ({
              ...prev,
              level2Filter: value,
              level3Filter: undefined,
              level4Filter: undefined,
              level5Filter: undefined,
            }),
          });
        }}
        value={search.level2Filter ?? ''}>
        <SelectTrigger>
          <SelectValue placeholder='Location - L2' />
        </SelectTrigger>
        <SelectContent>
          <SelectGroup>
            {filteredLevel2Nodes?.map((node) => (
              <SelectItem key={node.id} value={node.name}>
                {node.name}
              </SelectItem>
            ))}
          </SelectGroup>
        </SelectContent>
      </Select>

      <Select
        disabled={!search.level2Filter || !filteredLevel3Nodes?.length}
        onValueChange={(value) => {
          void navigate({
            search: (prev) => ({ ...prev, level3Filter: value, level4Filter: undefined, level5Filter: undefined }),
          });
        }}
        value={search.level3Filter ?? ''}>
        <SelectTrigger>
          <SelectValue placeholder='Location - L3' />
        </SelectTrigger>
        <SelectContent>
          <SelectGroup>
            {filteredLevel3Nodes?.map((node) => (
              <SelectItem key={node.id} value={node.name}>
                {node.name}
              </SelectItem>
            ))}
          </SelectGroup>
        </SelectContent>
      </Select>

      <Select
        disabled={!search.level3Filter || !filteredLevel4Nodes?.length}
        onValueChange={(value) => {
          void navigate({ search: (prev) => ({ ...prev, level4Filter: value, level5Filter: undefined }) });
        }}
        value={search.level4Filter ?? ''}>
        <SelectTrigger>
          <SelectValue placeholder='Location - L4' />
        </SelectTrigger>
        <SelectContent>
          <SelectGroup>
            {filteredLevel4Nodes?.map((node) => (
              <SelectItem key={node.id} value={node.name}>
                {node.name}
              </SelectItem>
            ))}
          </SelectGroup>
        </SelectContent>
      </Select>

      <Select
        disabled={!search.level4Filter || !filteredLevel5Nodes?.length}
        onValueChange={(value) => {
          void navigate({ search: (prev) => ({ ...prev, level5Filter: value }) });
        }}
        value={search.level5Filter ?? ''}>
        <SelectTrigger>
          <SelectValue placeholder='Location - L5' />
        </SelectTrigger>
        <SelectContent>
          <SelectGroup>
            {filteredLevel5Nodes?.map((node) => (
              <SelectItem key={node.id} value={node.name}>
                {node.name}
              </SelectItem>
            ))}
          </SelectGroup>
        </SelectContent>
      </Select>
    </>
  );
}
