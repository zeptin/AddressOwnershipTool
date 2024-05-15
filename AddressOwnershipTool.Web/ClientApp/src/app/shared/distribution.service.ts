import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { ClaimGroupResponse, SwapRequest } from '../models/claim-group';
import { lastValueFrom } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class DistributionService {
  constructor(private http: HttpClient) {}

  load(path: string, limit: number) {
    //let body = JSON.stringify();
    //console.log(body);
    //const httpOptions = {
    //  headers: new HttpHeaders({'Content-Type': 'application/json'})
    //}
    return lastValueFrom(this.http.post<ClaimGroupResponse>('/api/distribution/load', { path: path, limit: limit }));
  }

  update(data: SwapRequest) {
    return lastValueFrom(this.http.post<ClaimGroupResponse>('/api/distribution/update', data));
  }
}
