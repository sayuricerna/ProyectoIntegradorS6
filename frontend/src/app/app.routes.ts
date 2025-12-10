import { Routes } from '@angular/router';

export const routes: Routes = [
// 1. Redirige la ruta base (/) al módulo de autenticación
  { path: '', redirectTo: 'auth', pathMatch: 'full' }, 
  
  // 2. Carga el módulo de autenticación (que tiene el Login)
    {path: 'auth', loadChildren: () => import('./auth/auth-module').then(m => m.AuthModule) },
    // 3. Carga el módulo de administración para rutas que comienzan con /admin
    {path: 'admin', loadChildren: () => import('./admin/admin-module').then(m => m.AdminModule) },
];
