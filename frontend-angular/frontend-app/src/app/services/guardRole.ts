import { inject } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivateFn, Router } from '@angular/router';
import { of } from 'rxjs';
import { catchError, map, switchMap } from 'rxjs/operators';
import { AuthService } from './authService';
import { Roles } from '../models/modelRoles';

export const RoleGuard: CanActivateFn = (route: ActivatedRouteSnapshot) => {
  const auth = inject(AuthService);
  const router = inject(Router);

  //Get the roles allowed to access specific routes
  const allowed = route.data['roles'] as Roles[] | undefined;

  return auth.verifyRole().pipe(
    switchMap((isAuth) => {
      //Confirm the local storage role with roles get in backend
      if (auth.getRole() != isAuth) {
        auth.clearLocalStorage()
        return of(router.createUrlTree(['/login']));
      }
      //Verify if the allow is empty, or the current role exist in enum roles
      if (!allowed || auth.hasRole(allowed)) {
        return of(true);
      }
      //If the roles not exist, define the correct path do direct
      const role = auth.getRole();
      const target =
        role === Roles.Admin        ? ['/admin'] :
        role === Roles.Teacher      ? ['/teacher'] :
        role === Roles.User         ? ['/dashboard'] :
        role === Roles.Unauthorized ? ['/unauthorized'] :
                                      ['/login'];

      return of(router.createUrlTree(target));
    }),
    catchError(() => of(router.createUrlTree(['/login'])))
  );
};