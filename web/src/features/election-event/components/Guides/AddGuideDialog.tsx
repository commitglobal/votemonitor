import { Dialog, DialogClose, DialogContent, DialogFooter, DialogHeader, DialogTitle } from '@/components/ui/dialog';
import { Separator } from '@/components/ui/separator';
import { GuidePageType, GuideType } from '../../models/guide';
import AddGuideForm from './AddGuideForm';
import { Button } from '@/components/ui/button';

export interface AddGuideDialogProps {
  open: boolean;
  onOpenChange: (open: any) => void;
  guideType: GuideType;
  guidePageType: GuidePageType;
}

function AddGuideDialog({ guidePageType, guideType, open, onOpenChange }: AddGuideDialogProps) {
  return (
    <Dialog open={open} onOpenChange={onOpenChange} modal={true}>
      <DialogContent
        className='max-w-[650px]'
        onInteractOutside={(e) => {
          e.preventDefault();
        }}
        onEscapeKeyDown={(e) => {
          e.preventDefault();
        }}>
        <DialogHeader>
          <DialogTitle className='mb-3.5'>New guide</DialogTitle>
          <Separator />
        </DialogHeader>
        <div className='flex flex-col gap-3 max-w-[600px]'>
          <AddGuideForm
            onSuccess={() => onOpenChange(false)}
            onError={() => onOpenChange(false)}
            guideType={guideType}
            guidePageType={guidePageType}>
            <DialogFooter>
              <DialogClose asChild>
                <Button className='text-purple-900 border border-purple-900 border-input bg-background hover:bg-purple-50 hover:text-purple-600'>
                  Cancel
                </Button>
              </DialogClose>
              <Button className='bg-purple-900 hover:bg-purple-600'>Upload guide</Button>
            </DialogFooter>
          </AddGuideForm>
        </div>
      </DialogContent>
    </Dialog>
  );
}

export default AddGuideDialog;
