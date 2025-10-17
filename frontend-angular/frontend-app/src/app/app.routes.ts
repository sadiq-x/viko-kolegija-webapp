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

export const routes: Routes = [
    { path: '', redirectTo: 'home', pathMatch: 'full' },
    { path: 'home', component: Home },
    { path: 'login', component: Login },
    { path: 'register', component: Register },
    {
        path: '', component: Layout, canActivate: [AuthGuard],
        children: [
            {
                path: 'dashboard',
                component: Dashboard,
                canActivate: [RoleGuard], 
                data: { roles: [Roles.Admin, Roles.Teacher, Roles.Unauthorized] } //This define who can access
            },
            {
                path: 'profile',
                component: Profile,
                canActivate: [RoleGuard], 
                data: { roles: [Roles.Admin, Roles.Teacher, Roles.Unauthorized] } //This define who can access
            },
            {
                path: 'courses',
                component: Courses,
                canActivate: [RoleGuard], 
                data: { roles: [Roles.Admin, Roles.Teacher] } //This define who can access
            },
            //Dedicated areas
            {
                path: 'teacher',
                component: Teacher,
                canActivate: [RoleGuard], 
                data: { roles: [Roles.Teacher] } //This define who can access
            },
            {
                path: 'admin',
                component: Admin,
                canActivate: [RoleGuard], 
                data: { roles: [Roles.Admin]} //This define who can access
            }
        ]
    },
    { path: '**', redirectTo: 'home', pathMatch: 'full' }
];
