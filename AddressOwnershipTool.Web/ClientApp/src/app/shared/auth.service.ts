import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { BehaviorSubject, lastValueFrom, Observable } from 'rxjs';
import Web3 from 'web3';

declare let window: any;

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private web3?: Web3;
  private isLoggedInSubject = new BehaviorSubject<boolean>(false);
  isLoggedIn$: Observable<boolean> = this.isLoggedInSubject.asObservable();

  constructor(private httpClient: HttpClient, @Inject('BASE_URL') private baseUrl: string) {
    if (typeof window.ethereum !== 'undefined') {
      this.web3 = new Web3(window.ethereum);
    }
  }

  async connectToMetaMask(): Promise<void> {
    if (!!sessionStorage.getItem('token')) {
      this.isLoggedInSubject.next(true);
      return;
    }

    try {
      const accounts = await window.ethereum.request({ method: 'eth_requestAccounts' });
      const account = accounts[0];

      sessionStorage.setItem('account', account);
      console.log('Connected account:', account);

      const nonceResponse = await lastValueFrom(this.httpClient.get<{ nonce: string }>(`${this.baseUrl}api/auth/nonce?account=${account}`));
      const signedNonce = await this.web3?.eth.personal.sign(nonceResponse.nonce, account, '');
      const verifyResponse = await lastValueFrom(this.httpClient.post<{ token: string }>(`${this.baseUrl}api/auth/verify`, { address: account, nonce: nonceResponse.nonce, signedMessage: signedNonce  }));
      const token = verifyResponse.token;
      this.isLoggedInSubject.next(true);
      console.log(token);
      sessionStorage.setItem('token', token);
    } catch (error) {
      this.isLoggedInSubject.next(false);
      console.error('Error connecting to MetaMask:', error);
    }
  }
}
