import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { Dashboard } from './dashboard/dashboard';
import { Clients } from './clients/clients';
import { Reports } from './reports/reports';
import { Settings } from './settings/settings';
import { ManagePayments } from './manage-payments/manage-payments';
import { RegisterSale } from './register-sale/register-sale';
import { AdminLayout } from './admin-layout/admin-layout';

const routes: Routes = [

  {path: 'dashboard', component: Dashboard },
  {path: 'clients', component: Clients },
  {path: 'reports', component: Reports },
  {path: 'settings', component: Settings },
  {path: 'manage-payments', component: ManagePayments },
  {path: 'register-sale', component: RegisterSale },
  {path: '', redirectTo: 'dashboard', pathMatch: 'full' },
  {path: 'layout', component: AdminLayout },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class AdminRoutingModule { }
