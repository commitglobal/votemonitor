'use client';

import * as React from 'react';
import * as SliderPrimitive from '@radix-ui/react-slider';

import { cn } from '@/lib/utils';
import { differenceInHours, subHours, addHours, roundToNearestHours } from 'date-fns';

export interface TimelineIntervalValue {
  start: Date;
  end: Date;
}

interface TimelineSliderProps {
  name?: string;
  disabled?: boolean;
  orientation?: React.AriaAttributes['aria-orientation'];
  start?: Date;
  end?: Date;
  value?: TimelineIntervalValue;
  defaultValue?: TimelineIntervalValue;
  onValueChange?(value: TimelineIntervalValue): void;
  onValueCommit?(value: TimelineIntervalValue): void;
  form?: string;
  labelFormatter: (value: Date) => string;
  className?: string | undefined;
}

const TimelineSlider = React.forwardRef<React.ElementRef<typeof SliderPrimitive.Root>, TimelineSliderProps>(
  ({ className, labelFormatter, start, end, value, defaultValue, onValueChange, onValueCommit, ...props }, ref) => {
    const rightEndpoint = end ?? roundToNearestHours(new Date(), { roundingMethod: 'ceil' });
    const leftEndpoint = start ?? subHours(rightEndpoint, 12);

    const initialValue = [
      differenceInHours(value?.start ?? leftEndpoint, leftEndpoint),
      differenceInHours(value?.end ?? rightEndpoint, leftEndpoint),
    ];

    const sliderValue = React.useMemo(() => {
      if (value) {
        return [differenceInHours(value.start, leftEndpoint), differenceInHours(value.end, leftEndpoint)];
      }

      return undefined;
    }, [value]);

    const sliderDefaultValue = React.useMemo(() => {
      if (defaultValue)
        return [differenceInHours(defaultValue.start, leftEndpoint), differenceInHours(defaultValue.end, leftEndpoint)];

      return undefined;
    }, [defaultValue]);

    function internalOnValueChange(value: number[]) {
      onValueChange?.({
        start: addHours(leftEndpoint, value[0] ?? 0),
        end: addHours(leftEndpoint, value[1] ?? 0),
      });
    }

    function internalOnValueCommit(value: number[]) {
      onValueCommit?.({
        start: addHours(leftEndpoint, value[0] ?? 0),
        end: addHours(leftEndpoint, value[1] ?? 0),
      });
    }

    return (
      <div className='flex justify-center w-full'>
        <SliderPrimitive.Root
          ref={ref}
          className={cn('relative flex w-[calc(100%-240px)] touch-none select-none items-center h-16', className)}
          {...props}
          value={sliderValue}
          step={1}
          min={0}
          max={12}
          minStepsBetweenThumbs={1}
          defaultValue={sliderDefaultValue}
          onValueChange={internalOnValueChange}
          onValueCommit={internalOnValueCommit}>
          <SliderPrimitive.Track className='relative w-full h-2 overflow-hidden rounded-full grow bg-[#DADADA]'>
            <SliderPrimitive.Range className='absolute h-full bg-[#7833B3]' />
          </SliderPrimitive.Track>
          {initialValue.map((value, index) => (
            <React.Fragment key={index}>
              <SliderPrimitive.Thumb className='block w-2 h-6 transition-colors border-2 rounded-md border-primary bg-background ring-offset-background focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2 disabled:pointer-events-none disabled:opacity-50'>
                {labelFormatter && (
                  <span
                    className={cn(
                      'absolute top-1/2 z-20 w-[120px]',
                      index === 0 && '-right-6',
                      index === 1 && 'left-6'
                    )}>
                    {labelFormatter(addHours(leftEndpoint, value))}
                  </span>
                )}
              </SliderPrimitive.Thumb>
            </React.Fragment>
          ))}
        </SliderPrimitive.Root>
      </div>
    );
  }
);
TimelineSlider.displayName = 'TimelineSlider';

export { TimelineSlider };
