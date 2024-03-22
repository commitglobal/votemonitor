import type { ReactNode } from "react";

interface Props {
  header?: ReactNode;
  footer?: ReactNode;
  children: ReactNode;
}

const Panel = ({header, footer, children } : Props): ReactNode => {
  return (
    <div className="overflow-hidden bg-white divide-y divide-gray-200 rounded-lg shadow">
      {!!header && (
        <div className="px-4 py-5 sm:px-6">
          {header}
        </div>
      )}
      <div className="px-4 py-5 sm:p-6">{children}</div>
      {!!footer && (
        <div className="px-4 py-4 sm:px-6">
          {footer}
        </div>
      )}
    </div>
  )
}

export default Panel;
