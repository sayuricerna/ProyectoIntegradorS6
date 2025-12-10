import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatSidenavModule } from '@angular/material/sidenav'; // <-- Nuevo
import { MatListModule } from '@angular/material/list';     // <-- Nuevo
import { MatIconModule } from '@angular/material/icon';       // <-- Nuevo
import { AdminRoutingModule } from './admin-routing-module';
import { AdminLayout } from './admin-layout/admin-layout';
import { Dashboard } from './dashboard/dashboard';
@NgModule({
  declarations: [
    

  ],
  imports: [
    CommonModule,
    AdminRoutingModule,
    MatSidenavModule, // <-- Nuevo
    MatListModule,    // <-- Nuevo
    MatIconModule     // <-- Nuevo  
  ]
})
export class AdminModule { }
