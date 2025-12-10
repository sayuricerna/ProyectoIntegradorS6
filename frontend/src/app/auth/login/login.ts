import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms'; 
import { Auth } from '../auth';
import { MatCardModule } from '@angular/material/card'; 
import { MatButtonModule } from '@angular/material/button';


@Component({
  selector: 'app-login',
  standalone: true, 
  imports: [
    FormsModule, 
    MatCardModule, 
    MatButtonModule
  ],
  templateUrl: './login.html',
  styleUrls: ['./login.css']
})
export class Login {
hidePassword: boolean = true;

togglePassword() {
  this.hidePassword = !this.hidePassword;
}
  email: string = ''; 
  password: string = ''; 

  constructor(private authService: Auth) { }

  onLogin() {
    this.authService.login(this.email); 
  }
}