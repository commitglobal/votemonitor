import CountryFilter from "@/features/filtering/components/CountryFilter";
import { ElectionRoundStatusFilter } from "@/features/filtering/components/ElectionRoundStatusFilter";
import { FilteringContainer } from "@/features/filtering/components/FilteringContainer";

export default function ElectionsRoundFilter() {
  return (
    <FilteringContainer>
      <ElectionRoundStatusFilter />
      <CountryFilter />
    </FilteringContainer>
  );
}
