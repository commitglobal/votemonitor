import { siteConfig } from "@/config/site";
import { Icons } from "./Icons";
import MainNav from "./MainNav";
import { MobileNav } from "./MobileNav";
import { Button } from "./ui/button";
import { Link } from "@tanstack/react-router";
import { ModeSwitcher } from "./ModeSwitcher";
import { useAuth } from "@/contexts/auth.context";
import NgoAdminNav from "./NgoAdminNav";
import PlatformAdminNav from "./PlatformAdminNav";

export interface ElectionSiteHeaderProps {
  electionId: string;
}
export function ElectionSiteHeader({ electionId }: ElectionSiteHeaderProps) {
  const auth = useAuth();
  return (
    <header className="border-grid sticky top-0 z-50 w-full border-b bg-background/95 backdrop-blur supports-[backdrop-filter]:bg-background/60">
      <div className="container-wrapper">
        <div className="container flex h-14 items-center gap-2 md:gap-4">
          <div className="flex items-center">
            <MainNav />
            <span className="ml-2">/</span>
            <Button asChild variant="link">
              <Link
                to="/elections/$electionRoundId"
                params={{ electionRoundId: electionId }}
              >
                Election name goes here
              </Link>
            </Button>
          </div>
          <MobileNav />
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
              <Button
                variant={"link"}
                className="cursor-pointer"
                onClick={() => auth.logout()}
              >
                log out
              </Button>
            </nav>
          </div>
        </div>
        <div className="container flex items-center gap-2 md:gap-4">
          {auth.userRole === "NgoAdmin" ? (
            <NgoAdminNav electionRoundId={electionId} />
          ) : (
            <PlatformAdminNav electionRoundId={electionId} />
          )}

          <MobileNav />
        </div>
      </div>
    </header>
  );
}
