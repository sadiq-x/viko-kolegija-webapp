import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment.development';
import { Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { catchError, map, of } from 'rxjs';
import { ModelUserLogin } from '../models/modelUser';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  private apiUrlLogin = environment.apiUrl + 'auth/login';
  private apiUrlRegister = environment.apiUrl + 'auth/register';

  constructor(private http: HttpClient, private router: Router) { }

  login(obj: { Username: string, PasswordHash: string }) {
    return this.http.post<any>(this.apiUrlLogin, obj).pipe(
      map((response) => {
        if (response?.token && response?.user) {
          let modelUser: ModelUserLogin = response.user;
          console.log(response.user);
          console.log(modelUser);
          localStorage.setItem('authUser', JSON.stringify(modelUser))
          localStorage.setItem('authToken', response.token);
          alert("Login Successful");
          return true;
        }
        return false;
      }),
      catchError((error) => {
        if (error.status === 404 || error.status === 400) {
          console.log(error)
          alert('Wrong Credentials.');
        }
        return of(false);
      })
    );
  }

  logout() {
    localStorage.clear(); //Clear all items from local storage
    this.router.navigate(['/login']); //Navigate to path Login
  }

  getAuthToken() {
    return localStorage.getItem('authToken') || null; //Get the Token if have, else return null
  }

  isAuthenticated() { //Confirm if the token is in local storage
    const token = this.getAuthToken()
    return !!token;
    /**
    Or

    if (!token)
    return false
     */
  }
}
