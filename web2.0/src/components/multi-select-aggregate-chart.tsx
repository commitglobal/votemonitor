'use client'

import { useMemo } from 'react'
import { MultiSelectQuestionAggregateModel } from '@/types/submissions-aggregate'
import { Download } from 'lucide-react'
import { Bar, BarChart, CartesianGrid, LabelList, XAxis, YAxis } from 'recharts'
import { getTranslation } from '@/lib/translated-string'
import { Card, CardAction, CardContent, CardHeader } from '@/components/ui/card'
import {
  ChartConfig,
  ChartContainer,
  ChartLegend,
  ChartLegendContent,
  ChartTooltip,
  ChartTooltipContent,
} from '@/components/ui/chart'
import { Button } from './ui/button'

export const description = 'A horizontal bar chart'

export function MultiSelectAggregateChart({
  language,
  aggregate,
}: {
  language: string
  aggregate: MultiSelectQuestionAggregateModel
}) {
  const colors = ['#ff0000', '#00ff00', '#0000ff', '#ffff00', '#ff00ff']

  const chartData = useMemo(() => {
    return aggregate.question.options.map((option) => {
      const translatedLabel = getTranslation(option.text, language)
      const count = aggregate.answersHistogram[option.id] ?? 0
      return {
        name: translatedLabel,
        [translatedLabel]: count,
      }
    })
  }, [aggregate, language])

  console.log(chartData)

  // Build chart config for legend + colors
  const chartConfig = useMemo(() => {
    return aggregate.question.options.reduce((acc, option, index) => {
      const translatedLabel = getTranslation(option.text, language)
      acc[translatedLabel] = {
        label: translatedLabel,
        color: colors[index % colors.length],
      }
      return acc
    }, {} as ChartConfig)
  }, [aggregate, language])

  return (
    <Card>
      <CardHeader>
        <CardAction>
          <Button variant='outline'>
            <Download className='mr-2 h-4 w-4' />
            Download
          </Button>
        </CardAction>
      </CardHeader>
      <CardContent>
        <ChartContainer config={chartConfig}>
          <BarChart
            accessibilityLayer
            data={chartData}
            layout='vertical'
            margin={{ top: 20, right: 20, left: 20, bottom: 20 }}
            barCategoryGap='20%'
            barGap={0}
          >
            <CartesianGrid strokeDasharray='3 3' />
            <XAxis type='number' allowDecimals={false} />
            <YAxis
              dataKey='name'
              type='category'
              width={150}
              height={150}
              interval={0}
            />
            <ChartTooltip content={<ChartTooltipContent hideLabel />} />
            <ChartLegend content={<ChartLegendContent />} />

            {aggregate.question.options.map((option, index) => {
              const translatedLabel = getTranslation(option.text, language)
              return (
                <Bar
                  key={option.id}
                  dataKey={translatedLabel}
                  fill={colors[index % colors.length]}
                  radius={[0, 4, 4, 0]}
                >
                  <LabelList dataKey={translatedLabel} position='right' />
                </Bar>
              )
            })}
          </BarChart>
        </ChartContainer>
      </CardContent>
    </Card>
  )
}
