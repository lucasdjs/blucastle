import { Injectable } from '@angular/core';
import { getFirestore, collection, getDocs, deleteDoc , updateDoc, doc, addDoc, query, where, getDoc } from 'firebase/firestore';
import { environment } from '../environments/environments';
import { initializeApp, FirebaseApp } from 'firebase/app';
import { v4 as uuidv4 } from 'uuid';
import { User } from '../models/users.interface';

@Injectable({
  providedIn: 'root'
})
export class UsersService {
  private firebaseApp: FirebaseApp;
  constructor() { 
    this.firebaseApp = initializeApp(environment.firebaseConfig);
  }

  public getUsers() {
    const db = getFirestore(this.firebaseApp);
    const usersRef = collection(db, 'users');
    const q = query(usersRef, where('status', '==', true));
  
    return getDocs(q).then(querySnapshot => {
      return querySnapshot.docs.map(doc => ({
        id: doc.id,
        ...doc.data()
      } as User));
    }).catch((error) => {
      console.error("Erro ao obter usuários ativos", error);
      return [];
    });
  }

  public getUsersInactive() {
    const db = getFirestore(this.firebaseApp);
    const usersRef = collection(db, 'users');
    const q = query(usersRef, where('status', '==', false));
  
    return getDocs(q).then(querySnapshot => {
      return querySnapshot.docs.map(doc => ({
        id: doc.id,
        ...doc.data()
      }as User));
    }).catch((error) => {
      console.error("Erro ao obter usuários ativos", error);
      return [];
    });
  }

  deleteUser(userId: string) {
    const db = getFirestore(this.firebaseApp);
    const userRef = doc(db, 'users', userId);
  
    return deleteDoc(userRef)
      .then(() => {
        console.log("Usuário excluído com sucesso");
      })
      .catch((error) => {
        console.error("Erro ao excluir usuário", error);
        throw error;
      });
  }

  async updateUser(id: string, data: Partial<User>): Promise<void> {
    const db = getFirestore(this.firebaseApp);
    const userRef = doc(db, 'users', id);
  
    return updateDoc(userRef, data)
      .then(() => {
        console.log('Usuário atualizado com sucesso');
      })
      .catch((error) => {
        console.error('Erro ao atualizar usuário:', error);
        throw error;
      });
  }

  async deactivateExpiredUsers() {
    const db = getFirestore(this.firebaseApp);
    const usersRef = collection(db, 'users');
    const allUsersSnapshot = await getDocs(usersRef);
  
    const now = new Date();
  
    const promises = allUsersSnapshot.docs.map(async (docSnap) => {
      const user = docSnap.data();
      const tipoUso = user['tipoUso'];
      const status = user['status'];
      const userRef = doc(db, 'users', docSnap.id);
  
      if (!status) return null; // já está desativado, não faz nada
  
      if (tipoUso === 1) {
        // Validade por data
        const validadeData = user['validade'];
        const validade = validadeData?.seconds ? new Date(validadeData.seconds * 1000) : null;
  
        if (validade && validade < now) {
          return updateDoc(userRef, { status: false });
        }
      } else if (tipoUso === 0) {
        // Controle por quantidade de uso
        const quantidadeUso = user['quantidadeUso'];
        if (quantidadeUso <= 0) {
          return updateDoc(userRef, { status: false });
        }
      }
  
      return null;
    });
  
    return Promise.all(promises);
  }
  
  createUser(userData: any) {
    const db = getFirestore(this.firebaseApp);
    const usersRef = collection(db, 'users');
  
    const identificador = uuidv4();
  
    const user = {
      ...userData,
      status: userData.status ?? false,
      identificador,
    };
  
    return addDoc(usersRef, user)
      .then(async (docRef) => {
        // Agora adiciona o UID no próprio documento
        await updateDoc(docRef, {
          uid: docRef.id
        });
  
        console.log('Usuário cadastrado com sucesso. UID salvo no campo uid:', docRef.id);
        return docRef.id;
      })
      .catch((error) => {
        console.error('Erro ao cadastrar usuário:', error);
        throw error;
      });
  }

  updateUserStatus(userId: string, status: boolean) {
    const db = getFirestore(this.firebaseApp);
    const userRef = doc(db, 'users', userId);
  
    return updateDoc(userRef, {
      status: status
    });
  }  

  async getUserById(id: string): Promise<User> {
    const db = getFirestore(this.firebaseApp);
    const userDocRef = doc(db, 'users', id);
    const userSnap = await getDoc(userDocRef);
  
    if (!userSnap.exists()) {
      throw new Error('Usuário não encontrado');
    }
  
    const data = userSnap.data() as User;
    return { ...data }; 
}

  updateUserLog(userId: string, log: boolean) {
    const db = getFirestore(this.firebaseApp);
    const userRef = doc(db, 'users', userId);
  
    return updateDoc(userRef, {
      log: log
    });
  }
  
}
