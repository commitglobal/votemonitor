type IconProps = React.HTMLAttributes<SVGElement>;

export const Icons = {
  logo: (props: IconProps) => (
    <svg
      xmlns="http://www.w3.org/2000/svg"
      viewBox="0 0 148 147"
      {...props}
      fill="none"
    >
      <g fill="currentColor">
        <path d="M53.4689 91.4484H91.1516L72.3103 58.841L53.4689 91.4484ZM98.806 95.8638H45.8146L72.3103 50.0103L98.806 95.8638Z"></path>
        <path d="M81.3352 91.4484H119.018L100.177 58.841L81.3352 91.4484ZM126.672 95.8638H73.6808L100.176 50.0103L126.672 95.8638Z"></path>
        <path d="M23.6543 54.4158L42.4957 87.0231L61.337 54.4158H23.6543ZM42.4957 95.8535L16 50H68.9914L42.4957 95.8535Z"></path>
      </g>
    </svg>
  ),
  spinner: (props: IconProps) => (
    <svg
      xmlns="http://www.w3.org/2000/svg"
      width="24"
      height="24"
      viewBox="0 0 24 24"
      fill="none"
      stroke="currentColor"
      strokeWidth="2"
      strokeLinecap="round"
      strokeLinejoin="round"
      {...props}
    >
      <path d="M21 12a9 9 0 1 1-6.219-8.56" />
    </svg>
  ),
};
