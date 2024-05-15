import { authApi } from '@/common/auth-api';

export const downloadImportExample = async () => {
    const res = await authApi.get('/monitoring-observers:import-template');
    const csvData = res.data;

    const blob = new Blob([csvData], { type: 'text/csv' });
    const url = window.URL.createObjectURL(blob);

    const a = document.createElement('a');
    a.style.display = 'none';
    a.href = url;
    a.download = 'import-template.csv';

    document.body.appendChild(a);
    a.click();

    window.URL.revokeObjectURL(url);
  };
