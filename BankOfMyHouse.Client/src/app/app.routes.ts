import { Routes } from '@angular/router';
import { LoginComponent } from './auth/login/login.component';
import { RegisterComponent } from './auth/register/register.component';
import { GuestGuard } from './auth/guards/guest.guard';
import { AuthGuard } from './auth/guards/auth.guard';
import { FinanceDashboardComponent } from './dashboard/finance_dashboard/finance-dashboard.component';
import { InvestmentsDashboardComponent } from './dashboard/investments_dashboard/investments-dashboard.component';

export const routes: Routes = [
  { path: '', redirectTo: '/home', pathMatch: 'full' },
  { path: 'home', component: FinanceDashboardComponent, canActivate: [AuthGuard] },
  { path: 'investments', component: InvestmentsDashboardComponent, canActivate: [AuthGuard] },
  { path: 'login', component: LoginComponent, canActivate: [GuestGuard] },
  { path: 'register', component: RegisterComponent, canActivate: [GuestGuard] },
  { path: '**', redirectTo: '/login' },
  // Add more routes as needed
];
