import { Component } from '@angular/core';
import { Auth } from '../../services/auth.service';

@Component({
  templateUrl: 'welcome.component.html',
  styleUrls: [ 'welcome.component.css' ]
})
export class WelcomeComponent {

  constructor(public auth: Auth) {

  }

}
