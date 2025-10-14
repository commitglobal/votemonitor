import z from 'zod'

export interface PollingStation {
  id: string
  level1: string
  level2?: string
  level3?: string
  level4?: string
  level5?: string
  number: string
  address: string
  displayOrder: number
  tags?: Record<string, string>
}

export type LevelNodeModel = {
  id: number
  name: string
  depth: number
  parentId: number
}

export const pollingStationsFiltersSchema = z.object({
  level1Filter: z.string().optional(),
  level2Filter: z.string().optional(),
  level3Filter: z.string().optional(),
  level4Filter: z.string().optional(),
  level5Filter: z.string().optional(),
  pollingStationNumberFilter: z.string().optional(),
})

export type PollingStationsFilters = z.infer<
  typeof pollingStationsFiltersSchema
>
