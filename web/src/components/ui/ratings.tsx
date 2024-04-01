
import * as React from "react"
import * as RadioGroupPrimitive from "@radix-ui/react-radio-group"

import { cn } from "@/lib/utils"

interface RatingItemProps
  extends React.ComponentPropsWithoutRef<typeof RadioGroupPrimitive.Item> {
  selectedValue: number | null
}

const RatingItem = React.forwardRef<
  React.ElementRef<typeof RadioGroupPrimitive.Item>,
  RatingItemProps
>(({ className, value, selectedValue, ...props }, ref) => {
  return (
    <RadioGroupPrimitive.Item
      ref={ref}
      value={value}
      className={cn(
        "aspect-square fill-transparent px-1.5 text-primary ring-offset-background focus:outline-none focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2 disabled:cursor-not-allowed disabled:opacity-50 [&>svg]:stroke-primary",
        props["aria-readonly"] && "pointer-events-none",
        className
      )}
      {...props}
    >
      <div className={cn((selectedValue ?? -1) >= Number(value) && "bg-red-950")}>{value}</div>
    </RadioGroupPrimitive.Item>
  )
})

RatingItem.displayName = RadioGroupPrimitive.Item.displayName

interface RatingGroupProps
  extends React.ComponentPropsWithoutRef<typeof RadioGroupPrimitive.Root> {
  ratingSteps?: number
}

const RatingGroup = React.forwardRef<
  React.ElementRef<typeof RadioGroupPrimitive.Root>,
  RatingGroupProps
>(
  (
    {
      className,
      ratingSteps = 5,
      ...props
    },
    ref
  ) => {
    const [selectedValue, setSelectedValue] = React.useState<number | null>(null)

    return (
      <RadioGroupPrimitive.Root
        className={cn(
          "flex items-center",
          props.disabled && "pointer-events-none",
          className
        )}
        {...props}
        ref={ref}
        onValueChange={(value) => {
          setSelectedValue(Number(value))
          props.onValueChange && props.onValueChange(value)
        }}
        tabIndex={0}
      >
        {Array.from({ length: ratingSteps }, (_, i) => i + 1).map((value) => (
          <RatingItem
            key={value}
            value={value.toString()}
            selectedValue={selectedValue}
          />
        ))}
      </RadioGroupPrimitive.Root>
    )
  }
)
RatingGroup.displayName = RadioGroupPrimitive.Root.displayName

export { RatingGroup, RatingItem }