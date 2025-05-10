import { Component, Inject } from '@angular/core';
import { HeaderComponent } from '../header/header.component';
import { UsersService } from '../../services/users.service';
import { CommonModule, isPlatformBrowser } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { getAuth } from 'firebase/auth';
import { Router } from '@angular/router';
import { PLATFORM_ID } from '@angular/core';
import { User } from '../../models/users.interface';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-users',
  standalone: true,
  imports: [HeaderComponent, CommonModule, FormsModule, RouterModule],
  templateUrl: './users.component.html',
  styleUrl: './users.component.css',
})
export class UsersComponent {
  users: User[] = [];
  searchTerm: string = '';
  filteredUsers: User[] = [];
  message: string = '';
  messageType: 'success' | 'danger' = 'success';
  currentUserId: string = '';

  isLoading: boolean = false;

  constructor(
    private usersService: UsersService,
    private router: Router,
    @Inject(PLATFORM_ID) private platformId: Object
  ) {}

  ngOnInit(): void {
    if (isPlatformBrowser(this.platformId)) {
      const auth = getAuth();
      const user = auth.currentUser;

      if (user) {
        this.currentUserId = user.uid;
      }

      this.usersService.deactivateExpiredUsers().then(() => {
        this.loadUsers();
      });

      this.loadUsers();
    }
  }

  loadUsers() {
    this.isLoading = true;

    this.usersService
      .getUsers()
      .then((users) => {
        this.users = users.sort((a, b) => a.display_name.localeCompare(b.display_name));
        this.filteredUsers = this.users;
        this.isLoading = false;
      })
      .catch((error) => {
        console.error('Erro ao carregar usuários', error);
        this.isLoading = false;
      });
  }

  convertTimestamp(timestamp: any): string {
    if (!timestamp?.seconds) return '-';
    const date = new Date(timestamp.seconds * 1000);
    return date.toLocaleDateString('pt-BR');
  }

  filterUsers() {
    const term = this.searchTerm.trim().toLowerCase();
  
    if (term) {
      this.filteredUsers = this.users.filter((user) => {
        const name = (user.display_name || '').toLowerCase();
        const serial = String(user.seriais || '').toLowerCase();
        return name.includes(term) || serial.includes(term);
      });
    } else {
      this.filteredUsers = this.users;
    }
  }
   

  toggleStatus(user: any) {
    const updatedStatus = !user.status;
    const action = updatedStatus ? 'ativar' : 'desativar';

    if (confirm(`Tem certeza que deseja ${action} o usuário ${user.display_name}?`)) {
      this.usersService.updateUserStatus(user.id, updatedStatus)
        .then(() => {
          user.status = updatedStatus;
          this.message = `Status do usuário ${user.display_name} foi atualizado para ${updatedStatus ? 'Ativo' : 'Inativo'}.`;
          this.messageType = 'success';
          this.clearMessageAfterDelay();
        })
        .catch((error) => {
          console.error('Erro ao atualizar status do usuário:', error);
          this.message = 'Erro ao atualizar o status do usuário.';
          this.messageType = 'danger';
          this.clearMessageAfterDelay();
        });
    }
  }

  toggleLog(user: any) {
    const updatedLog = !user.log;
    const action = updatedLog ? 'ativar' : 'desativar';
  
    if (confirm(`Tem certeza que deseja ${action} o log do usuário ${user.display_name}?`)) {
      this.usersService.updateUserLog(user.id, updatedLog)
        .then(() => {
          user.log = updatedLog;
          this.message = `Log do usuário ${user.display_name} foi ${updatedLog ? 'ativado' : 'desativado'}.`;
          this.messageType = 'success';
          this.clearMessageAfterDelay();
        })
        .catch((error) => {
          console.error('Erro ao atualizar o log do usuário:', error);
          this.message = 'Erro ao atualizar o log do usuário.';
          this.messageType = 'danger';
          this.clearMessageAfterDelay();
        });
    }
  }  

  deleteUser(user: any) {
    if (user.id === this.currentUserId) {
      this.message = 'Você não pode excluir a sua própria conta.';
      this.messageType = 'danger';
      return;
    }

    if (confirm(`Tem certeza que deseja excluir o usuário ${user.display_name}?`)) {
      this.usersService.deleteUser(user.id)
        .then(() => {
          this.message = 'Usuário excluído com sucesso.';
          this.messageType = 'success';
          this.loadUsers();
        })
        .catch((error) => {
          console.error('Erro ao excluir usuário:', error);
          this.message = 'Erro ao excluir o usuário.';
          this.messageType = 'danger';
        });
    }
  }

  createUser() {
    this.router.navigate(['/cadastro-usuario']);
  }

  clearMessageAfterDelay() {
    setTimeout(() => {
      this.message = '';
      this.loadUsers();
    }, 200);
  }
}
