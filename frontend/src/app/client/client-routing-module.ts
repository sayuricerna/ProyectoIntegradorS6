import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { Dashboard } from './dashboard/dashboard';
import { Wallet } from './wallet/wallet';

const routes: Routes = [
  { path: 'dashboard', loadChildren: () => import('./dashboard/dashboard').then(m => m.Dashboard) },
  {path: '', redirectTo: 'dashboard', pathMatch: 'full' },
  {path: 'wallet', component: Wallet },

];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ClientRoutingModule { }
