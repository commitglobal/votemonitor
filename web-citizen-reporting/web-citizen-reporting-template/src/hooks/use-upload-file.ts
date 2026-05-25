import { API } from "@/api/api";
import * as React from "react";
import { toast } from "sonner";

interface UseUploadFileOptions {
  defaultUploadedFiles?: File[];
}

export function useUploadFile({ defaultUploadedFiles }: UseUploadFileOptions) {
  const [uploadedFiles, setUploadedFiles] = React.useState<File[]>(
    defaultUploadedFiles ?? []
  );
  const [progresses, setProgresses] = React.useState<Record<string, number>>(
    {}
  );
  const [isUploading, setIsUploading] = React.useState(false);

  async function onUpload(files: File[]) {
    setIsUploading(true);
    try {
      //   const res = await API.postForm();
      //   setUploadedFiles((prev) => (prev ? [...prev, ...res] : res));
    } catch (err) {
      toast.error("Something went wrong, please try again later.");
    } finally {
      setProgresses({});
      setIsUploading(false);
    }
  }

  return {
    onUpload,
    uploadedFiles,
    progresses,
    isUploading,
  };
}
