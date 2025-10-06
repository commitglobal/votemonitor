import { Separator } from "@/components/ui/separator";
import { H3, P } from "@/components/ui/typography";

type ContentSectionProps = {
  title: string;
  desc?: string;
  children: React.JSX.Element;
};

export function ContentSection({ title, desc, children }: ContentSectionProps) {
  return (
    <div className="flex flex-1 flex-col w-full">
      <div>
        <H3>{title}</H3>
        {desc && <P>{desc}</P>}
      </div>
      <Separator className="my-4" />
      <div className="flex w-full flex-col gap-2.5 overflow-auto">
        {children}
      </div>
    </div>
  );
}
