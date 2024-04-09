import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { TaskItem } from '../../models/task-item.model';
import { Category } from '../../models/category.model';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class TaskService {
  private apiUrl = environment.apiUrl;

  constructor(private http: HttpClient) { }

  getCategories(): Observable<Category[]> {
    return this.http.get<Category[]>(`${this.apiUrl}/categories`);
  }
  createCategory(category: Category): Observable<Category> {
    return this.http.post<Category>(`${this.apiUrl}/categories`, category);
  }
  
  getTasksCount(userId: number): Observable<number> {
    let params = new HttpParams().set('userId', userId.toString());
  
    return this.http.get<{ count: number }>(`${this.apiUrl}/tasks/count`, { params }).pipe(
      map(response => response.count)
    );
  }
  

  getTasks(userId: number, page: number, pageSize: number): Observable<TaskItem[]> {
    // Initialize HttpParams
    let params = new HttpParams()
      .set('userId', userId.toString())
      .set('page', page.toString())
      .set('pageSize', pageSize.toString());

    return this.http.get<TaskItem[]>(`${this.apiUrl}/tasks`, { params });
  }


  createTask(task: TaskItem): Observable<TaskItem> {
    return this.http.post<TaskItem>(`${this.apiUrl}/tasks`, task);
  }

  updateTask(task: TaskItem): Observable<TaskItem> {
    return this.http.put<TaskItem>(`${this.apiUrl}/tasks/${task.id}`, task);
  }

  deleteTask(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/tasks/${id}`);
  }
}
