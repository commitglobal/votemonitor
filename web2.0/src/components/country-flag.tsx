import * as Flags from "country-flag-icons/react/3x2";
import type { SVGProps } from "react";
import { Badge } from "./ui/badge";

type CountryFlagProps = {
  code: string;
  className?: string;
};

export function CountryFlag({ code, className }: CountryFlagProps) {
  const key = code.toUpperCase();
  const Flag = (
    Flags as Record<string, React.ComponentType<SVGProps<SVGSVGElement>>>
  )[key];

  if (!Flag) {
    // Fallback if code is unknown
    return <Badge className={className}>{key}</Badge>;
  }

  return <Flag className={className} />;
}
