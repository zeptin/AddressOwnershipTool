import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { lastValueFrom } from 'rxjs';
import Web3 from 'web3';

declare let window: any;

@Injectable({
  providedIn: 'root'
})
export class EthService {
  private web3?: Web3;

  constructor(private httpClient: HttpClient, @Inject('BASE_URL') private baseUrl: string) {
    if (typeof window.ethereum !== 'undefined') {
      this.web3 = new Web3(window.ethereum);
    } else {
        console.error('MetaMask is not installed!');
    }
  }

  async sendTransaction(fromAddress: string, toAddress: string, amountEther: number): Promise<void> {
    const transactionParameters = {
      to: toAddress,
      from: fromAddress,
      value: Number(amountEther * 1e18).toString(16)
    };

    try {
      const txHash = await window.ethereum.request({
          method: 'eth_sendTransaction',
          params: [transactionParameters],
      });
      console.log('Transaction Hash:', txHash);
    } catch (error) {
      console.error('Transaction failed:', error);
      throw new Error('Transaction failed');
    }
  }
}
