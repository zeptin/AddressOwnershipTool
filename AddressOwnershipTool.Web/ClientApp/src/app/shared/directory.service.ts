import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs/operators';

@Injectable({ providedIn: 'root' })
export class DirectoryValidationService {
  constructor(private http: HttpClient) {}

  checkDirectoryExists(path: string) {
    return this.http.post<{ exists: boolean }>('/api/directory/exists', { Path: path })
      .pipe(map(response => response.exists));
  }
}
