import { DataTable } from './DataTable';

export default {
  component: DataTable,
  title: 'DataTable',
  tags: ['datatable', 'table'],
};

export const Default = {
  args: {
    task: {
      id: '1',
      title: 'Test Task',
      state: 'TASK_INBOX',
    },
  },
};
