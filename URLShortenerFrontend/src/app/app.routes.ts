import { Routes } from '@angular/router';
import { RegistrationComponent } from './user/registration/registration.component';
import { LoginComponent } from './user/login/login.component';
import { TableComponent } from './table/table.component';
import { InfoComponent } from './info/info.component';
import { AboutComponent } from './about/about.component';
import { UrlinfoComponent } from './urlinfo/urlinfo.component';

export const routes: Routes = [
    { path: 'register', component: RegistrationComponent },
    { path: 'login', component: LoginComponent },
    { path: 'table', component: TableComponent},
    { path: 'info', component: InfoComponent},
    { path: 'about', component: AboutComponent },
    { path: 'url-info/:code', component: UrlinfoComponent},
    { path: '', redirectTo: '/login', pathMatch: 'full' },
    { path: '**', redirectTo: '/login' }]
