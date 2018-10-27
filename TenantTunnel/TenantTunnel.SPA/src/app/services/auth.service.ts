import { Injectable } from '@angular/core';
import * as AuthenticationContext from 'adal-angular';
import { Observable, ReplaySubject } from 'rxjs';
import { environment } from '../../environments/environment';

@Injectable()
export class Auth {

  private context: AuthenticationContext;
  private user: AuthenticationContext.UserInfo = null;
  constructor() { }

  public init() {
    this.context = new AuthenticationContext(environment.adalConfig);

    if (window.location.hash.includes('#id_token=')
    || window.location.hash.includes('#access_token=')
    || window.location.hash.startsWith('#error=')) {
      this.context.handleWindowCallback();
    }

    this.context.getUser((error, u: AuthenticationContext.UserInfo) => {
        this.user = u;
    });
  }

  public login() {
    this.context.login();
  }

  public logout() {
    this.context.logOut();
  }

  public get username(): string {
    return this.user.userName;
  }

  public get isAuthenticated(): boolean {
    return this.user != null;
  }

  public acquireToken(resource: string): Observable<string> {
    const subject = new ReplaySubject<string>();
    const observable = subject.asObservable();
    this.context.acquireToken(resource, (er, token: string) => {
      if (er) {
        if (typeof er === 'string' && er.startsWith('AADSTS65001: The user or administrator has not consented')) {
          console.warn('starting redirect for interactive auth...', er);
          this.context.acquireTokenRedirect(resource);
        } else { 
          subject.error(er);
        }
      } else {
          subject.next(token);
          subject.complete();
      }
    });
    return observable;
  }
}
