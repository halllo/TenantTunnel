import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot } from '@angular/router';
import { Router } from '@angular/router';
import { Auth } from './auth.service';

@Injectable()
export class AuthenticatedGuard implements CanActivate {
  constructor(private router: Router, private auth: Auth) { }

  canActivate(route: ActivatedRouteSnapshot) {
    return this.auth.isAuthenticated;
  }
}

@Injectable()
export class NotAuthenticatedGuard implements CanActivate {
  constructor(private router: Router, private auth: Auth) { }

  canActivate(route: ActivatedRouteSnapshot) {
    if (this.auth.isAuthenticated) {
      this.router.navigate(['dashboard']);
      return false;
    } else {
      return true;
    }
  }
}

@Injectable()
export class AdalJsIFrameGuard implements CanActivate {
  constructor(private router: Router) { }

  canActivate(route: ActivatedRouteSnapshot) {
    return !this.isInAdalIFrame;
  }

  /**
   * Adal.Js creates a hidden iframe for getting the access-token to the requested resource.
   * This iframe attempts to load the entire web app inside that iframe even though
   * it only needs the app to handleWindowCallback inside AppComponent.
   * This router guard checks if it is running inside that iframe and if it does, it prevents the home modules from being initialized.
   * Now the home modules don't need to be carefull when calling remote backends.
   */
  public get isInAdalIFrame(): boolean {
    return window.frameElement != null && window.frameElement.id.includes('adalRenewFrame');
  }
}
