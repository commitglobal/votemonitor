import { Button } from "@/components/ui/button";
import { Link } from "@tanstack/react-router";
import { FileQuestion } from "lucide-react";
import { typographyClasses } from "../config/site";

export default function NotFound() {
  return (
    <div className="flex flex-col items-center justify-center min-h-[100dvh] px-4 text-center">
      <div className="space-y-6 max-w-md mx-auto">
        <FileQuestion className="h-24 w-24 mx-auto text-muted-foreground" />

        <div className="space-y-2">
          <h1 className={typographyClasses.h1}>
            404
          </h1>
          <h2 className={typographyClasses.h2}>
            Page not found
          </h2>
          <p className={typographyClasses.p}>
            Sorry, we couldn't find the page you're looking for. It might have
            been moved, deleted, or never existed.
          </p>
        </div>

        <div className="flex justify-center">
          <Button asChild>
            <Link to="/">Return Home</Link>
          </Button>
        </div>
      </div>
    </div>
  );
}
