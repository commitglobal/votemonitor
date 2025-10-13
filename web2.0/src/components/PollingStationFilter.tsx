import { useMemo } from 'react'
import { useCurrentElectionRound } from '@/contexts/election-round.context'
import { usePollingStationsLocationLevels } from '@/queries/polling-stations'
import { NavigateFn, SearchRecord } from '@/hooks/use-table-url-state'
import { SingleSelectDataTableFacetedFilter } from './data-table-faceted-filter'

export function PollingStationFilteruseFilters({
  search,
  navigate,
}: {
  search: SearchRecord
  navigate: NavigateFn
}) {
  const { electionRoundId } = useCurrentElectionRound()
  const { data } = usePollingStationsLocationLevels(electionRoundId)

  const selectedLevel1Node = useMemo(
    () => data?.[1]?.find((node) => node.name === search.level1Filter),
    [data, search.level1Filter]
  )

  console.log('selectedLevel1Node', selectedLevel1Node)

  const selectedLevel2Node = useMemo(
    () => data?.[2]?.find((node) => node.name === search.level2Filter),
    [data, search.level2Filter]
  )

  const selectedLevel3Node = useMemo(
    () => data?.[3]?.find((node) => node.name === search.level3Filter),
    [data, search.level3Filter]
  )

  const selectedLevel4Node = useMemo(
    () => data?.[4]?.find((node) => node.name === search.level4Filter),
    [data, search.level4Filter]
  )

  const selectedLevel5Node = useMemo(
    () => data?.[5]?.find((node) => node.name === search.level5Filter),
    [data, search.level5Filter]
  )

  const level1Nodes = useMemo(
    () =>
      data?.[1]
        ?.sort((a, b) => a.name.localeCompare(b.name))
        .map((node) => ({
          value: node.name,
          label: node.name,
        })) ?? [],
    [data]
  )

  const filteredLevel2Nodes = useMemo(
    () =>
      data?.[2]
        ?.filter((node) => node.parentId === selectedLevel1Node?.id)
        ?.sort((a, b) => a.name.localeCompare(b.name))
        .map((node) => ({
          value: node.name,
          label: node.name,
        })) ?? [],
    [data, selectedLevel1Node?.id]
  )

  const filteredLevel3Nodes = useMemo(
    () =>
      data?.[3]
        ?.filter((node) => node.parentId === selectedLevel2Node?.id)
        ?.sort((a, b) => a.name.localeCompare(b.name))
        .map((node) => ({
          value: node.name,
          label: node.name,
        })) ?? [],
    [data, selectedLevel2Node?.id]
  )

  const filteredLevel4Nodes = useMemo(
    () =>
      data?.[4]
        ?.filter((node) => node.parentId === selectedLevel3Node?.id)
        ?.sort((a, b) => a.name.localeCompare(b.name))
        .map((node) => ({
          value: node.name,
          label: node.name,
        })) ?? [],
    [data, selectedLevel3Node?.id]
  )

  const filteredLevel5Nodes = useMemo(
    () =>
      data?.[5]
        ?.filter((node) => node.parentId === selectedLevel4Node?.id)
        ?.sort((a, b) => a.name.localeCompare(b.name))
        .map((node) => ({
          value: node.name,
          label: node.name,
        })) ?? [],
    [data, selectedLevel4Node?.id]
  )

  const isFinalNode = useMemo(() => {
    if (data === undefined) return false

    if (selectedLevel5Node) return true
    if (selectedLevel4Node)
      return (
        data[5] === undefined ||
        !data[5].some((node) => node.parentId === selectedLevel4Node.id)
      )
    if (selectedLevel3Node)
      return (
        data[4] === undefined ||
        !data[4].some((node) => node.parentId === selectedLevel3Node?.id)
      )
    if (selectedLevel2Node)
      return (
        data[3] === undefined ||
        !data[3].some((node) => node.parentId === selectedLevel2Node?.id)
      )
    if (selectedLevel1Node)
      return (
        data[2] === undefined ||
        !data[2].some((node) => node.parentId === selectedLevel1Node?.id)
      )

    return false
  }, [
    data,
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
      />

      <SingleSelectDataTableFacetedFilter
        title='Level 3'
        options={filteredLevel3Nodes}
        value={search.level3Filter as string}
        onValueChange={(value) => setFilters({ level3Filter: value })}
      />

      <SingleSelectDataTableFacetedFilter
        title='Level 4'
        options={filteredLevel4Nodes}
        value={search.coalitionMemberId as string}
        onValueChange={(value) => setFilters({ level4Filter: value })}
      />

      <SingleSelectDataTableFacetedFilter
        title='Level 5'
        options={filteredLevel5Nodes}
        value={search.level5Filter as string}
        onValueChange={(value) => setFilters({ level5Filter: value })}
      />
    </>
  )
}
