import { siteConfig } from "@/config/site";
import { Link } from "@tanstack/react-router";
import { Icons } from "./Icons";
import MainNav from "./MainNav";
import { ModeSwitcher } from "./ModeSwitcher";
import { ProfileDropdown } from "./ProfileDropdown";
import { Button } from "./ui/button";

export function SiteHeader() {
  return (
    <header className="border-grid sticky top-0 z-50 w-full border-b bg-background/95 backdrop-blur supports-[backdrop-filter]:bg-background/60">
      <div className="container-wrapper">
        <div className="container flex h-14 items-center gap-2 md:gap-4">
          <MainNav />
          <div className="ml-auto flex items-center gap-2 md:flex-1 md:justify-end">
            <nav className="flex items-center gap-0.5">
              <Button
                asChild
                variant="ghost"
                size="icon"
                className="h-8 w-8 px-0"
              >
                <Link
                  to={siteConfig.links.github}
                  target="_blank"
                  rel="noreferrer"
                >
                  <Icons.gitHub className="h-4 w-4" />
                  <span className="sr-only">GitHub</span>
                </Link>
              </Button>
              <ModeSwitcher />
              <ProfileDropdown />
            </nav>
          </div>
        </div>
      </div>
    </header>
  );
}
