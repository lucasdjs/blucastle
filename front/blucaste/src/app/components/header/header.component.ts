import { Component } from '@angular/core';
import { AuthService } from '../../services/auth.service';
import { Router, RouterModule  } from '@angular/router';

@Component({
  selector: 'app-header',
  imports: [RouterModule],
  templateUrl: './header.component.html',
  styleUrl: './header.component.css'
})
export class HeaderComponent {
  constructor(private authService: AuthService, private router: Router) {}
  onLogout() {
    this.authService.logout().then(() => {
      this.router.navigate(['/login']);
    }).catch((error) => {
      console.error("Erro ao fazer logout", error);
    });
  }
}
