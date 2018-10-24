import { Component } from '@angular/core';
import { Auth } from '../../services/auth.service';

@Component({
  selector: 'app-dashboard',
  templateUrl: './full-layout.component.html',
  styleUrls: ['./full-layout.component.css']
})
export class FullLayoutComponent {

  navbarCollapsed = true;

  constructor(public auth: Auth) {

   }

}
