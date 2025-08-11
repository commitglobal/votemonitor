import { useCurrentElectionRound } from "@/contexts/election-round.context";
import { useCoalitionDetails } from "@/queries/coalitions";
import { useNavigate, useSearch } from "@tanstack/react-router";
import { useMemo } from "react";
import { SingleSelectDataTableFacetedFilter } from "./data-table-faceted-filter";
import { DataSource } from "@/types/common";

export default function CoalitionMemberFilter() {
  const { electionRoundId } = useCurrentElectionRound();
  const { data } = useCoalitionDetails(electionRoundId);
  console.log(data);
  const search = useSearch({ strict: false });
  const navigate = useNavigate();

  const options = useMemo(() => {
    return (
      data?.members.map((ngo) => ({
        value: ngo.id,
        label: ngo.name,
      })) ?? []
    );
  }, [data]);

  return search.dataSource === DataSource.Coalition ? (
    <SingleSelectDataTableFacetedFilter
      title="Coalition member"
      options={options}
      value={search.coalitionMemberId as string}
      onValueChange={(value) =>
        navigate({
          to: ".",
          search: (prev) => ({
            ...prev,
            coalitionMemberId: value,
          }),
          replace: true,
        })
      }
    />
  ) : null;
}
