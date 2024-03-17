import { Component, Inject, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { UserService } from '../common/services/user-service';
import { Router } from '@angular/router';
import { UserLoginRequest } from '../common/models/user-login-request';
import { CommonService } from '../common/services/common-service';
import { HttpClient } from '@angular/common/http';
import { APP_CONFIG, IAppConfig } from '../common/services/app.config';
import { ActivatedRoute } from '@angular/router';


@Component({
  selector: 'app-reset-password',
  templateUrl: './reset-password.html',
})
export class resetPassword implements OnInit {
  private clientGuid = this.commonService.generateGuid();
  imgCaptcha = '';
  userName: string = '';
  token: string = '';
  userDataResetPassword = new FormGroup({
    password: new FormControl('', Validators.required),
    confirmPassword: new FormControl('', Validators.required),
    captcha: new FormControl('', Validators.required),
  });
  constructor(
    private userService: UserService,
    private http: HttpClient,
    @Inject(APP_CONFIG) private appConfig: IAppConfig,
    private commonService: CommonService,
    private route: ActivatedRoute
  ) {}
  ngOnInit(): void {
    this.getCaptcha(),
    this.route.queryParams.subscribe((q1) => {
      const t = this.route.snapshot.queryParams.token;
      const userName = this.route.snapshot.queryParams.username;
      this.userName = userName;
      this.token = t;
    });
  }
  resetPassword() {
    const item = this.userDataResetPassword.value as UserLoginRequest;
    item.clientGuid = this.clientGuid;
    item.username = this.userName;
    item.token = this.token
    this.http
      .post(`${this.appConfig.apiEndpoint}/user/ResetPassword`, item)
      .subscribe(
        (response: any) => {
          if (response.isValid) {
          } else {
            this.getCaptcha();
          }
        },
        (error) => {
          this.getCaptcha();
        }
      );
  }
  getCaptcha() {
    this.userDataResetPassword.patchValue({ captcha: '' });
    this.userService.getCaptcha(this.clientGuid).subscribe((response) => {
      this.imgCaptcha = response;
    });
  }
}
