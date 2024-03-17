import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { UserRegisterRequest } from '../common/models/user-register-request';
import { CommonService } from '../common/services/common-service';
import { UserService } from '../common/services/user-service';

@Component({
  selector: 'app-register',
  templateUrl: './register.html'
})
export class Register  implements OnInit, OnDestroy {
  private timerId: NodeJS.Timer;
  imgCaptcha = '';
  clientGuid = '';
  userRegister = new FormGroup({
    userName: new FormControl('', Validators.required),
    password: new FormControl('', Validators.required),
    confirmPassword: new FormControl('', Validators.required),
    firstName: new FormControl('', Validators.required),
    lastName: new FormControl('', Validators.required),
    phoneNumber: new FormControl('', Validators.required),
    email: new FormControl(''),
    captcha: new FormControl('', Validators.required),
  });

  constructor(private userService: UserService,
    private router: Router,
    private commonService: CommonService) {
  }
  ngOnDestroy(): void {
    if (this.timerId) {
      clearInterval(this.timerId);
    }
    }
  ngOnInit() {
    this.timerId = setInterval(() => {
      this.getCaptcha();
    }, 2 * 60 * 1000);
    this.clientGuid = this.commonService.generateGuid();
    this.getCaptcha();
  }

  submit() {
    if (!this.userRegister.valid) {
      return;
    }
    const item = this.userRegister.value as UserRegisterRequest;
    item.clientGuid = this.clientGuid;
    this.userService.register(item).subscribe(response => {
      if (response.isValid) {
        this.router.navigate(["/login"]);
      }
      else {
        this.getCaptcha();
      }
    }, (error) => {
      this.getCaptcha();
    });
  }
  getCaptcha() {
    this.userRegister.patchValue({ captcha: '' });
    this.userService.getCaptcha(this.clientGuid).subscribe(response => {
      this.imgCaptcha = response;
    });
  }
  get firstName() { return this.userRegister.get('firstName'); }
  get userName() { return this.userRegister.get('userName'); }
  get password() { return this.userRegister.get('password'); }
  get confirmPassword() { return this.userRegister.get('confirmPassword'); }
  get lastName() { return this.userRegister.get('lastName'); }
  get phoneNumber() { return this.userRegister.get('phoneNumber'); }
  get email() { return this.userRegister.get('email'); }
  get captcha() { return this.userRegister.get('captcha'); }

}
