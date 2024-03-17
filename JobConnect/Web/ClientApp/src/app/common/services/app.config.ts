import { InjectionToken } from "@angular/core";
import { environment } from "../../../environments/environment";

export let APP_CONFIG = new InjectionToken<string>("app.config");

export interface IAppConfig {
  apiEndpoint: string;
  apiSettingsPath: string;
}

export const AppConfig: IAppConfig = {
  //apiEndpoint: "https://localhost:7022",////https://localhost:7022 http://www.jobbex.ir
  apiEndpoint: environment.API_URL,
  apiSettingsPath: "ApiSettings"
};
