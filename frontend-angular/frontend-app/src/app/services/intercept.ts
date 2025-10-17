import { inject, Injectable } from '@angular/core';
import { AuthService } from './authService';
import { Router } from '@angular/router';
import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { catchError, throwError } from 'rxjs';

export const AuthInterceptor: HttpInterceptorFn = (req, next) => {

  const authService = inject(AuthService);
  const router = inject(Router);

  // Retrieve token (could come from auth service, local storage, etc.)
  const token = authService.getAuthToken();

  // Clone the request if we have a token
  const authReq = token
    ? req.clone({ setHeaders: { Authorization: `Bearer ${token}` } })
    : req;

  // Forward the request, then handle any 401/403 errors
  return next(authReq).pipe(
    catchError((error: HttpErrorResponse) => {
      if (error.status === 401) {
        // Unauthorized
        // e.g. force logout and redirect to login
        authService.logout();
        router.navigate(['/login']);
      } else if (error.status === 403) {
        // Forbidden
        // e.g. navigate to a "no access" page or just log
        console.error('Access denied – 403');
        router.navigate(['/login']);
        // router.navigate(['/forbidden']);
      }
      // Propagate the error so the component or other interceptors can handle it
      return throwError(() => error);
    })
  );
};
