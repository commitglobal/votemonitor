import { authApi } from '@/common/auth-api';
import { Button } from '@/components/ui/button';
import { Dialog, DialogClose, DialogContent, DialogFooter, DialogHeader, DialogTitle } from '@/components/ui/dialog';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { Separator } from '@/components/ui/separator';
import { toast } from '@/components/ui/use-toast';
import { queryClient } from '@/main';
import { useMutation } from '@tanstack/react-query';
import { useRef, useState } from 'react';
import {
    AlertDialog,
    AlertDialogAction,
    AlertDialogCancel,
    AlertDialogContent,
    AlertDialogDescription,
    AlertDialogFooter,
    AlertDialogHeader,
    AlertDialogTitle,
} from "@/components/ui/alert-dialog"

export interface ConfirmDeleteDialogProps {
    alertTitle: string;
    alertDescription?: string;
    confirmActionButtonText: string;
    cancelActionButtonText: string;
    open: boolean;
    onConfirm: () => void;
    onCancel?: () => void;
    onOpenChange: (open: any) => void;
}

function ConfirmDeleteDialog({
    alertTitle,
    alertDescription,
    confirmActionButtonText,
    cancelActionButtonText,
    open,
    onConfirm,
    onCancel,
    onOpenChange
}: ConfirmDeleteDialogProps) {

    return (
        <AlertDialog open={open} onOpenChange={onOpenChange}>
            <AlertDialogContent>
                <AlertDialogHeader>
                    <AlertDialogTitle>{alertTitle}</AlertDialogTitle>
                    {!!alertDescription && <AlertDialogDescription>{alertDescription}</AlertDialogDescription>}
                </AlertDialogHeader>
                <AlertDialogFooter>
                    <AlertDialogCancel onClick={onCancel}>{cancelActionButtonText}</AlertDialogCancel>
                    <AlertDialogAction onClick={onConfirm} className='bg-red-600 hover:bg-red-900'>{confirmActionButtonText}</AlertDialogAction>
                </AlertDialogFooter>
            </AlertDialogContent>
        </AlertDialog>
    )
}

export default ConfirmDeleteDialog;