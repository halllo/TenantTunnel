import { Component, OnInit } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Api } from '../../services/api.service';
import { Auth } from '../../services/auth.service';
import { asEnumerable } from 'linq-es2015';
import { environment } from '../../../environments/environment';

@Component({
  templateUrl: 'dashboard.component.html',
  styleUrls: [ 'dashboard.component.css' ]
})
export class DashboardComponent implements OnInit {

  access_token: string;
  endpoint: string;
  method: string;
  argument: string;
  responses: Response[] = [];

  constructor(private auth: Auth, public api: Api) {
  }

  ngOnInit(): void {
    this.auth.acquireToken(environment.adalConfigApiEndpoint).subscribe(token => {
      this.access_token = token;
    }, err => {
      console.error(err);
      alert(err);
    });
  }

  async onSubmit(form) {
    try {
      const response = await this.api.requestThroughTunnel(this.endpoint, this.method, this.argument).toPromise();
      this.responses.push(<Response> {
        url: environment.backend_api + `/api/tunnel/${this.endpoint}/${this.method}?a=${this.argument}`,
        received: new Date(),
        body: response
      })
    } catch (err) {
      this.responses.push(<Response> {
        url: environment.backend_api + `/api/tunnel/${this.endpoint}/${this.method}?a=${this.argument}`,
        received: new Date(),
        body: err,
        error: true
      })
    }
  }
}

class Response {
  url: string;
  received: Date;
  body: string;
  error: boolean;
}
