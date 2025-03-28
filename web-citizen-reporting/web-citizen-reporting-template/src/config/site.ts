export const siteConfig = {
  name: "VM Data Viz",
  url: "TBD",
  ogImage: "TBD",
  description: "a really nice descripiton",
  links: {
    github: "https://github.com/idormenco/vote-monitor-data-viz",
  },
};

export type SiteConfig = typeof siteConfig;

export const META_THEME_COLORS = {
  light: "#ffffff",
  dark: "#09090b",
};

export const typographyClasses = {
  h1: "scroll-m-20 text-4xl font-extrabold tracking-tight lg:text-5xl",
  p: "leading-7 [&:not(:first-child)]:mt-6",
  h2: "mt-10 scroll-m-20 border-b pb-2 text-3xl font-semibold tracking-tight transition-colors first:mt-0",
  a: "font-medium text-primary underline underline-offset-4",
  blockquote: "mt-6 border-l-2 pl-6 italic",
  h3: "mt-8 scroll-m-20 text-2xl font-semibold tracking-tight",
  ul: "my-6 ml-6 list-disc [&>li]:mt-2",
  table: "w-full",
  tr: "m-0 border-t p-0 even:bg-muted",
  th: "border px-4 py-2 text-left font-bold [&[align=center]]:text-center [&[align=right]]:text-right",
  td: "border px-4 py-2 text-left [&[align=center]]:text-center [&[align=right]]:text-right",
};