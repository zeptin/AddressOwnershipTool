import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ClaimGroupResponse, SwapRequest } from '../models/claim-group';
import { lastValueFrom } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class DistributionService {
  constructor(private http: HttpClient) {}

  load(path: string, limit: number) {
    return lastValueFrom(this.http.post<ClaimGroupResponse>('/api/distribution/load', { path: path, limit: limit }));
  }

  update(data: SwapRequest) {
    return lastValueFrom(this.http.post<ClaimGroupResponse>('/api/distribution/update', data));
  }
}
