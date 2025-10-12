import * as React from 'react'
import * as RadioGroupPrimitive from '@radix-ui/react-radio-group'
import { RatingScaleType } from '@/types/form'
import { cn } from '@/lib/utils'

function RatingGroupItem({
  className,
  value,
  ...props
}: React.ComponentProps<typeof RadioGroupPrimitive.Item>) {
  return (
    <RadioGroupPrimitive.Item
      value={value}
      data-slot='radio-group-item'
      className={cn(
        'first:rounded-l-md last:rounded-r-md',
        'border-input text-primary focus-visible:border-ring focus-visible:ring-ring/50 aria-invalid:ring-destructive/20 dark:aria-invalid:ring-destructive/40 aria-invalid:border-destructive aspect-square size-10 shrink-0 border shadow-xs transition-[color,box-shadow] outline-none focus-visible:ring-[3px]',
        'data-[state=checked]:bg-primary data-[state=checked]:text-primary-foreground',
        className
      )}
      {...props}
    >
      {value}
    </RadioGroupPrimitive.Item>
  )
}

export function ratingScaleToNumber(scale: RatingScaleType): number {
  switch (scale) {
    case RatingScaleType.OneTo3: {
      return 3
    }
    case RatingScaleType.OneTo4: {
      return 4
    }
    case RatingScaleType.OneTo5: {
      return 5
    }
    case RatingScaleType.OneTo6: {
      return 6
    }
    case RatingScaleType.OneTo7: {
      return 7
    }
    case RatingScaleType.OneTo8: {
      return 8
    }
    case RatingScaleType.OneTo9: {
      return 9
    }
    case RatingScaleType.OneTo10: {
      return 10
    }
    default: {
      return 5
    }
  }
}

function RatingGroup({
  scale,
  lowerLabel,
  upperLabel,
  className,
  ...props
}: {
  scale: RatingScaleType
  className?: string
  lowerLabel?: string
  upperLabel?: string
} & React.ComponentProps<typeof RadioGroupPrimitive.Root>) {
  const scaleNumber = ratingScaleToNumber(scale)

  console.log(props)
  return (
    <RadioGroupPrimitive.Root
      data-slot='radio-group'
      className={cn('grid gap-3', className)}
      {...props}
    >
      <div className='inline-flex w-full'>
        {Array.from({ length: scaleNumber }, (_, i) => i + 1).map((value) => (
          <RatingGroupItem key={value} value={value.toString()} />
        ))}
      </div>
      <p className='text-sm italic' dir='auto'>
        {lowerLabel && <> 1 - {lowerLabel}</>}
      </p>
      <p className='text-sm italic' dir='auto'>
        {upperLabel && (
          <>
            {scale} - {upperLabel}
          </>
        )}
      </p>
    </RadioGroupPrimitive.Root>
  )
}

export { RatingGroup }
