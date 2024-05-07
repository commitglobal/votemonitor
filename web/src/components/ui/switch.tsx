import * as SwitchPrimitive from '@radix-ui/react-switch';
import { forwardRef } from 'react';

const Switch = forwardRef<HTMLButtonElement, SwitchPrimitive.SwitchProps>((props, ref) => {
  return (
    <div className='flex gap-2'>
      <SwitchPrimitive.Root
        {...props}
        ref={ref}
        className='w-11 h-6 bg-slate-300 relative rounded-full outline-none data-[state=checked]:bg-purple-500'>
        <SwitchPrimitive.Thumb className='w-5 h-5 block rounded-full bg-white translate-x-[2px] data-[state=checked]:translate-x-[22px] duration-100 transition-transform will-change-transform' />
      </SwitchPrimitive.Root>
      {props.children && (
        <label className='text-base font-normal text-purple-500' htmlFor={props.id}>
          {props.children}
        </label>
      )}
    </div>
  );
});

export { Switch };
