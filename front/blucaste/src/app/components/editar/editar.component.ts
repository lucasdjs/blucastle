import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { UsersService } from '../../services/users.service';
import { Timestamp } from 'firebase/firestore';
import { User } from '../../models/users.interface';
import { HeaderComponent } from '../header/header.component';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-editar',
  standalone: true,
  imports: [HeaderComponent, CommonModule, FormsModule],
  templateUrl: './editar.component.html'
})
export class EditarComponent implements OnInit {
  usuario: User = {
    id: '',
    display_name: '',
    status: false,
    log: false,
    validade: '',
    quantidadeUso: 0,
    tipoUso: 0,
    seriais: []
  };

  public seriaisTexto: string = '';

  mensagem: string = '';
  tipoMensagem: 'success' | 'danger' = 'success';

  constructor(
    private route: ActivatedRoute,
    private usersService: UsersService,
    private router: Router
  ) {}

  ngOnInit() {
    const userId = this.route.snapshot.paramMap.get('id');
    if (userId) {
      this.usersService.getUserById(userId).then((dados: User) => {
        this.usuario = {
          ...dados,
          id: userId,
          validade: dados.validade?.toDate().toISOString().split('T')[0] ?? '',
          seriais: Array.isArray(dados.seriais) ? dados.seriais : []
        };

        const seriais = this.usuario.seriais;
        if (seriais) {
          this.seriaisTexto = seriais.join('\n');
        }

      });
    }
  }

  salvarEdicao() {
    if (!this.usuario.display_name) {
      this.mensagem = 'Nome é obrigatório.';
      this.tipoMensagem = 'danger';
      return;
    }

    const usuarioAtualizado: any = {
      display_name: this.usuario.display_name,
      status: this.usuario.status,
      log: this.usuario.log,
      tipoUso: this.usuario.tipoUso
    };

    if (this.usuario.tipoUso === 0) {
      if (this.usuario.quantidadeUso <= 0) {
        this.mensagem = 'Quantidade de uso deve ser maior que 0.';
        this.tipoMensagem = 'danger';
        return;
      }
      usuarioAtualizado.quantidadeUso = this.usuario.quantidadeUso;
    } else {
      const validadeDate = new Date(this.usuario.validade || '2100-01-01');
      if (isNaN(validadeDate.getTime())) {
        this.mensagem = 'Data de validade inválida.';
        this.tipoMensagem = 'danger';
        return;
      }
      usuarioAtualizado.validade = Timestamp.fromDate(validadeDate);
    }

    usuarioAtualizado.seriais = this.seriaisTexto
      .split(/[\n,]/)
      .map(s => s.trim())
      .filter(s => s.length > 0);

    this.usersService.updateUser(this.usuario.id, usuarioAtualizado)
      .then(() => {
        this.mensagem = 'Usuário atualizado com sucesso!';
        this.tipoMensagem = 'success';
        setTimeout(() => this.router.navigate(['/users']), 500);
      })
      .catch(error => {
        this.mensagem = 'Erro ao atualizar usuário: ' + error.message;
        this.tipoMensagem = 'danger';
      });
  }
}
