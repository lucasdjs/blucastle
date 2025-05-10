import { Component } from '@angular/core';
import { AuthService } from '../../services/auth.service';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  standalone: true,
  imports: [FormsModule, CommonModule],
  styleUrls: ['./login.component.css'],
})
export class LoginComponent {
  email: string = '';
  password: string = '';
  loading: boolean = false;
  message: string = '';
  messageType: string = '';

  constructor(private authService: AuthService, private router: Router) {}

  onLogin() {
    this.loading = true;
    this.message = '';

    this.authService.login(this.email, this.password)
      .then(() => {
        this.loading = false;
        this.message = 'Login realizado com sucesso!';
        this.messageType = 'success';
        this.router.navigate(['/users']);
      })
      .catch((error) => {
        let mensagemErro = '';
    
        switch (error.code) {
          case 'auth/invalid-email':
            mensagemErro = 'O e-mail informado é inválido.';
            this.messageType = 'danger';
            this.loading = false;
            break;
          case 'auth/user-disabled':
            this.loading = false;
            this.messageType = 'danger';
            mensagemErro = 'Este usuário foi desativado.';
            break;
          case 'auth/user-not-found':
            this.loading = false;
            this.messageType = 'danger';
            mensagemErro = 'Usuário não encontrado.';
            break;
          case 'auth/wrong-password':
          case 'auth/invalid-credential':
          this.loading = false;
          this.messageType = 'danger';
            mensagemErro = 'E-mail ou senha incorretos.';
            break;
          default:
            this.loading = false;
            this.messageType = 'danger';
            mensagemErro = 'Ocorreu um erro ao tentar fazer login.';
        }
    
        this.message = mensagemErro;
      });
  }
}
