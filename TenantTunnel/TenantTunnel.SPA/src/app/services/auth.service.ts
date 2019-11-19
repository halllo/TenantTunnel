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

      if (!environment.production) {
        console.warn(window.location.hash);
      }

      this.context.handleWindowCallback();

      const cachedUser = this.context.getCachedUser();
      if (cachedUser && !cachedUser.profile.upn) {
        console.warn('User does not have a UPN. Redirect to get Access-Token to make sure to not get an access token for different logged-in user!', cachedUser);
      }
    }

    this.context.getUser((error, u: AuthenticationContext.UserInfo) => {
        this.user = u;
    });
  }

  public login() {
    this.context.login();
  }

  public adminConsent() {
    this.context.config.extraQueryParameter = 'prompt=admin_consent';
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

    const cachedUser = this.context.getCachedUser();
    if (cachedUser && !cachedUser.profile.upn) {
      const cachedToken = this.context.getCachedToken(resource);
      if (cachedToken) {
        subject.next(cachedToken);
        subject.complete();
      } else {
        /* 
        * The current user does not have a UPN that could be used as login_hint.
        * Just invoking acquireToken would then get an access token for the first signed in user in an iframe.
        * When there are different users logged in, the access token could be for a different account than the used for login and the user does not see that.
        * Therefore we invoke acquireTokenRedirect to explicitly get an access token for whatever account the user wants.
        * No more silent fail but a little less convenient. Only in case there is no UPN claim, which happpens for guest accounts in AAD.
        */
        this.context.acquireTokenRedirect(resource);
      }
    } else {
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
    }

    return observable;
  }
}
