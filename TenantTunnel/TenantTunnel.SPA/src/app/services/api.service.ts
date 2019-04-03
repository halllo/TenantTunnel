import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { map, mergeMap, filter, scan, tap, catchError, merge, finalize, share } from 'rxjs/operators';
import { of } from 'rxjs/observable/of';
import { Observable, Subject } from 'rxjs';
import { environment } from '../../environments/environment';
import { Auth } from './auth.service';

@Injectable()
export class Api {

  public loading: Boolean;

  constructor(private http: HttpClient, private auth: Auth) { }

  public requestThroughTunnel(endpoint: string, method: string, argument: string, subjectIsolation: boolean): Observable<string> {
    return of(0).pipe(
      tap(x => this.loading = true),
      mergeMap(x => this.auth.acquireToken(environment.adalConfigApiEndpoint)),
      mergeMap(token => {
        return this.http.post(
          environment.backend_api + `/api/tunnel/${endpoint}/${method}` + (subjectIsolation ? '?isolation=subject' : ''), 
          argument,
          {
            headers: new HttpHeaders({
              'Authorization': `Bearer ${token}`
            }),
            responseType: 'text'
          });
      }),
      map(result => result),
      finalize(() => this.loading = false),
      share()
    );
  }
}
