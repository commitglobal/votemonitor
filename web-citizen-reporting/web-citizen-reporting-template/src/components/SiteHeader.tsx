import MainNav from "./MainNav";
import { MobileNav } from "./MobileNav";
import { ModeSwitcher } from "./ModeSwitcher";

export function SiteHeader() {
  return (
    <header className="border-grid sticky top-0 z-50 w-full border-b bg-background/95 backdrop-blur supports-[backdrop-filter]:bg-background/60">
      <div className="container-wrapper">
        <div className="container flex h-14 items-center gap-2 md:gap-4">
          <MainNav />
          <MobileNav />
          <div className="ml-auto flex items-center gap-2 md:flex-1 md:justify-end">
            <nav className="flex items-center gap-0.5">
              <ModeSwitcher />
            </nav>
          </div>
        </div>
      </div>
    </header>
  );
}
