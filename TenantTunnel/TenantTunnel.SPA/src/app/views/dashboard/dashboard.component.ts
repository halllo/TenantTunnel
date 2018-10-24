import { Component, OnInit } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Api, UserDto, MessageDto, MessageRecipientDto, MessageSenderDto } from '../../services/api.service';
import { Auth } from '../../services/auth.service';
import * as nacl from 'tweetnacl';
import { asEnumerable } from 'linq-es2015';

@Component({
  templateUrl: 'dashboard.component.html',
  styleUrls: [ 'dashboard.component.css' ]
})
export class DashboardComponent implements OnInit {

  public user: UserDto;

  constructor(private auth: Auth, public api: Api) {
  }

  public ngOnInit(): void {
    
  }

}
