import { Button } from '@/components/ui/button';
import { Dialog, DialogClose, DialogContent, DialogFooter, DialogHeader, DialogTitle } from '@/components/ui/dialog';
import { Separator } from '@/components/ui/separator';
import { GuideModel, GuidePageType } from '../../models/guide';

import EditGuideForm from './EditGuideForm';

export interface EditGuideDialogProps {
  open: boolean;
  onOpenChange: (open: any) => void;
  guide: GuideModel;
  guidePageType: GuidePageType;
}

function EditGuideDialog({ guide, guidePageType, open, onOpenChange }: EditGuideDialogProps) {
  debugger;
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
          <DialogTitle className='mb-3.5'>Update guide</DialogTitle>
          <Separator />
        </DialogHeader>
        <div className='flex flex-col gap-3 max-w-[600px]'>
          <EditGuideForm
            guideId={guide.id}
            guidePageType={guidePageType}
            guideType={guide.guideType}
            onSuccess={() => onOpenChange(false)}
            onError={() => onOpenChange(false)}>
            <DialogFooter>
              <DialogClose asChild>
                <Button className='text-purple-900 border border-purple-900 border-input bg-background hover:bg-purple-50 hover:text-purple-600'>
                  Cancel
                </Button>
              </DialogClose>
              <Button className='bg-purple-900 hover:bg-purple-600'>Update guide</Button>
            </DialogFooter>
          </EditGuideForm>
        </div>
      </DialogContent>
    </Dialog>
  );
}

export default EditGuideDialog;
