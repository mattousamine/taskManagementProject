import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError, tap } from 'rxjs/operators';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private apiUrl = environment.apiUrl;
  private httpOptions = {
    headers: new HttpHeaders({ 'Content-Type': 'application/json' }),
  };

  constructor(private http: HttpClient) {}

  signIn(email: string, password: string): Observable<any> {
    console.log('email: ', email, 'password: ', password);
    return this.http.post<any>(`${this.apiUrl}/users/signin`, { email, password }, this.httpOptions)
      .pipe(
        tap(response => {
          // Check if the response includes the ID and email
          if (response.id && response.email) {
            localStorage.setItem('user', JSON.stringify({ id: response.id, email: response.email }));
          }
        }),
        catchError(this.handleError)
      );
  }
  
  signUp(email: string, password: string): Observable<any> {
    return this.http.post(`${this.apiUrl}/users`, { email, password }, this.httpOptions)
      .pipe(
        catchError(this.handleError)
      );
  }
  signOut(): void {
    localStorage.removeItem('user');
  }
  

  private handleError(error: any) {
    return throwError(error);
  }
}
