export interface UserLoginRequest {
  token: string;
  username: string;
  password: string;
  confirmPassword: string;
  rememberMe: boolean;
  captcha: string;
  clientGuid: string;
  email: string
}
