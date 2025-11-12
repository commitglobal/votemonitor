import { Link } from "@tanstack/react-router";
import { Icons } from "./Icons";
import { cn } from "@/lib/utils";

export default function MainNav() {
  return (
    <div className="mr-4 hidden md:flex">
      <Link to="/" className="mr-4 flex items-center gap-2 lg:mr-6">
        <Icons.logo className="h-8 w-8" />
        <span className="hidden font-bold lg:inline-block">
          Citizen reporting template
        </span>
      </Link>
      <nav className="flex items-center gap-4 text-sm xl:gap-6">
        <Link
          to="/"
          className={cn("transition-colors hover:text-foreground/80")}
          activeProps={{
            className: "text-foreground font-bold",
          }}
          preload={"intent"}
        >
          Home
        </Link>
        <Link
          to="/forms"
          className={cn(
            "transition-colors hover:text-foreground/80 text-foreground/80"
          )}
          activeProps={{
            className: "text-foreground font-bold",
          }}
          preload={"intent"}
        >
          Forms
        </Link>
        <Link
          to="/guides"
          className={cn(
            "transition-colors hover:text-foreground/80 text-foreground/80"
          )}
          activeProps={{
            className: "text-foreground font-bold",
          }}
          preload={"intent"}
        >
          Guides
        </Link>
        <Link
          to="/notifications"
          className={cn(
            "transition-colors hover:text-foreground/80 text-foreground/80"
          )}
          activeProps={{
            className: "text-foreground font-bold",
          }}
          preload={"intent"}
        >
          Notifications
        </Link>
        <Link
          to="/about"
          className={cn(
            "transition-colors hover:text-foreground/80 text-foreground/80"
          )}
          activeProps={{
            className: "text-foreground font-bold",
          }}
          preload={"intent"}
        >
          About
        </Link>
      </nav>
    </div>
  );
}
