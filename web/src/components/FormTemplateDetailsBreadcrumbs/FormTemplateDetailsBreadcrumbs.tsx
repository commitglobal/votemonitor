import type { FunctionComponent } from '@/common/types';
import { Link } from '@tanstack/react-router';

export interface FormTemplateDetailsBreadcrumbsProps {
  formCode: string;
  formName: string;
}

export function FormTemplateDetailsBreadcrumbs({ formCode, formName }: FormTemplateDetailsBreadcrumbsProps): FunctionComponent {
  return (
    <div className='breadcrumbs flex flex-row gap-2 mb-4'>
      <Link className='crumb' to='/form-templates' preload='intent'>
      form-templates
      </Link>
      <Link className='crumb' to='.'>
        {formCode} - {formName}
      </Link>
    </div>
  );
}
