import { Routes } from '@angular/router';
import { Auth } from './services/auth';
import { Home } from './pages/private/home/home';
import { Profile } from './pages/private/profile/profile';
import { Login } from './pages/login/login';
import { Register } from './pages/register/register';
import { Courses } from './pages/private/courses/courses';
import { Layout } from './pages/private/layout/layout';

export const routes: Routes = [
    { path: '', redirectTo: '', pathMatch: 'full' },
    {
        path: '', component: Layout, canActivate: [Auth],
        children: [
            {
                path: 'home',
                component: Home
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
    { path: '**', redirectTo: '', pathMatch: 'full' }
];
