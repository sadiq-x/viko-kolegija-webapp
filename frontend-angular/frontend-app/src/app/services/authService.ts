import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment.development';
import { Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { catchError, map, Observable, of } from 'rxjs';
import { ModelUserLoginResponse, ModelUserRegisterRequest } from '../models/modelUser';
import { Roles } from '../models/modelRoles';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private apiUrlLogin = environment.apiUrl + 'auth/login';
  private apiUrlRegister = environment.apiUrl + 'auth/register';
  private apiUrlUpdatePassword = environment.apiUrl + 'auth/update/password';
  private apiUrlAuthorizationRole = environment.apiUrl + 'auth/roles';

  constructor(private http: HttpClient, private router: Router) {}

  //Request the backend to do user login
  login(
    objBody: { Username: string; PasswordHash: string },
    objInput: boolean
  ): Observable<boolean> {
    return this.http.post<any>(this.apiUrlLogin, objBody).pipe(
      map((response) => {
        if (response?.token && response?.user) {
          const role = response.user.roleType;
          let modelUser: ModelUserLoginResponse = response.user;

          //If the user turn on remember button, the credentials will be saved in localStorage, if not, the credentials will be saved in sessionStorage
          if (objInput) {
            localStorage.setItem('authUser', JSON.stringify(modelUser));
            localStorage.setItem('role', response.user.roleType);
            localStorage.setItem('authToken', response.token);
          } else {
            sessionStorage.setItem('authUser', JSON.stringify(modelUser));
            sessionStorage.setItem('role', response.user.roleType);
            sessionStorage.setItem('authToken', response.token);
          }
          alert('Login Successful' + role);

          //Directs the client for the page according to the Role type
          switch (role) {
            case Roles.Admin:
              this.router.navigate(['/admin']);
              break;
            case Roles.Teacher:
              this.router.navigate(['/teacher']);
              break;
            case Roles.Unauthorized:
              this.router.navigate(['/unauthorized']);
              break;
            case Roles.User:
              this.router.navigate(['/dashboard']);
              break;
          }
          return true as const;
        }
        return false as const;
      }),
      catchError((error) => {
        if (error.status === 404 || error.status === 400) {
          alert('Wrong Credentials.');
        }
        return of(false);
      })
    );
  }
  //Request the backend to do user register
  register(obj: ModelUserRegisterRequest): Observable<boolean> {
    return this.http.post<any>(this.apiUrlRegister, obj).pipe(
      map((response) => {
        return response?.success === true;
      }),
      catchError((error) => {
        return of(false);
      })
    );
  }
  //Request the backend to update password
  updatePassword(obj: {
    EntityId: string;
    Username: string;
    PasswordHash: string;
  }): Observable<boolean> {
    return this.http.post<any>(this.apiUrlUpdatePassword, obj).pipe(
      map((response) => {
        return response?.success === true;
      }),
      catchError((error) => {
        return of(false);
      })
    );
  }
  //Logout the user, will remove the Token and Role
  logout() {
    localStorage.clear(); //Clear all items from local storage/session storage
    sessionStorage.clear();
    this.router.navigate(['/login']); //Navigate to path Login
  }
  //Clear everything in local storage/session storage
  clearLocalStorageAndSessionStorage() {
    localStorage.clear();
    sessionStorage.clear();
  }
  //For the Intercept Service
  //Get the Token from local storage/session storage
  getAuthToken() {
    return localStorage.getItem('authToken') || sessionStorage.getItem('authToken') || null; //Get the Token if have, else return null
  }
  //Get the authUser from local storage/session storage
  getAuthUser() {
    return localStorage.getItem('authUser') || sessionStorage.getItem('authUser') || null
  }
  //Request backend to get verify of entityId, username and role type, will return true if the user are authorized to access the page who wants
  verifyRole(): Observable<string | false> {
    //Return the role or false
    return this.http.get<any>(this.apiUrlAuthorizationRole).pipe(
      map((response) => {
        if (response.success) {
          return response.roleVerify.type;
        }
        return !!response.success;
      })
    );
  }
  //Get the Role from local storage/session storage
  getRole() {
    const role = localStorage.getItem('role') || sessionStorage.getItem('role');
    if (
      role === Roles.Admin ||
      role === Roles.Teacher ||
      role === Roles.Unauthorized ||
      role === Roles.User
    ) {
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
    const token = this.getAuthToken();
    return !!token;
    /**
    Or

    if (!token)
    return false
     */
  }
}
