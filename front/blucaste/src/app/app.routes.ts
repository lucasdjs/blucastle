import { Routes } from '@angular/router';
import { LoginComponent } from './components/login/login.component';
import { UsersComponent } from './components/users/users.component';
import { CadastrarComponent } from './components/cadastrar/cadastrar.component';
import { InactiveComponent } from './components/inactive/inactive.component';
import { EditarComponent } from './components/editar/editar.component';

export const routes: Routes = [
    { path: '', component: LoginComponent },
    { path: 'login', component: LoginComponent },
    { path: 'users', component: UsersComponent },
    { path: 'cadastro-usuario', component: CadastrarComponent },
    { path: 'inactive-users', component: InactiveComponent },
    { path: 'editar-usuario/:id', component: EditarComponent },
];
