import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment.development';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class Auth {

  private apiUrlLogin = environment.apiUrl+ 'auth/login';
  private apiUrlRegister = environment.apiUrl + 'auth/register';

  constructor(private router: Router){}

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
