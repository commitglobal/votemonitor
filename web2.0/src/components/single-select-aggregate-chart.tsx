'use client'

import { Card, CardAction, CardContent, CardHeader } from '@/components/ui/card'
import {
  ChartConfig,
  ChartContainer,
  ChartLegend,
  ChartLegendContent,
} from '@/components/ui/chart'
import { getTranslation } from '@/lib/translated-string'
import { SingleSelectQuestionAggregateModel } from '@/types/submissions-aggregate'
import { Download } from 'lucide-react'
import { useMemo } from 'react'
import { Cell, Pie, PieChart, Tooltip } from 'recharts'
import { Button } from './ui/button'

export const description = 'A pie chart with hover, labels, and tooltip'

export function SingleSelectAggregateChart({
  language,
  aggregate,
}: {
  language: string
  aggregate: SingleSelectQuestionAggregateModel
}) {
  const colors = ['#ff0000', '#00ff00', '#0000ff', '#ffff00', '#ff00ff']

  const chartData = useMemo(() => {
    return aggregate.question.options.map((option, index) => {
      const translated = getTranslation(option.text, language)
      return {
        option: translated,
        count: aggregate.answersHistogram[option.id] ?? 0,
      }
    })
  }, [aggregate, language])

  const chartConfig = useMemo(() => {
    return aggregate.question.options.reduce((acc, option, index) => {
      const label = getTranslation(option.text, language)
      acc[label] = {
        label,
        color: colors[index % colors.length],
      }
      return acc
    }, {} as ChartConfig)
  }, [aggregate, language])

  // Total count for percentage calculation
  const total = chartData.reduce((sum, d) => sum + d.count, 0)

  return (
    <Card className='flex flex-col'>
      <CardHeader className='items-center pb-0'>
        <CardAction>
          <Button variant='outline'>
            <Download className='mr-2 h-4 w-4' />
            Download
          </Button>
        </CardAction>
      </CardHeader>
      <CardContent className='flex-1 pb-0'>
        <ChartContainer config={chartConfig} className='mx-auto aspect-square'>
          <PieChart>
            <Pie
              data={chartData}
              dataKey='count'
              nameKey='option'
              outerRadius={100}
              paddingAngle={2}
              stroke='#fff'
              strokeWidth={2}
              isAnimationActive
              // label={({ name, value }) =>
              //   `${name}: ${value}${total ? ` (${Math.round((value / total) * 100)}%)` : ''}`
              // }
              labelLine={false}
            >
              {chartData.map((entry, index) => (
                <Cell
                  key={`cell-${index}`}
                  fill={colors[index % colors.length]}
                  stroke='#fff'
                  strokeWidth={2}
                />
              ))}
            </Pie>
            <Tooltip
              formatter={(value: number, name: string) => [
                `${value} (${total ? Math.round((value / total) * 100) : 0}%)`,
                name,
              ]}
            />
            <ChartLegend
              content={<ChartLegendContent nameKey='option' />}
              className='mt-2 flex-wrap justify-center gap-2'
            />
          </PieChart>
        </ChartContainer>
      </CardContent>
    </Card>
  )
}
