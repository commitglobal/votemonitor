import { useMemo } from 'react'
import { useNavigate, useSearch } from '@tanstack/react-router'
import { useCurrentElectionRound } from '@/contexts/election-round.context'
import { useCoalitionDetails } from '@/queries/coalitions'
import { DataSource } from '@/types/common'
import { SingleSelectDataTableFacetedFilter } from './data-table/data-table-faceted-filter'

export default function CoalitionMemberFilter() {
  const { electionRound } = useCurrentElectionRound()
  const { data } = useCoalitionDetails(electionRound?.id)
  const search = useSearch({ strict: false })
  const navigate = useNavigate()

  const options = useMemo(() => {
    return (
      data?.members.map((ngo) => ({
        value: ngo.id,
        label: ngo.name,
      })) ?? []
    )
  }, [data])

  return search.dataSource === DataSource.Coalition ? (
    <SingleSelectDataTableFacetedFilter
      title='Coalition member'
      options={options}
      value={search.coalitionMemberId as string}
      onValueChange={(value) =>
        navigate({
          to: '.',
          search: (prev) => ({
            ...prev,
            coalitionMemberId: value,
          }),
          replace: true,
        })
      }
    />
  ) : null
}
