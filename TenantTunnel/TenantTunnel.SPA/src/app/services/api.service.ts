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

  public loadUser(): Observable<UserDto> {
    return of(0).pipe(
      tap(x => this.loading = true),
      mergeMap(x => this.auth.acquireToken(environment.adalConfigApiEndpoint)),
      mergeMap(token => {
        return this.http.get(environment.backend_api + '/api/user', {
          headers: new HttpHeaders({
            'Authorization': `Bearer ${token}`
          })
        });
      }),
      map(result => <UserDto>result),
      finalize(() => this.loading = false),
      share()
    );
  }

  public storeKeypair(keypair: KeypairDto): Observable<KeypairDto> {
    return of(0).pipe(
      tap(x => this.loading = true),
      mergeMap(x => this.auth.acquireToken(environment.adalConfigApiEndpoint)),
      mergeMap(token => {
        return this.http.post(environment.backend_api + '/api/user/keypair', keypair, {
          headers: new HttpHeaders({
            'Authorization': `Bearer ${token}`
          })
        });
      }),
      map(result => <KeypairDto>result),
      finalize(() => this.loading = false),
      share()
    );
  }

  public loadUsers(sender: MessageSenderDto): Observable<UserDto[]>;
  // tslint:disable-next-line:unified-signatures
  public loadUsers(searchterm: string): Observable<UserDto[]>;
  public loadUsers(senderOrSearchterm: MessageSenderDto | string): Observable<UserDto[]> {
    if (typeof senderOrSearchterm === 'string') {
      return of(0).pipe(
        tap(x => this.loading = true),
        mergeMap(x => this.auth.acquireToken(environment.adalConfigApiEndpoint)),
        mergeMap(token => {
          return this.http.get(environment.backend_api + `/api/users?searchterm=${senderOrSearchterm}`, {
            headers: new HttpHeaders({
              'Authorization': `Bearer ${token}`
            })
          });
        }),
        map(result => <UserDto[]>result),
        finalize(() => this.loading = false),
        share()
      );
    } else {
      return of(0).pipe(
        tap(x => this.loading = true),
        mergeMap(x => this.auth.acquireToken(environment.adalConfigApiEndpoint)),
        mergeMap(token => {
          return this.http.get(environment.backend_api + `/api/users?upn=${senderOrSearchterm.upn}`, {
            headers: new HttpHeaders({
              'Authorization': `Bearer ${token}`
            })
          });
        }),
        map(result => <UserDto[]>result),
        finalize(() => this.loading = false),
        share()
      );
    }
  }

  public publishMessage(message: MessageDto): Observable<MessageDto> {
    return of(0).pipe(
      tap(x => this.loading = true),
      mergeMap(x => this.auth.acquireToken(environment.adalConfigApiEndpoint)),
      mergeMap(token => {
        return this.http.post(environment.backend_api + '/api/messages', message, {
          headers: new HttpHeaders({
            'Authorization': `Bearer ${token}`
          })
        });
      }),
      map(result => <MessageDto>result),
      finalize(() => this.loading = false),
      share()
    );
  }

  public loadMessages(toUpn?: string): Observable<MessageDto[]> {
    return of(0).pipe(
      tap(x => this.loading = true),
      mergeMap(x => this.auth.acquireToken(environment.adalConfigApiEndpoint)),
      mergeMap(token => {
        return this.http.get(environment.backend_api + '/api/messages' + (toUpn ? '?toUpn=' + toUpn : ''), {
          headers: new HttpHeaders({
            'Authorization': `Bearer ${token}`
          })
        });
      }),
      map(result => <MessageDto[]>result),
      finalize(() => this.loading = false),
      share()
    );
  }
}

export class UserDto {
  name: string;
  upn: string;
  keypair: KeypairDto;
}

export class KeypairDto {
  publicKey: string;
  encryptedPrivateKey: string;
  encryptedPrivateKeyIv: string;
}

export class MessageDto {
  sender: MessageSenderDto;
  sent: Date;
  recipients: MessageRecipientDto[];
  nonce: string;
  body: string;
}

export class MessageSenderDto {
  name: string;
  upn: string;
}

export class MessageRecipientDto {
  name: string;
  upn: string;
  encryptedMessageKey: string;
  nonce: string;
}
