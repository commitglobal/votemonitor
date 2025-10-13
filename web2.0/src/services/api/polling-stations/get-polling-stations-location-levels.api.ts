import API from '@/services/api'
import { LevelNodeModel } from '@/types/polling-station'

type PollingStationsLocationLevelsResponse = { nodes: LevelNodeModel[] }

export const getPollingStationsLocationLevels = (
  electionRoundId: string
): Promise<Record<string, LevelNodeModel[]>> => {
  return API.get<PollingStationsLocationLevelsResponse>(
    `/election-rounds/${electionRoundId}/polling-stations:fetchLevels`
  ).then((res) =>
    res.data.nodes.reduce<Record<string, LevelNodeModel[]>>(
      (group, node) => ({
        ...group,
        [node.depth]: [...(group[node.depth] ?? []), node],
      }),
      {}
    )
  )
}
