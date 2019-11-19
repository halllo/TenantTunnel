import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { AppRoutingModule } from './app.routing';
import { AppComponent } from './app.component';
import { SimpleLayoutComponent, FullLayoutComponent } from './container';
import { P404Component } from './views/404/404.component';
import { RedirectingComponent } from './views/redirecting/redirecting.component';
import { AuthenticatedGuard, NotAuthenticatedGuard, AdalJsIFrameGuard } from './services/auth-guard.service';
import { Api } from './services/api.service';
import { Auth } from './services/auth.service';

@NgModule({
  imports: [
    BrowserModule,
    HttpClientModule,
    AppRoutingModule,
    NgbModule,
  ],
  declarations: [
    AppComponent,
    SimpleLayoutComponent,
    FullLayoutComponent,
    P404Component,
    RedirectingComponent,
  ],
  providers: [
    Auth,
    AuthenticatedGuard,
    NotAuthenticatedGuard,
    AdalJsIFrameGuard,
    Api
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
