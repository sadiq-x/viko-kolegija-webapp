import { Routes } from '@angular/router';
import { Auth } from './services/auth';
import { Home } from './pages/home/home';
import { Profile } from './pages/private/profile/profile';
import { Login } from './pages/login/login';
import { Register } from './pages/register/register';
import { Courses } from './pages/private/courses/courses';
import { Layout } from './pages/private/layout/layout';
import { Dashboard } from './pages/private/dashboard/dashboard';

export const routes: Routes = [
    { path: '', redirectTo: 'home', pathMatch: 'full' },
    {
        path: '', component: Layout, canActivate: [Auth],
        children: [
            {
                path: 'dashboard',
                component: Dashboard
            },
            {
                path: 'profile',
                component: Profile
            },
            {
                path: 'courses',
                component: Courses
            }
        ]
    },
    { path: 'login', component: Login },
    { path: 'register', component: Register },
    { path: 'home', component: Home },
    { path: '**', redirectTo: 'home', pathMatch: 'full' }
];
