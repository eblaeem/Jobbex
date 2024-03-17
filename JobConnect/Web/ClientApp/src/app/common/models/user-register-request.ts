export interface UserRegisterRequest {
  username: string;
  password: string;
  confirmPassword: string;
  firstName: string;
  lastName: string;
  phoneNumber: string;
  email: string;
  captcha: string;
  clientGuid: string;
}
