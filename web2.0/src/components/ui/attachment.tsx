import { cn } from "@/lib/utils";
import { useState } from "react";
import ReactPlayer from "react-player";
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";
// Removed AspectRatio to better constrain image within modal

type AttachmentProps = {
  src: string;
  mimeType: string;
  fileName?: string;
  className?: string;
  width?: string | number;
  height?: string | number;
  controls?: boolean;
  loop?: boolean;
  muted?: boolean;
  playing?: boolean;
};

function isVideo(mime: string) {
  return mime.startsWith("video/");
}

function isAudio(mime: string) {
  return mime.startsWith("audio/");
}

function isImage(mime: string) {
  return mime.startsWith("image/");
}

export function Attachment({
  src,
  mimeType,
  fileName,
  className,
  width = "100%",
  height = "100%",
  controls = true,
  loop,
  muted,
  playing,
}: AttachmentProps) {
  const [open, setOpen] = useState(false);
  if (isImage(mimeType)) {
    return (
      // eslint-disable-next-line @next/next/no-img-element
      <>
        <img
          src={src}
          alt={fileName ?? "attachment"}
          className="aspect-square w-full rounded-sm object-cover cursor-pointer"
          style={{ width, height }}
          onClick={() => setOpen(true)}
        />
        <ImageModal
          open={open}
          onOpenChange={setOpen}
          src={src}
          alt={fileName ?? "attachment"}
        />
      </>
    );
  }

  if (isVideo(mimeType) || isAudio(mimeType)) {
    const Player = ReactPlayer as unknown as React.ComponentType<any>;
    return (
      <Player
        url={src}
        width={width}
        height={height}
        controls={controls}
        loop={loop}
        muted={muted}
        playing={playing}
        className={cn("overflow-hidden", className)}
      />
    );
  }

  return (
    <a
      href={src}
      target="_blank"
      rel="noreferrer"
      className={cn("underline", className)}
    >
      {fileName ?? src}
    </a>
  );
}

export default Attachment;

type ImageModalProps = {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  src: string;
  alt?: string;
};

function ImageModal({ open, onOpenChange, src, alt }: ImageModalProps) {
  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="p-0 max-w-5xl w-[min(90vw,80rem)]">
        <DialogHeader className="sr-only">
          <DialogTitle>{alt}</DialogTitle>
        </DialogHeader>
        <div className="flex items-center justify-center max-h-[80vh]">
          {/* eslint-disable-next-line @next/next/no-img-element */}
          <img
            src={src}
            alt={alt}
            className="max-h-[80vh] max-w-full object-contain rounded-md"
          />
        </div>
      </DialogContent>
    </Dialog>
  );
}
