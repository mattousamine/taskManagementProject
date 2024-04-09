export interface TaskItem {
    id?: number;
    name: string;
    description: string;
    priority: 'red' | 'green' | 'blue';
    status: 'ongoing' | 'completed';
    categoryId?: number;
    userId?: number;
  }
  