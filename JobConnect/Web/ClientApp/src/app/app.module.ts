import { HTTP_INTERCEPTORS } from "@angular/common/http";
import { APP_INITIALIZER } from "@angular/core";
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { Select2Module } from 'ng-select2-component';
import { NgxPaginationModule } from 'ngx-pagination';

import { ToastrModule } from 'ngx-toastr';
import { ApiConfigService } from "./common/services/api-config.service";
import { AppConfig, APP_CONFIG } from "./common/services/app.config";
import { AuthInterceptor } from "./common/services/auth.interceptor";
import { XsrfInterceptor } from "./common/services/xsrf.interceptor";


import { HttpClientModule } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { BrowserModule } from '@angular/platform-browser';
import { RouterModule } from '@angular/router';
import { NotFound } from './404/404';

import { NgPersianDatepickerModule } from "ng-persian-datepicker";
import { AboutUs } from './about-use/about-us';
import { AccessDenied } from "./access-denied/access-denied";
import { AppComponent } from './app.component';
import { ChangePassword } from './change-password/change-password';
import { ModalComponent } from "./common/app-modal/app-modal";
import { AppReadMore } from "./common/app-readmore/app-readmore";
import { AuthGuard } from "./common/services/auth.guard";
import { Companies } from "./company/companies";
import { Company } from "./company/company";
import { ContactUs } from './contact-us/contact-us';
import { Faq } from './faq/faq';
import { Footer } from './footer/footer';
import { HomeComponent } from './home/home.component';
import { JobDetail } from './job-detail/job-detail';
import { JobRecommended } from "./job-recommended/job-recommended";
import { Jobs } from './jobs/jobs';
import { LoginComponent } from './login/login';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { Pricing } from './pricing/pricing';
import { Profile } from './profile/profile';
import { Register } from './register/register';
import { Resumeh } from "./resumeh/resumeh";
import { Term } from "./term/term";
import { UserPinJobs } from "./user-pin-jobs/user-pin-jobs";
import { ForgetPasswordComponent } from './forget-password/forget-password.component';
import { resetPassword } from './reset-password/reset-password';
import { AutoTabDirective } from './auto-tab.directive';
import { CommonModule } from "@angular/common";

@NgModule({
  declarations: [
    AppComponent,
    AppReadMore,
    NavMenuComponent,
    Footer,
    HomeComponent,
    LoginComponent,
    Jobs,
    JobDetail,
    Register,
    ChangePassword,
    Profile,
    AboutUs,
    Pricing,
    NotFound,
    Faq,
    ContactUs,
    ModalComponent,
    AccessDenied,
    Resumeh,
    JobRecommended,
    Company,
    Companies,
    AppReadMore,
    UserPinJobs,
    ForgetPasswordComponent,
    resetPassword,
    AutoTabDirective,
  ],
  imports: [
    //BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    BrowserModule,
    BrowserAnimationsModule,
    NgxPaginationModule,
    NgPersianDatepickerModule,
    HttpClientModule,
    FormsModule,
    ReactiveFormsModule,
    Select2Module,
    CommonModule,
    ToastrModule.forRoot({
      timeOut: 5000,
      positionClass: 'toast-top-right',
      preventDuplicates: true,
    }),
    RouterModule.forRoot([
      { path: '', component: HomeComponent, pathMatch: 'full', data: { homePage: true, visible: true, title: 'صفحه اصلی'} },
      { path: 'home', component: HomeComponent, pathMatch: 'full', data: { homePage: true, visible: true, title: 'صفحه اصلی' } },
      { path: 'login', component: LoginComponent, data: { visible: false, title: 'ورود به سایت' } },
      { path: 'register', component: Register, data: { visible: false, title: 'ثبت نام' } },
      {
        path: 'profile', component: Profile, data: {
          title: 'پروفایل کاربری',
          animation: 1,
          visible: true,
          permission: { permittedRoles: ["User"] }
        }, canActivate: [AuthGuard]
      },
      {
        path: 'resumeh', component: Resumeh, data: {
          title: 'مشاهده رزومه',
          visible: true,
          permission: { permittedRoles: ["User"] }
        }, canActivate: [AuthGuard]
      },
      {
        path: 'job-recommended', component: JobRecommended, data: {
          title: 'پیشنهادهای شغلی',
          visible: true,
          permission: { permittedRoles: ["User"] }
        }, canActivate: [AuthGuard]
      },
      {
        path: 'change-password', component: ChangePassword, data: {
          title: 'تغییر رمز عبور',
          visible: true,
          permission: { permittedRoles: ["User"] }
        }, canActivate: [AuthGuard]
      },
      {
        path: 'forget-password', component: ForgetPasswordComponent, data: {
          title: 'فراموشی رمز عبور',
          visible: true,
        }
      },
      {
        path: 'reset-password/', component: resetPassword, data: {
          title: 'بازنشانی رمز عبور',
          visible: true,
        },children:[{
          path:':username',
          component:resetPassword
        }]
      },
      { path: 'companies', component: Companies, data: { visible: true, title: 'شرکتهای فعال' } },
      { path: 'company/:id', component: Company, data: { visible: true, title: 'شرکتهای فعال' } },
      { path: 'reset-password', component: resetPassword, data: { visible: true, title: 'ریست رمز عبور' } },
      { path: 'jobs', component: Jobs, data: { visible: true, title: 'فرصتهای شغلی' } },
      { path: 'job/:id', component: JobDetail, data: { visible: true, title: '' } },
      { path: 'about-us', component: AboutUs, data: { visible: true, title: 'درباره ما' } },
      { path: 'pricing', component: Pricing, data: { visible: true, title: 'قیمت گذاری' } },
      { path: 'faq', component: Faq, data: { visible: true, title: 'سوالات متداول' } },
      { path: 'contact-us', component: ContactUs, data: { visible: true, title: 'تماس با ما' } },
      { path: 'term', component: Term, data: { title: 'قوانین و مقررات' } },
      { path: 'user-pin-jobs', component: UserPinJobs, data: { visible: true, title: 'آگهی های نشان شده' } },
      { path: '404', component: NotFound, data: { visible: false, title: 'صفحه 404' } },
      { path: 'access-denied', component: AccessDenied, data: { visible: false, title: 'عدم دسترسی' } },
      { path: '**', component: NotFound, data: { visible: false, title: 'صفحه 404' } },
    ])
  ],
  providers: [
    {
      provide: APP_CONFIG,
      useValue: AppConfig
    },
    {
      provide: HTTP_INTERCEPTORS,
      useClass: XsrfInterceptor,
      multi: true
    },
    {
      provide: HTTP_INTERCEPTORS,
      useClass: AuthInterceptor,
      multi: true
    },
    {
      provide: APP_INITIALIZER,
      useFactory: (config: ApiConfigService) => () => config.loadApiConfig().subscribe(response => { }),
      deps: [ApiConfigService],
      multi: true
    }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
