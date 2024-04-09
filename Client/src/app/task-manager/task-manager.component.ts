export interface PriorityMappings {
  [key: string]: string;
}
export interface ChartData {
  [index: number]: [string, number]; // Defines a tuple of string and number
}
export const PRIORITY_TEXT: PriorityMappings = {
  red: 'High',
  green: 'Medium',
  blue: 'Low'
};

export const PRIORITY_ICON: PriorityMappings = {
  red: 'arrow_upward',
  green: 'remove',
  blue: 'arrow_downward'
};

export const STATUS_ICON: { [key: string]: string } = {
  ongoing: 'hourglass_empty',
  completed: 'check_circle'
};


import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { MatIconRegistry } from '@angular/material/icon';
import { DomSanitizer } from '@angular/platform-browser';
import { AuthService } from '../services/auth/auth.service'; 
import { Router } from '@angular/router';
import { firstValueFrom } from 'rxjs';
import { ChartType } from 'angular-google-charts';


// Import the TaskService
import { TaskService } from '../services/task/task.service';
import { TaskItem } from '../models/task-item.model';
import { Category } from '../models/category.model';


@Component({
  selector: 'app-task-manager',
  templateUrl: './task-manager.component.html',
  styleUrls: ['./task-manager.component.css']
})
export class TaskManagerComponent implements OnInit {
  tasks: TaskItem[] = [];
  categories: Category[] = [];
  newCategoryName: string = '';
  taskForm: FormGroup;
  priorityText = PRIORITY_TEXT;
  priorityIcon = PRIORITY_ICON;
  statusIcon = STATUS_ICON;

  filteredTasks: TaskItem[] = [];
  isFormVisible: boolean = false;

  currentPage: number = 1;
  pageSize: number = 10;
  totalTasks: number = 0;

  currentPriorityFilter: string | null = null;
  currentStatusFilter: string | null = null;
  currentCategoryFilter: string | null = null;

  chartData: [string, number][] = [];
  chartType: ChartType = ChartType.PieChart;
  chartOptions = {
    title: 'Tasks Status Distribution',
    is3D: true,
  };
  
  


  constructor(private taskService: TaskService, private fb: FormBuilder, private authService: AuthService, private router: Router) {
    // Initialize the form
    this.taskForm = this.fb.group({
      categoryId: ['', Validators.required],
      newCategory: [''],
      id: [''],
      name: ['', Validators.required],
      description: ['', Validators.required],
      priority: ['red', Validators.required], 
      status: ['ongoing', Validators.required],
      userId: ['']
    });
  }

  ngOnInit() {
    this.loadTasks(this.currentPage, this.pageSize);
    this.loadCategories();
  }

  toggleFormVisibility(): void {
    this.isFormVisible = !this.isFormVisible;
    if (!this.isFormVisible) {
      this.taskForm.reset(); 
    }
  }
  
  addCategory(event : Event): void {
    event.preventDefault();
    event.stopPropagation();
    const newCategoryName = this.newCategoryName;
  
  

    const newCategory: Category = { id:0, name: this.newCategoryName };
    this.taskService.createCategory(newCategory).subscribe({
      next: (category) => {
        this.loadCategories();
        this.newCategoryName = '';
      },
      error: (error) => {
        console.error('Error creating category:', error);
      }
    });
    
  }
  loadCategories(): void {

    this.taskService.getCategories().subscribe(categories => {
      this.categories = categories;
    });
  }

  

  async loadTasks(page: number, pageSize: number): Promise<void> {
    const user = JSON.parse(localStorage.getItem('user') || '{}');
  
    if (user && user.id) {
      try {
        // Convert the Observable to a Promise and await its resolution
        this.totalTasks = await firstValueFrom(this.taskService.getTasksCount(user.id));
  
        // Do the same for the tasks
        const tasks = await firstValueFrom(this.taskService.getTasks(user.id, page, pageSize));
        this.tasks = tasks;
        this.tasks = tasks.sort((a, b) => {
          if (a.status === 'ongoing' && b.status === 'completed') {
            return -1;
          }
          if (a.status === 'completed' && b.status === 'ongoing') {
            return 1;
          }
          return 0;
        });
        this.filteredTasks = [...this.tasks];
        this.updateChartData();
      } catch (error) {
        console.error('Error loading tasks:', error);
      }
    } else {
      console.error('User ID not found. Please ensure the user is signed in.');
    }
  }
  
  updateChartData(): void {
    const completedCount = this.tasks.filter(task => task.status === 'completed').length;
    const ongoingCount = this.tasks.length - completedCount;
    
    // Now TypeScript knows what type to expect for chartData
    this.chartData = [
      ['Completed', completedCount],
      ['Ongoing', ongoingCount]
    ];
  }

  filterTasks(priority: string | null, status: string | null, category: string | null): void {
    if (priority !== null) {
      this.currentPriorityFilter = priority;
    }
    if (status !== null) {
      this.currentStatusFilter = status;
    }
    if (category !== null) {
      this.currentCategoryFilter = category;
    }
    this.filteredTasks = this.tasks.filter(task => {
      return (this.currentPriorityFilter ? task.priority === this.currentPriorityFilter : true)
        && (this.currentStatusFilter ? task.status === this.currentStatusFilter : true)
        && (this.currentCategoryFilter ? task.categoryId === Number(this.currentCategoryFilter) : true);
    });
  }
  

  onSubmit(): void {
    if (this.taskForm.valid) {
      const task: TaskItem = this.taskForm.value;
      const user = JSON.parse(localStorage.getItem('user') || '{}');

      if (user && user.id) {
        task.userId = user.id;
      }
      

      if (task.id) {
        this.taskService.updateTask(task).subscribe(() => this.loadTasks(this.currentPage, this.pageSize));
        this.isFormVisible = false;
      } else {
        task.id = 0;
        this.taskService.createTask(task).subscribe(() => this.loadTasks(this.currentPage, this.pageSize));
        this.isFormVisible = false;
      }
      this.taskForm.reset();
    }
  }

  onEdit(task: TaskItem): void {
    this.isFormVisible = true;
    this.taskForm.patchValue({
      id: task.id,
      name: task.name,
      description: task.description,
      priority: task.priority,
      status: task.status,
      categoryId: task.categoryId || '',
      userId: task.userId || ''
    });
  }
  applyCurrentFilters(): void {
    this.filterTasks(this.currentPriorityFilter, this.currentStatusFilter, this.currentCategoryFilter);
  }
  
  onPageChange(event: any): void {
    this.currentPage = event.pageIndex + 1; // pageIndex is 0-based
    this.pageSize = event.pageSize;
    this.loadTasks(this.currentPage, this.pageSize).then(() => {
      this.filterTasks(this.currentPriorityFilter, this.currentStatusFilter, this.currentCategoryFilter);

    });
  }

  onDelete(id: number): void {
    this.taskService.deleteTask(id).subscribe(() => this.loadTasks(this.currentPage, this.pageSize));
  }
  signOut(): void {
    this.authService.signOut();
    this.router.navigate(['/sign-in']);
  }
}
