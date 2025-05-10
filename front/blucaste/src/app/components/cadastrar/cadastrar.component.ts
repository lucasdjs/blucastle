import { Component } from '@angular/core';
import { HeaderComponent } from '../header/header.component';
import { CommonModule } from '@angular/common';
import { UsersService } from '../../services/users.service';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { Timestamp } from 'firebase/firestore';
import { User } from '../../models/users.interface';

@Component({
  selector: 'app-cadastrar',
  imports: [HeaderComponent, CommonModule, FormsModule],
  templateUrl: './cadastrar.component.html',
})
export class CadastrarComponent {
  novoUsuario: User = {
    id: '',
    display_name: '',
    status: false,
    log: false,
    validade: '',
    quantidadeUso: 0,
    tipoUso: 0, // 0 = uso por quantidade, 1 = uso por validade
  };

  mensagem: string = '';
  tipoMensagem: 'success' | 'danger' = 'success';

  constructor(private usersService: UsersService, private router: Router) {}

  cadastrarUsuario() {
    if (!this.novoUsuario.display_name) {
      this.mensagem = 'Preencha todos os campos obrigatórios.';
      this.tipoMensagem = 'danger';
      return;
    }

    const usuarioParaSalvar: any = {
      display_name: this.novoUsuario.display_name,
      status: this.novoUsuario.status,
      log: this.novoUsuario.log,
      tipoUso: this.novoUsuario.tipoUso,
    };

    if (this.novoUsuario.tipoUso === 0) {
      if (this.novoUsuario.quantidadeUso <= 0) {
        this.mensagem = 'Quantidade de uso deve ser maior que 0.';
        this.tipoMensagem = 'danger';
        return;
      }
      usuarioParaSalvar.quantidadeUso = this.novoUsuario.quantidadeUso;
    } else if (this.novoUsuario.tipoUso === 1) {
      const validadeDate = new Date(this.novoUsuario.validade || '2100-01-01');
      if (isNaN(validadeDate.getTime())) {
        this.mensagem = 'Data de validade inválida.';
        this.tipoMensagem = 'danger';
        return;
      }
      usuarioParaSalvar.validade = Timestamp.fromDate(validadeDate);
    }

    this.usersService
      .createUser(usuarioParaSalvar)
      .then(() => {
        this.mensagem = 'Usuário cadastrado com sucesso!';
        this.tipoMensagem = 'success';

        this.novoUsuario = {
          id: '',
          display_name: '',
          status: false,
          log: false,
          validade: '',
          quantidadeUso: 0,
          tipoUso: 0,
        };

        setTimeout(() => this.router.navigate(['/users']), 200);
      })
      .catch((error) => {
        this.mensagem = 'Erro ao cadastrar usuário: ' + error.message;
        this.tipoMensagem = 'danger';
      });
  }
}
