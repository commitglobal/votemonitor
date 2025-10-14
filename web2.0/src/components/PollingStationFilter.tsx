import { useMemo } from 'react'
import { useCurrentElectionRound } from '@/contexts/election-round.context'
import { usePollingStationsLocationLevels } from '@/queries/polling-stations'
import { NavigateFn, SearchRecord } from '@/hooks/use-table-url-state'
import { SingleSelectDataTableFacetedFilter } from './data-table-faceted-filter'
import { DebouncedInput } from './ui/debounced-input'

export function PollingStationFilteruseFilters({
  search,
  navigate,
}: {
  search: SearchRecord
  navigate: NavigateFn
}) {
  const { electionRoundId } = useCurrentElectionRound()
  const { data: levels } = usePollingStationsLocationLevels(electionRoundId)

  const selectedLevel1Node = useMemo(
    () => levels?.[1]?.find((node) => node.name === search.level1Filter),
    [levels, search.level1Filter]
  )

  const selectedLevel2Node = useMemo(
    () => levels?.[2]?.find((node) => node.name === search.level2Filter),
    [levels, search.level2Filter]
  )

  const selectedLevel3Node = useMemo(
    () => levels?.[3]?.find((node) => node.name === search.level3Filter),
    [levels, search.level3Filter]
  )

  const selectedLevel4Node = useMemo(
    () => levels?.[4]?.find((node) => node.name === search.level4Filter),
    [levels, search.level4Filter]
  )

  const selectedLevel5Node = useMemo(
    () => levels?.[5]?.find((node) => node.name === search.level5Filter),
    [levels, search.level5Filter]
  )

  const level1Nodes = useMemo(
    () =>
      levels?.[1]
        ?.sort((a, b) => a.name.localeCompare(b.name))
        .map((node) => ({
          value: node.name,
          label: node.name,
        })) ?? [],
    [levels]
  )

  const filteredLevel2Nodes = useMemo(
    () =>
      levels?.[2]
        ?.filter((node) => node.parentId === selectedLevel1Node?.id)
        ?.sort((a, b) => a.name.localeCompare(b.name))
        .map((node) => ({
          value: node.name,
          label: node.name,
        })) ?? [],
    [levels, selectedLevel1Node?.id]
  )

  const filteredLevel3Nodes = useMemo(
    () =>
      levels?.[3]
        ?.filter((node) => node.parentId === selectedLevel2Node?.id)
        ?.sort((a, b) => a.name.localeCompare(b.name))
        .map((node) => ({
          value: node.name,
          label: node.name,
        })) ?? [],
    [levels, selectedLevel2Node?.id]
  )

  const filteredLevel4Nodes = useMemo(
    () =>
      levels?.[4]
        ?.filter((node) => node.parentId === selectedLevel3Node?.id)
        ?.sort((a, b) => a.name.localeCompare(b.name))
        .map((node) => ({
          value: node.name,
          label: node.name,
        })) ?? [],
    [levels, selectedLevel3Node?.id]
  )

  const filteredLevel5Nodes = useMemo(
    () =>
      levels?.[5]
        ?.filter((node) => node.parentId === selectedLevel4Node?.id)
        ?.sort((a, b) => a.name.localeCompare(b.name))
        .map((node) => ({
          value: node.name,
          label: node.name,
        })) ?? [],
    [levels, selectedLevel4Node?.id]
  )

  const isFinalNode = useMemo(() => {
    if (levels === undefined) return false

    if (selectedLevel5Node) return true
    if (selectedLevel4Node)
      return (
        levels[5] === undefined ||
        !levels[5].some((node) => node.parentId === selectedLevel4Node.id)
      )
    if (selectedLevel3Node)
      return (
        levels[4] === undefined ||
        !levels[4].some((node) => node.parentId === selectedLevel3Node?.id)
      )
    if (selectedLevel2Node)
      return (
        levels[3] === undefined ||
        !levels[3].some((node) => node.parentId === selectedLevel2Node?.id)
      )
    if (selectedLevel1Node)
      return (
        levels[2] === undefined ||
        !levels[2].some((node) => node.parentId === selectedLevel1Node?.id)
      )

    return false
  }, [
    levels,
    selectedLevel1Node?.id,
    selectedLevel2Node?.id,
    selectedLevel3Node?.id,
    selectedLevel4Node?.id,
    selectedLevel5Node?.id,
  ])

  const setFilters = (partialFilters: SearchRecord) =>
    navigate({
      search: (prev) => ({
        ...(prev as SearchRecord),
        ...partialFilters,
      }),
    })

  return (
    <>
      <SingleSelectDataTableFacetedFilter
        title='Level 1'
        options={level1Nodes}
        value={search.level1Filter as string}
        onValueChange={(value) => setFilters({ level1Filter: value })}
      />

      <SingleSelectDataTableFacetedFilter
        title='Level 2'
        options={filteredLevel2Nodes}
        value={search.level2Filter as string}
        onValueChange={(value) => setFilters({ level2Filter: value })}
        disabled={!search.level1Filter || !filteredLevel2Nodes?.length}
      />

      <SingleSelectDataTableFacetedFilter
        title='Level 3'
        options={filteredLevel3Nodes}
        value={search.level3Filter as string}
        onValueChange={(value) => setFilters({ level3Filter: value })}
        disabled={!search.level2Filter || !filteredLevel3Nodes?.length}
      />

      <SingleSelectDataTableFacetedFilter
        title='Level 4'
        options={filteredLevel4Nodes}
        value={search.coalitionMemberId as string}
        onValueChange={(value) => setFilters({ level4Filter: value })}
        disabled={!search.level3Filter || !filteredLevel4Nodes?.length}
      />

      <SingleSelectDataTableFacetedFilter
        title='Level 5'
        options={filteredLevel5Nodes}
        value={search.level5Filter as string}
        onValueChange={(value) => setFilters({ level5Filter: value })}
        disabled={!search.level4Filter || !filteredLevel5Nodes?.length}
      />

      <DebouncedInput
        placeholder='Polling station number'
        className='h-8 w-40 lg:w-48'
        disabled={!isFinalNode}
        onChange={(value) => {
          setFilters({ pollingStationNumberFilter: value })
        }}
        value={search.pollingStationNumberFilter as string}
      />
    </>
  )
}
