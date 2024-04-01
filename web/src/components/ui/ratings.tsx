
import * as React from "react"
import * as RadioGroupPrimitive from "@radix-ui/react-radio-group"

import { cn } from "@/lib/utils"

interface RatingItemProps
  extends React.ComponentPropsWithoutRef<typeof RadioGroupPrimitive.Item> {
  selectedValue: number | null;
  scale: number
}

const RatingItem = React.forwardRef<
  React.ElementRef<typeof RadioGroupPrimitive.Item>,
  RatingItemProps
>(({ className, value, selectedValue, scale, ...props }, ref) => {
  return (
    <RadioGroupPrimitive.Item
      ref={ref}
      value={value}
      className={cn(
        Number(value) === 1 && "rounded-l-md",
        "relative inline-flex items-center px-4 py-2 text-sm font-semibold text-gray-900 ring-1 ring-inset ring-gray-300 focus:z-20 focus:outline-offset-0",
        (selectedValue ?? -1) === Number(value) && "z-10 bg-indigo-600 text-white focus-visible:outline focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-indigo-600",
        Number(value) === scale && "rounded-r-md",
        className
      )}
      {...props}
    >
      <div>{value}</div>
    </RadioGroupPrimitive.Item>
  )
})

RatingItem.displayName = RadioGroupPrimitive.Item.displayName

interface RatingGroupProps
  extends React.ComponentPropsWithoutRef<typeof RadioGroupPrimitive.Root> {
  scale?: number
}

const RatingGroup = React.forwardRef<
  React.ElementRef<typeof RadioGroupPrimitive.Root>,
  RatingGroupProps
>(
  (
    {
      className,
      scale = 5,
      ...props
    },
    ref
  ) => {
    const [selectedValue, setSelectedValue] = React.useState<number | null>(null)

    return (
      <RadioGroupPrimitive.Root
        className={cn(
          "isolate inline-flex -space-x-px rounded-md shadow-sm",
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
        {Array.from({ length: scale }, (_, i) => i + 1).map((value) => (
          <RatingItem
            key={value}
            value={value.toString()}
            selectedValue={selectedValue}
            scale={scale}
          />
        ))}
      </RadioGroupPrimitive.Root>
    )
  }
)
RatingGroup.displayName = RadioGroupPrimitive.Root.displayName

export { RatingGroup, RatingItem }