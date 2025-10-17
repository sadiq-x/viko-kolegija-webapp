import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment.development';
import { Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { catchError, map, Observable, of } from 'rxjs';
import { ModelUserLoginResponse, ModelUserRegisterRequest } from '../models/modelUser';
import { Roles } from '../models/modelRoles';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private apiUrlLogin = environment.apiUrl + 'auth/login';
  private apiUrlRegister = environment.apiUrl + 'auth/register';
  private apiUrlUpdatePassword = environment.apiUrl + 'auth/update/password';

  constructor(private http: HttpClient, private router: Router) { }

  login(obj: { Username: string, PasswordHash: string }): Observable<boolean> {
    return this.http.post<any>(this.apiUrlLogin, obj).pipe(
      map((response) => {
        if (response?.token && response?.user) {
          const role = response.user.roleType
          let modelUser: ModelUserLoginResponse = response.user;
          localStorage.setItem('authUser', JSON.stringify(modelUser))
          localStorage.setItem('role', response.user.roleType)
          localStorage.setItem('authToken', response.token);
          alert("Login Successful");

          //Directs the client for the page according to the Role type
          switch (role) {
            case Roles.Admin: this.router.navigate(['/admin']); break;
            case Roles.Teacher: this.router.navigate(['/teacher']); break;
            case Roles.Unauthorized: this.router.navigate(['/dashboard']); break;
            default: this.router.navigate(['/dashboard']);
          }
          return true;
        };
        return false;
      }),
      catchError((error) => {
        if (error.status === 404 || error.status === 400) {
          alert('Wrong Credentials.');
        }
        return of(false);
      })
    );
  };

  register(obj: ModelUserRegisterRequest): Observable<boolean> {
    return this.http.post<any>(this.apiUrlRegister, obj).pipe(
      map((response) => {
        console.log(response)
        return response?.success === true;
      }),
      catchError((error) => {
        return of(false);
      })
    );
  }

  updatePassword(obj: { EntityId: string, Username: string, PasswordHash: string }): Observable<boolean> {
    return this.http.post<any>(this.apiUrlUpdatePassword, obj).pipe(
      map((response) => {
        return response?.success === true;
      }),
      catchError((error) => {
        return of(false);
      })
    )
  };

  //Logout the user, will remove the Token and Role
  logout() {
    localStorage.clear(); //Clear all items from local storage
    this.router.navigate(['/login']); //Navigate to path Login
  }

  //For the Intercept Service 
  //Get the Token from local storage
  getAuthToken() {
    return localStorage.getItem('authToken') || null; //Get the Token if have, else return null
  };

  //For the Login/Role Guard Service 
  //Get the Role from local storage 
  getRole() {
    const role = localStorage.getItem('role');
    if (role === Roles.Admin || role === Roles.Teacher || role === Roles.Unauthorized || role === Roles.User) {
      return role as Roles;
    }
    return null;
  }
  //Verify if the roles exist and if are in the Roles Enum
  hasRole(allowed: Roles[]): boolean {
    const current = this.getRole();
    return current !== null && allowed.includes(current);
  }
  //Confirm if the token is in local storage
  isAuthenticated() {
    const token = this.getAuthToken()
    return !!token;
    /**
    Or

    if (!token)
    return false
     */
  }
}
