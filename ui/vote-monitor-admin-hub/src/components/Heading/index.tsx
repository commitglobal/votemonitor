import React from "react";

const classNames = {
  h1: "text-6xl leading-none font-extrabold tracking-tight",
  h2: "text-4xl leading-10 font-bold",
  h3: "text-3xl leading-9 font-bold",
  h4: "text-2xl leading-8 font-bold",
  h5: "",
  h6: "",
};

interface HeadingProps extends React.HTMLAttributes<HTMLHeadingElement> {
  level: "h1" | "h2" | "h3" | "h4" | "h5" | "h6";
}

const Heading = ({ children, level }: HeadingProps) => (
  <div className={classNames[level]}>{children}</div>
);

export default Heading;
