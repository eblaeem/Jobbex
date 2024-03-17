import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { UserLoginRequest } from '../common/models/user-login-request';
import { CommonService } from '../common/services/common-service';
import { UserService } from '../common/services/user-service';
import { log } from 'console';

@Component({
  selector: 'app-login',
  templateUrl: './login.html',
})
export class LoginComponent implements OnInit, OnDestroy {
  private returnUrl = '';
  private clientGuid = '';
  private timerId: NodeJS.Timer;
  imgCaptcha = '';
  userLogin = new FormGroup({
    userName: new FormControl('', Validators.required),
    password: new FormControl('', Validators.required),
    captcha: new FormControl(''),
    rememberMe: new FormControl(false),
    input1Value : new FormControl(''),
    input2Value : new FormControl(''),
    input3Value : new FormControl(''),
    input4Value : new FormControl(''),
    input5Value : new FormControl(''),
  });

  constructor(
    private userService: UserService,
    private router: Router,
    private route: ActivatedRoute,
    private commonService: CommonService
  ) {}
  ngOnDestroy(): void {
    if (this.timerId) {
      clearInterval(this.timerId);
    }
  }

  ngOnInit() {
    this.timerId = setInterval(() => {
      this.getCaptcha();
    }, 2 * 60 * 1000);
    this.userService.logout(false);
    this.returnUrl = this.route.snapshot.queryParams['returnUrl'];
    this.clientGuid = this.commonService.generateGuid();
    this.getCaptcha();
  }

  clearInput(inputField: string): void {
    const item = this.userLogin.value as any;
    switch (inputField) {
      case 'input1':
        item.input1Value = '';
        break;
      case 'input2':
        item.input2Value = '';
        break;
      case 'input3':
        item.input3Value = '';
        break;
      case 'input4':
        item.input4Value = '';
        break;
      case 'input5':
        item.input5Value = '';
        break;
      default:
        break;
    }
  }

  login() {
    if (!this.userLogin.valid) {
      return;
    }
    const item = this.userLogin.value as any;
    item.captcha =
    item.input5Value +
    item.input4Value +
    item.input3Value +
    item.input2Value +
    item.input1Value;
    console.log(item.captcha);
    item.clientGuid = this.clientGuid;
    this.userService.login(item).subscribe(
      (response) => {
        if (response.isValid) {
          if (this.returnUrl) {
            this.router.navigate([this.returnUrl]);
          } else {
            this.router.navigate(['/']);
          }
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
    this.userLogin.patchValue({ captcha: '' });
    this.userService.getCaptcha(this.clientGuid).subscribe((response) => {
      this.imgCaptcha = response;
    });
  }
}
