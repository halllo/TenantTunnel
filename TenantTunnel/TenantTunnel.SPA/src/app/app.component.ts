import { Component, OnInit } from '@angular/core';
import { environment } from '../environments/environment';
import { Auth } from './services/auth.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {

  constructor(private auth: Auth) {
  }

  ngOnInit() {

    this.auth.init();

  }
}
