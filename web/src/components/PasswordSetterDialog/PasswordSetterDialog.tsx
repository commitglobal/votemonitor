import { Button } from '@/components/ui/button';
import { Dialog, DialogClose, DialogContent, DialogFooter, DialogTitle } from '@/components/ui/dialog';
import { Form, FormField, FormItem, FormLabel, FormMessage } from '@/components/ui/form';
import { PasswordInput } from '@/components/ui/password-input';
import { UseFormReturn } from 'react-hook-form';
import { PasswordSetterFormData } from './usePasswordSetterDialog';

export interface CreateNGODialogProps {
  displayName: string;
  open: boolean;
  internalOnOpenChange: (open: any) => void;
  form: UseFormReturn<PasswordSetterFormData>;
  onSubmit: (values: PasswordSetterFormData) => void;
}

function PasswordSetterDialog({ displayName, open, form, internalOnOpenChange, onSubmit }: CreateNGODialogProps) {
  return (
    <Dialog open={open} onOpenChange={internalOnOpenChange} modal={true}>
      <DialogContent
        className='min-w-[650px]'
        onInteractOutside={(e) => {
          e.preventDefault();
        }}
        onEscapeKeyDown={(e) => {
          e.preventDefault();
        }}>
        <DialogTitle className='mb-3.5'>Set password for {displayName}</DialogTitle>
        <div className='flex flex-col gap-3'>
          <Form {...form}>
            <form onSubmit={form.handleSubmit(onSubmit)} className='space-y-4'>
              <FormField
                control={form.control}
                name='newPassword'
                render={({ field, fieldState }) => (
                  <FormItem>
                    <FormLabel>Password</FormLabel>
                    <PasswordInput placeholder='Password' {...field} {...fieldState} />

                    <FormMessage />
                  </FormItem>
                )}
              />

              <FormField
                control={form.control}
                name='confirmNewPassword'
                render={({ field, fieldState }) => (
                  <FormItem>
                    <FormLabel>Confirm password</FormLabel>
                    <PasswordInput placeholder='Confirm password' {...field} {...fieldState} />

                    <FormMessage />
                  </FormItem>
                )}
              />

              <DialogFooter>
                <DialogClose asChild>
                  <Button className='text-purple-900 border border-purple-900 border-input bg-background hover:bg-purple-50 hover:text-purple-600'>
                    Cancel
                  </Button>
                </DialogClose>
                <Button title='Set password' type='submit' className='px-6'>
                  Set password
                </Button>
              </DialogFooter>
            </form>
          </Form>
        </div>
      </DialogContent>
    </Dialog>
  );
}

export default PasswordSetterDialog;
