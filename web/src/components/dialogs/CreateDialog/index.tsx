import type { ReactNode } from "react";
import { PlusIcon } from '@heroicons/react/24/outline';
import { Button } from '@/components/ui/button';
import {
  Dialog,
  DialogClose,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from '@/components/ui/dialog';
import { useTranslation } from "react-i18next";

interface Props {
  title?: ReactNode;
  description?: ReactNode;
  children: ReactNode;
}

const CreateDialog = ({ title, description, children }: Props): ReactNode => {
  const { t } = useTranslation();

  return (
    <Dialog>
      <DialogTrigger asChild>
        <Button variant="default">
          <PlusIcon className="w-5 h-5 mr-2 -ml-1.5" />
          <span>{title}</span>
        </Button>
      </DialogTrigger>

      <DialogContent className="sm:max-w-2xl">
        <DialogHeader>
          <DialogTitle className="text-2xl leading-loose">{title}</DialogTitle>
          {description && <DialogDescription>{description}</DialogDescription>}
        </DialogHeader>

        {children}

        <DialogFooter>
          <DialogClose asChild>
            <Button type="button" variant="outline">
              {t('app.action.close')}
            </Button>
          </DialogClose>
          <Button variant="default" type="submit">
            {t('app.action.create')}
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
};

export default CreateDialog;
