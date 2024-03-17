import { HttpClient } from "@angular/common/http";
import { Inject, Injectable } from "@angular/core";
import { Observable, tap } from "rxjs";

import { APP_CONFIG, IAppConfig } from "./app.config";

@Injectable({
  providedIn: 'root'
})
export class ApiConfigService {
  private config: IApiConfig | null = null;

  constructor(private http: HttpClient,
    @Inject(APP_CONFIG) private appConfig: IAppConfig) { }

  loadApiConfig(): Observable<any> {
    const url = `${this.appConfig.apiEndpoint}/${this.appConfig.apiSettingsPath}`;
    return this.http.get<IApiConfig>(url).pipe(tap(config => {
      this.config = config;
    }));
  }

  get configuration(): IApiConfig {
    if (!this.config) {
      throw new Error("Attempted to access configuration property before configuration data was loaded.");
    }
    return this.config;
  }
}

export interface IApiConfig {
  loginPath: string;
  logoutPath: string;
  refreshTokenPath: string;
  accessTokenObjectKey: string;
  refreshTokenObjectKey: string;
  adminRoleName: string;
}
