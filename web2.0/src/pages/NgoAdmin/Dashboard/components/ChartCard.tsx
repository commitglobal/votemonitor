import { useRef, useState, type ReactNode } from 'react'
import { Copy, Download, Maximize2 } from 'lucide-react'
import { useCopyToClipboard } from '@/hooks/use-copy-to-clipboard'
import { Button } from '@/components/ui/button'
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
} from '@/components/ui/dialog'
import { ScrollArea } from '@/components/ui/scroll-area'
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from '@/components/ui/table'
import { Tabs, TabsContent, TabsList, TabsTrigger } from '@/components/ui/tabs'
import { downloadChartAsPng } from '../utils/download-chart'

export type ChartColumn = {
  key: string
  label: string
}

type ChartRow = Record<string, string | number>

type ChartCardProps = {
  title: string
  /** Headline value and period, repeated inside the dialog. */
  summary?: ReactNode
  columns: ChartColumn[]
  rows: ChartRow[]
  /** Base name of the exported file, without extension. */
  fileName: string
  children: ReactNode
}

/**
 * Wraps a chart with the three things every chart on this dashboard offers:
 * saving it as an image, opening it large, and reading the numbers behind it.
 *
 * The data tab is not a nicety. Several of the theme's chart colours sit below
 * the 3:1 contrast the palette checks ask for, and the rule there is that a
 * readable table has to exist alongside — so the numbers are never gated
 * behind the ability to tell two hues apart.
 */
export function ChartCard({
  title,
  summary,
  columns,
  rows,
  fileName,
  children,
}: ChartCardProps) {
  const [isMaximized, setIsMaximized] = useState(false)
  const { copy } = useCopyToClipboard()

  // Two separate references: the card holds one copy of the chart and the
  // dialog another, and each download button must export the one it sits next to.
  const cardChartRef = useRef<HTMLDivElement>(null)
  const dialogChartRef = useRef<HTMLDivElement>(null)

  const copyRows = () => {
    // Tab separated with a header row, which is what spreadsheets expect from
    // the clipboard: pasting lands each value in its own cell.
    const header = columns.map((column) => column.label).join('\t')
    const body = rows
      .map((row) => columns.map((column) => row[column.key] ?? '').join('\t'))
      .join('\n')

    void copy(`${header}\n${body}`, { withToast: true })
  }

  return (
    <>
      <Card>
        <CardHeader>
          <div className='flex items-start justify-between gap-2'>
            <div>
              <CardTitle className='text-sm font-medium'>{title}</CardTitle>
              {summary}
            </div>
            <div className='flex shrink-0 items-center'>
              <Button
                type='button'
                variant='ghost'
                size='icon'
                aria-label={`Download ${title} as image`}
                onClick={() =>
                  downloadChartAsPng(cardChartRef.current, `${fileName}.png`)
                }
              >
                <Download className='size-4' />
              </Button>
              <Button
                type='button'
                variant='ghost'
                size='icon'
                aria-label={`Maximize ${title}`}
                onClick={() => setIsMaximized(true)}
              >
                <Maximize2 className='size-4' />
              </Button>
            </div>
          </div>
        </CardHeader>
        <CardContent ref={cardChartRef}>{children}</CardContent>
      </Card>

      <Dialog open={isMaximized} onOpenChange={setIsMaximized}>
        <DialogContent className='sm:max-w-4xl'>
          <DialogHeader>
            <DialogTitle>{title}</DialogTitle>
            <DialogDescription className='sr-only'>
              The chart at full size, and the numbers behind it.
            </DialogDescription>
            {summary}
          </DialogHeader>

          <Tabs defaultValue='chart'>
            <div className='flex items-center justify-between gap-2'>
              <TabsList>
                <TabsTrigger value='chart'>Chart</TabsTrigger>
                <TabsTrigger value='data'>Data</TabsTrigger>
              </TabsList>
            </div>

            <TabsContent value='chart' className='mt-4'>
              <div ref={dialogChartRef}>{children}</div>
              <div className='mt-4 flex justify-end'>
                <Button
                  type='button'
                  variant='outline'
                  onClick={() =>
                    downloadChartAsPng(
                      dialogChartRef.current,
                      `${fileName}.png`
                    )
                  }
                >
                  <Download className='mr-2 size-4' />
                  Download graph
                </Button>
              </div>
            </TabsContent>

            <TabsContent value='data' className='mt-4'>
              <ScrollArea className='max-h-[50vh]'>
                <Table>
                  <TableHeader>
                    <TableRow>
                      {columns.map((column) => (
                        <TableHead key={column.key}>{column.label}</TableHead>
                      ))}
                    </TableRow>
                  </TableHeader>
                  <TableBody>
                    {rows.length === 0 ? (
                      <TableRow>
                        <TableCell
                          colSpan={columns.length}
                          className='text-muted-foreground text-center'
                        >
                          No data yet
                        </TableCell>
                      </TableRow>
                    ) : (
                      rows.map((row, index) => (
                        <TableRow key={index}>
                          {columns.map((column) => (
                            <TableCell
                              key={column.key}
                              // Tabular figures so the digits line up down the
                              // column, which is the one place they belong.
                              className='tabular-nums'
                            >
                              {row[column.key]}
                            </TableCell>
                          ))}
                        </TableRow>
                      ))
                    )}
                  </TableBody>
                </Table>
              </ScrollArea>
              <div className='mt-4 flex justify-end'>
                <Button
                  type='button'
                  variant='outline'
                  onClick={copyRows}
                  disabled={rows.length === 0}
                >
                  <Copy className='mr-2 size-4' />
                  Copy to clipboard
                </Button>
              </div>
            </TabsContent>
          </Tabs>
        </DialogContent>
      </Dialog>
    </>
  )
}
