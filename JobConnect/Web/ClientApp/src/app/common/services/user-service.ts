import { HttpClient, HttpErrorResponse, HttpHeaders } from "@angular/common/http";
import { Inject, Injectable } from "@angular/core";
import { Router } from "@angular/router";
import { BehaviorSubject, Observable, throwError } from "rxjs";
import { catchError, finalize, map } from "rxjs/operators";

import { UserLoginRequest } from "../models/user-login-request";
import { UserRegisterRequest } from "../models/user-register-request";
import { AuthTokenType } from "./../models/auth-token-type";
import { AuthUser } from "./../models/auth-user";
import { ApiConfigService } from "./api-config.service";
import { APP_CONFIG, IAppConfig } from "./app.config";
import { MessageService } from "./message-service";
import { RefreshTokenService } from "./refresh-token.service";
import { TokenStoreService } from "./token-store.service";

@Injectable({
  providedIn: 'root'
})
export class UserService {

  private authStatusSource = new BehaviorSubject<boolean>(false);
  authStatus$ = this.authStatusSource.asObservable();

  constructor(private http: HttpClient,
    private router: Router,
    @Inject(APP_CONFIG) private appConfig: IAppConfig,
    private apiConfigService: ApiConfigService,
    private tokenStoreService: TokenStoreService,
    private refreshTokenService: RefreshTokenService,
    private messageService: MessageService) {

    this.updateStatusOnPageRefresh();
    this.refreshTokenService.scheduleRefreshToken(this.isAuthUserLoggedIn(), false);
  }

  login(item: UserLoginRequest) {
    const headers = new HttpHeaders({ "Content-Type": "application/json" });
    return this.http.post(`${this.appConfig.apiEndpoint}/user/login`, item, { headers: headers })
      .pipe(map((response: any) => {

        if (!response.isValid) {
          this.authStatusSource.next(false);
          this.messageService.error(response.message);
          return ;
        }

        this.tokenStoreService.setRememberMe(item.rememberMe);
        this.tokenStoreService.storeLoginSession(response);
        this.refreshTokenService.scheduleRefreshToken(true, true);
        this.authStatusSource.next(true);
        return response;
      }));
  }
  register(item: UserRegisterRequest) {
    const headers = new HttpHeaders({ "Content-Type": "application/json" });
    let url = `${this.appConfig.apiEndpoint}/user/Create`;
    return this.http.post(url, item, { headers: headers }).pipe(map((response: any) => {
      if (!response.isValid) {
        this.authStatusSource.next(false);
        this.messageService.error(response.message);
        return response;
      }

      return response;
    }));
  }
  getCaptcha(clientGuid: string): Observable<string> {
    let url = `${this.appConfig.apiEndpoint}/user/captcha?clientGuid=${clientGuid}`;
    return this.http.get(url).pipe(map((response: any) => {
      return 'data:image/gif;base64,' + response.message.split('?')[0];
    }));
  }

  getBearerAuthHeader(): HttpHeaders {
    return new HttpHeaders({
      "Content-Type": "application/json",
      "Authorization": `Bearer ${this.tokenStoreService.getRawAuthToken(AuthTokenType.AccessToken)}`
    });
  }

  logout(navigateToHome: boolean): void {
    const headers = new HttpHeaders({ "Content-Type": "application/json" });
    const refreshToken = encodeURIComponent(this.tokenStoreService.getRawAuthToken(AuthTokenType.RefreshToken));
    this.http.get(`${this.appConfig.apiEndpoint}/user/logout?refreshToken=${refreshToken}`,
      { headers: headers })
      .pipe(map(response => response || {}),
        catchError((error: HttpErrorResponse) => throwError(error)),
        finalize(() => {
          this.tokenStoreService.deleteAuthTokens();
          this.refreshTokenService.unscheduleRefreshToken(true);
          this.authStatusSource.next(false);
          if (navigateToHome) {
            this.router.navigate(["/"]);
          }
        }))
      .subscribe(result => {
        console.log("logout", result);
      });
  }

  isAuthUserLoggedIn(): boolean {
    return this.tokenStoreService.hasStoredAccessAndRefreshTokens() &&
      !this.tokenStoreService.isAccessTokenTokenExpired();
  }

  getAuthUser(): AuthUser | null {
    if (!this.isAuthUserLoggedIn()) {
      return null;
    }

    const decodedToken = this.tokenStoreService.getDecodedAccessToken();
    const roles = this.tokenStoreService.getDecodedTokenRoles();
    return Object.freeze({
      userId: decodedToken["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"],
      userName: decodedToken["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"],
      displayName: decodedToken["DisplayName"],
      roles: roles
    });
  }

  isAuthUserInRoles(requiredRoles: string[]): boolean {
    const user = this.getAuthUser();
    if (!user || !user.roles) {
      return false;
    }

    //if (user.roles.indexOf(this.apiConfigService.configuration.adminRoleName.toLowerCase()) >= 0) {
    //  return true; // The `Admin` role has full access to every pages.
    //}

    return requiredRoles.some(requiredRole => {
      if (user.roles) {
        return user.roles.indexOf(requiredRole.toLowerCase()) >= 0;
      } else {
        return false;
      }
    });
  }

  isAuthUserInRole(requiredRole: string): boolean {
    return this.isAuthUserInRoles([requiredRole]);
  }

  private updateStatusOnPageRefresh(): void {
    this.authStatusSource.next(this.isAuthUserLoggedIn());
  }
}
