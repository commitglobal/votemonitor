import {
  Select,
  SelectContent,
  SelectGroup,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import { HandleReducerSearchParams, useLocationsLevels } from "@/hooks";
import { LocationState } from "@/location-reducer";
import { FC, useMemo } from "react";

interface LocationSelectorProps {
  search: LocationState;
  handleReducerSearch: (params: HandleReducerSearchParams) => void;
}

export const LocationSelector: FC<LocationSelectorProps> = ({
  search,
  handleReducerSearch,
}) => {
  const currentElectionRoundId = import.meta.env["VITE_ELECTION_ROUND_ID"];
  const { data } = useLocationsLevels(currentElectionRoundId);

  const selectedLevel1Node = useMemo(
    () => data?.[1]?.find((node) => node.id === search.level1Filter),
    [data, search.level1Filter]
  );

  const selectedLevel2Node = useMemo(
    () => data?.[2]?.find((node) => node.id === search.level2Filter),
    [data, search.level2Filter]
  );

  const selectedLevel3Node = useMemo(
    () => data?.[3]?.find((node) => node.id === search.level3Filter),
    [data, search.level3Filter]
  );

  const selectedLevel4Node = useMemo(
    () => data?.[4]?.find((node) => node.id === search.level4Filter),
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
    <div className="mt-10">
      <h4 className="mb-2">Location:</h4>
      <div className="flex flex-col md:flex-row justify-between">
        <Select
          onValueChange={(value: string) => {
            handleReducerSearch({ level: "level1Filter", value });
          }}
          value={search.level1Filter?.toString() ?? ""}
        >
          <SelectTrigger className="md:w-[180px]">
            <SelectValue placeholder="Location - L1" />
          </SelectTrigger>
          <SelectContent>
            <SelectGroup>
              {data?.[1]?.map((node) => (
                <SelectItem key={node.id} value={node.id.toString()}>
                  {node.name}
                </SelectItem>
              ))}
            </SelectGroup>
          </SelectContent>
        </Select>

        <Select
          disabled={!search.level1Filter || !filteredLevel2Nodes?.length}
          onValueChange={(value) => {
            handleReducerSearch({ level: "level2Filter", value });
          }}
          value={search.level2Filter?.toString() ?? ""}
        >
          <SelectTrigger className="md:w-[180px]">
            <SelectValue placeholder="Location - L2" />
          </SelectTrigger>
          <SelectContent>
            <SelectGroup>
              {filteredLevel2Nodes?.map((node) => (
                <SelectItem key={node.id} value={node.id.toString()}>
                  {node.name}
                </SelectItem>
              ))}
            </SelectGroup>
          </SelectContent>
        </Select>

        <Select
          disabled={!search.level2Filter || !filteredLevel3Nodes?.length}
          onValueChange={(value) => {
            handleReducerSearch({ level: "level3Filter", value });
          }}
          value={search.level3Filter?.toString() ?? ""}
        >
          <SelectTrigger className="md:w-[180px]">
            <SelectValue placeholder="Location - L3" />
          </SelectTrigger>
          <SelectContent>
            <SelectGroup>
              {filteredLevel3Nodes?.map((node) => (
                <SelectItem key={node.id} value={node.id.toString()}>
                  {node.name}
                </SelectItem>
              ))}
            </SelectGroup>
          </SelectContent>
        </Select>

        <Select
          disabled={!search.level3Filter || !filteredLevel4Nodes?.length}
          onValueChange={(value) => {
            handleReducerSearch({ level: "level4Filter", value });
          }}
          value={search.level4Filter?.toString() ?? ""}
        >
          <SelectTrigger className="md:w-[180px]">
            <SelectValue placeholder="Location - L4" />
          </SelectTrigger>
          <SelectContent>
            <SelectGroup>
              {filteredLevel4Nodes?.map((node) => (
                <SelectItem key={node.id} value={node.id.toString()}>
                  {node.name}
                </SelectItem>
              ))}
            </SelectGroup>
          </SelectContent>
        </Select>

        <Select
          disabled={!search.level4Filter || !filteredLevel5Nodes?.length}
          onValueChange={(value) => {
            handleReducerSearch({ level: "level5Filter", value });
          }}
          value={search.level5Filter?.toString() ?? ""}
        >
          <SelectTrigger className="md:w-[180px]">
            <SelectValue placeholder="Location - L5" />
          </SelectTrigger>
          <SelectContent>
            <SelectGroup>
              {filteredLevel5Nodes?.map((node) => (
                <SelectItem key={node.id} value={node.id.toString()}>
                  {node.name}
                </SelectItem>
              ))}
            </SelectGroup>
          </SelectContent>
        </Select>
      </div>
    </div>
  );
};
