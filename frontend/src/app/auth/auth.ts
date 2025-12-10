import { Injectable } from '@angular/core';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root',
})
export class Auth {
  constructor(private router: Router) { }

  // SIMULACIÓN DE LOGIN 
  login(email: string): void {
    if (email === 'admin@admin.com') {
      // Redirige al dashboard de Admin
      this.router.navigate(['/admin/dashboard']); 
    } else if (email === 'client@client.com') {
      // Redirige al dashboard de Cliente
      this.router.navigate(['/client/dashboard']); 
    } else {
      alert('Credenciales inválidas. Use admin@admin.com (Admin) o client@client.com (Cliente).');
    }
  }

  logout(): void {
    this.router.navigate(['/auth/login']);
  }
}

