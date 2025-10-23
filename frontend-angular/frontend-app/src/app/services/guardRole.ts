import { inject } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivateFn, Router } from '@angular/router';
import { of } from 'rxjs';
import { catchError, map, switchMap } from 'rxjs/operators';
import { AuthService } from './authService';
import { Roles } from '../models/modelRoles';

export const RoleGuard: CanActivateFn = (route: ActivatedRouteSnapshot) => {
  const auth = inject(AuthService);
  const router = inject(Router);

  const allowed = route.data['roles'] as Roles[] | undefined;

  return auth.verifyRole().pipe(
    switchMap((isAuth) => {
      //TODO Need to check this
      
      // não autenticado / sem permissão base
      if (auth.getRole() != isAuth) {
        console.log("false")
        auth.clearLocalStorage()
        return of(router.createUrlTree(['/login']));
      }

      // rota sem restrição de roles
      if (!allowed || auth.hasRole(allowed)) {
        console.log("true")
        return of(true);
      }
      // role não permitido: calcular redireção por role atual
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