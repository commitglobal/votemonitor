"use client"

// * * This is just a demostration of delete modal, actual functionality may vary

import {
    AlertDialog,
    AlertDialogCancel,
    AlertDialogContent,
    AlertDialogDescription,
    AlertDialogFooter,
    AlertDialogHeader,
    AlertDialogTitle,
  } from "@/components/ui/alert-dialog";
import { Button } from "@/components/ui/button";
import { ImportObserverRow } from "../MonitoringObserversImport";
  
  type DeleteProps = {
    observer: ImportObserverRow;
    isOpen: boolean;
    showActionToggle: (open: boolean) => void;
    deleteObserver: (observer:ImportObserverRow)=>void;
  };
  
  export default function DeleteDialog({
    observer,
    isOpen,
    showActionToggle,
    deleteObserver
  }: DeleteProps) {
    return (
      <AlertDialog open={isOpen} onOpenChange={showActionToggle}>
        <AlertDialogContent>
          <AlertDialogHeader>
            <AlertDialogTitle>Are you sure absolutely sure ?</AlertDialogTitle>
            <AlertDialogDescription>
              This action cannot be undone. You are about to delete <b>{observer.firstName}</b> <b>{observer.lastName}</b> <b>{observer.email}</b>
            </AlertDialogDescription>
          </AlertDialogHeader>
          <AlertDialogFooter>
            <AlertDialogCancel>Cancel</AlertDialogCancel>
            <Button
              variant='destructive'
              onClick={() => {
                deleteObserver(observer);
                showActionToggle(false);
              }}
            >
              Delete
            </Button>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>
    );
  }