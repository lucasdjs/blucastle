import { Injectable } from '@angular/core';
import { getAuth, signInWithEmailAndPassword, signOut } from 'firebase/auth';
import { getFirestore, doc, getDoc } from 'firebase/firestore';
import { Router } from '@angular/router';
import { environment } from '../environments/environments';
import { initializeApp, FirebaseApp } from 'firebase/app';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private firebaseApp: FirebaseApp;
  public message: string = '';

  constructor(private router: Router) {
    this.firebaseApp = initializeApp(environment.firebaseConfig);
  }

  login(email: string, password: string) {
    const auth = getAuth(this.firebaseApp);

    return signInWithEmailAndPassword(auth, email, password)
      .then((userCredential) => {
        const user = userCredential.user;
        if (user) {

          this.checkAdminStatus(user.uid);
        }
      })
      .catch((error) => {
        console.error('[Login] Falha no login:', error);
        throw error;
      });
  }

  checkAdminStatus(uid: string): Promise<void> {
    const db = getFirestore(this.firebaseApp);
    const userRef = doc(db, 'users', uid);
  
    return getDoc(userRef).then((docSnap) => {
      if (docSnap.exists()) {
        const userData = docSnap.data();
  
        if (userData && userData['adminUser'] === true) {
          this.router.navigate(['/users']);
          return;
        } else {
          this.message = 'Acesso negado. Esta conta não possui privilégios de administrador.';
          return this.logout().then(() => {
            throw new Error(this.message);
          });
        }
      } else {
        this.message = 'Usuário não encontrado no Firestore.';
        throw new Error(this.message);
      }
    }).catch((error) => {
      console.error('[Admin Check] Erro ao verificar admin:', error);
      this.message = this.message || 'Erro ao verificar permissões.';
      throw error;
    });
  }
  

  logout() {
    const auth = getAuth(this.firebaseApp);
    return signOut(auth).then(() => {
      this.router.navigate(['/login']);
    }).catch((error) => {
      console.error('Logout failed', error);
    });
  }
}
