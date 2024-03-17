import { Component, Inject, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { UserService } from '../common/services/user-service';
import { Router } from '@angular/router';
import { UserLoginRequest } from '../common/models/user-login-request';
import { HttpClient } from '@angular/common/http';
import { APP_CONFIG, IAppConfig } from '../common/services/app.config';
import { ToastrService } from 'ngx-toastr';
import { CommonService } from '../common/services/common-service';

@Component({
  selector: 'app-forget-password',
  templateUrl: './forget-password.component.html',
})
export class ForgetPasswordComponent implements OnInit {
  private clientGuid = this.commonService.generateGuid();
  imgCaptcha = '';
  userData = new FormGroup({
    email: new FormControl('', Validators.required),
    captcha: new FormControl('', Validators.required),
  });
  constructor(
    private userService: UserService,
    private http: HttpClient,
    @Inject(APP_CONFIG) private appConfig: IAppConfig,
    private router: Router,
    private toastr: ToastrService,
    private commonService: CommonService
  ) {}
  ngOnInit(): void {
    this.getCaptcha();
  }

  linkSend() {
    const item = this.userData.value as UserLoginRequest
    item.clientGuid = this.clientGuid;
    this.http
      .post(`${this.appConfig.apiEndpoint}/user/ForgottenPassword`, item)
      .subscribe((response: any) => {

        if (response.isValid) {
          this.showSuccessToastr()
        }
        else {
          this.getCaptcha();
        }
      }, (error) => {
        this.showErrorToastr()
        this.getCaptcha();
      });

  }
  showSuccessToastr() {
    this.toastr.success('لینک فراموشی رمزعبور با موفقیت برای شما ارسال شد.');
  }
  showErrorToastr() {
    this.toastr.error('ارسال لینک فراموشی رمزعبور با خطا مواجه شد.');
  }

  getCaptcha() {
    this.userData.patchValue({ captcha: '' });
    this.userService.getCaptcha(this.clientGuid).subscribe((response) => {
      this.imgCaptcha = response;
    });
  }
}
