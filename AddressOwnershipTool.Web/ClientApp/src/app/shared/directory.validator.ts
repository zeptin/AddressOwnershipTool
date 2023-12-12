import { AbstractControl, ValidationErrors, AsyncValidatorFn } from '@angular/forms';
import { Observable, of } from 'rxjs';
import { map, catchError, tap } from 'rxjs/operators';
import { DirectoryValidationService } from './directory.service';

export function directoryExistsValidator(directoryService: DirectoryValidationService): AsyncValidatorFn {
  return (control: AbstractControl): Observable<ValidationErrors | null> => {
    if (!control.value) {
      return of(null);
    }

    return directoryService.checkDirectoryExists(control.value).pipe(
      map(exists => exists ? null : { directoryNotFound: true }),
      catchError(() => of(null))
    );
  };
}
