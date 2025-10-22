import { inject } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivateFn, Router } from '@angular/router';
import { AuthService } from './authService';
import { Roles } from '../models/modelRoles';

export const RoleGuard: CanActivateFn = (route: ActivatedRouteSnapshot) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  const allowed = route.data['roles'] as Roles[] | undefined; //Get the property data inside the Route

  //If the route does not declare 'roles', it does not restrict; otherwise, it validates
  if (!allowed || authService.hasRole(allowed)) return true;

  //Switcher of direction of Role page
  switch (authService.getRole()) {
    case Roles.Admin:
      router.navigate(['/admin']);
      break;
    case Roles.Teacher:
      router.navigate(['/teacher']);
      break;
    case Roles.Unauthorized:
      router.navigate(['/unauthorized']);
      break;
    case Roles.User:
      router.navigate(['/dashboard']);
      break;
  }

  //router.navigate(['/unauthorized']);
  return false;
};
