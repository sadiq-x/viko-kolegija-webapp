import { Routes } from '@angular/router';
import { Home } from './pages/home/home';
import { Profile } from './pages/private/profile/profile';
import { Login } from './pages/login/login';
import { Register } from './pages/register/register';
import { Courses } from './pages/private/courses/courses';
import { Layout } from './pages/private/layout/layout';
import { Dashboard } from './pages/private/dashboard/dashboard';
import { AuthGuard } from './services/guardLogin';
import { Teacher } from './pages/private/teacher/teacher';
import { Admin } from './pages/private/admin/admin';
import { RoleGuard } from './services/guardRole';
import { Roles } from './models/modelRoles';
import { Unauthorized } from './pages/private/unauthorized/unauthorized';
import { Topics } from './pages/private/topics/topics';
import { CoursesList } from './pages/private/courseslist/courseslist';
import { TeacherEvents } from './pages/private/teacher-events/teacher-events';
import { CoursesType } from './pages/private/coursestype/coursestype';

export const routes: Routes = [
  { path: '', redirectTo: 'home', pathMatch: 'full' },
  { path: 'home', component: Home },
  { path: 'login', component: Login },
  { path: 'register', component: Register },
  {
    path: '',
    component: Layout,
    canActivate: [AuthGuard],
    children: [
      {
        path: 'dashboard',
        component: Dashboard,
        canActivate: [RoleGuard],
        data: { roles: [Roles.User] }, //This define who can access
      },
      {
        path: 'profile',
        component: Profile,
        canActivate: [RoleGuard],
        data: { roles: [Roles.Admin, Roles.Teacher, Roles.User, Roles.Unauthorized] }, //This define who can access
      },
      {
        path: 'courses',
        component: CoursesList,
        canActivate: [RoleGuard],
        data: { roles: [Roles.Admin, Roles.Teacher, Roles.User] }, //This define who can access
      },
      {
        path: 'courses/type/:type',
        component: CoursesType,
        canActivate: [RoleGuard],
        data: { roles: [Roles.Admin, Roles.Teacher, Roles.User] }, //This define who can access
      },
      {
        path: 'topics',
        component: Topics,
        canActivate: [RoleGuard],
        data: { roles: [Roles.Admin, Roles.Teacher, Roles.User] }, //This define who can access
      },
      //Dedicated areas
      {
        path: 'teacher',
        component: Teacher,
        canActivate: [RoleGuard],
        data: { roles: [Roles.Teacher] },
        children: [], //This define who can access
      },
      {
        path: 'teacher/event/:id',
        component: TeacherEvents,
        canActivate: [RoleGuard],
        data: { roles: [Roles.Teacher] },
      },
      {
        path: 'admin',
        component: Admin,
        canActivate: [RoleGuard],
        data: { roles: [Roles.Admin] }, //This define who can access
      },
      {
        path: 'unauthorized',
        component: Unauthorized,
        canActivate: [RoleGuard],
        data: { roles: [Roles.Unauthorized] },
      },
    ],
  },
  { path: '**', redirectTo: 'home', pathMatch: 'full' },
];
