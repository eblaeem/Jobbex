import { HttpClient } from '@angular/common/http';
import { Component, Inject, OnInit } from '@angular/core';
import { APP_CONFIG, IAppConfig } from '../common/services/app.config';


@Component({
  selector: 'app-resumeh',
  templateUrl: './resumeh.html'
})
export class Resumeh implements OnInit {
  userProfileImage = '';
  userProfile: any = {};
  userSkills = [];

  constructor(private http: HttpClient,
    @Inject(APP_CONFIG) private appConfig: IAppConfig) {
  }
  ngOnInit() {
    this.http.get(`${this.appConfig.apiEndpoint}/userProfile/getResumeh`).subscribe((response: any) => {
      this.userProfile = response;
      if (!response.userProfileImage) {
        this.userProfileImage = '../assets/images/jobs-company/pic1.jpg';
      }
      this.userProfileImage = response.userProfileImage;
    });
  }
}
