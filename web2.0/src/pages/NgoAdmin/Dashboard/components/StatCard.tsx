import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'

type StatCardProps = {
  title: string
  /** Already formatted, so the caller decides rounding and separators. */
  value: string
  /** Short qualifier under the number, such as "hours" or "of 1,248". */
  caption?: string
}

/**
 * A single headline number.
 *
 * A lone value is not chart material: a one-bar chart would spend a whole card
 * to say what the number already says. The figure uses the font's proportional
 * digits on purpose, since tabular figures make a large standalone number look
 * loose.
 */
export function StatCard({ title, value, caption }: StatCardProps) {
  return (
    <Card>
      <CardHeader>
        <CardTitle className='text-sm font-medium'>{title}</CardTitle>
      </CardHeader>
      <CardContent>
        <p className='text-4xl font-semibold tracking-tight'>{value}</p>
        {caption ? (
          <p className='text-muted-foreground mt-1 text-sm'>{caption}</p>
        ) : null}
      </CardContent>
    </Card>
  )
}
